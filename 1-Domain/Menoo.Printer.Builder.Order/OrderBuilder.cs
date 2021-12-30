using Menoo.Printer.Builder.Orders.Constants;
using Menoo.Printer.Builder.Orders.Repository;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Menoo.PrinterService.Infraestructure.Repository;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Menoo.Printer.Builder.Orders
{
    [Handler]
    public class OrderBuilder : ITicketBuilder
    {
        private const int PRINT_MINUTES_ORDER_TABLE = 60;

        private const int PRINT_MINUTES_ORDER_TA = 30;

        private const int PRINT_MINUTES_ORDER_BOOKING = 60;

        private const int PRINT_MINUTES_ORDER_REPRINT = 120;

        private readonly MenooContext _menooContext;

        private readonly BookingRepository _bookingRepository;

        private readonly OrderRepository _orderRepository;

        private readonly StoreRepository _storeRepository;

        public OrderBuilder(
            MenooContext menooContext,
            StoreRepository storeRepository,
            BookingRepository bookingRepository,
            OrderRepository orderRepository,
            IFirebaseStorage storageService)
        {
            _menooContext = menooContext;
            _storeRepository = storeRepository;
            _bookingRepository = bookingRepository;
            _orderRepository = orderRepository;
        }

        public async Task<PrintInfo> BuildAsync(string id, PrintMessage message)
        {
            if (message.Builder != PrintBuilder.ORDER_BUILDER)
            {
                return null;
            }
            var dataToPrint = new PrintInfo();
            bool isCancelled = message.PrintEvent == PrintEvents.ORDER_CANCELLED;
            bool isReprint = message.PrintEvent == PrintEvents.REPRINT_ORDER;
            if (message.DocumentsId?.Count > 0 && string.IsNullOrEmpty(message.DocumentId))
            {
                var orders = new List<OrderV2>();
                foreach (var documentId in message.DocumentsId)
                {
                    var order = await _orderRepository.GetOrderById(documentId);
                    orders.Add(order);
                }
                dataToPrint = await GetUnifiedOrderTicketAsync(orders, isCancelled, isReprint);
            }
            else
            {
                var order = await _orderRepository.GetOrderById(message.DocumentId);
                dataToPrint = await GetSingleOrderTicketAsync(order, isCancelled, isReprint);
            }
            return dataToPrint;
        }

        public override string ToString()
        {
            return PrintBuilder.ORDER_BUILDER;
        }

        #region private methods
        private Tuple<string, string> GenerateOrderQR(OrderV2 order)
        {
            OrderQR orderInfoQR = new OrderQR
            {
                OrderId = order.Id,
                OrderType = order.OrderType.Trim().Capitalize(),
                StoreId = order.Store.Id
            };
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(JsonConvert.SerializeObject(orderInfoQR, Formatting.Indented), QRCodeGenerator.ECCLevel.L);
            var imgType = Base64QRCode.ImageType.Png;
            Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            string qrCodeImageAsBase64 = qrCode.GetGraphic(20, Color.Black, Color.White, true, imgType);
            string imageType = imgType.ToString().ToLower();
            return new Tuple<string, string>(qrCodeImageAsBase64, imageType);
        }

        private byte[] GenerateOrderQRBytes(OrderV2 order)
        {
            var qrBase64 = GenerateOrderQR(order);
            byte[] imageBytes = Convert.FromBase64String(qrBase64.Item1);
            return imageBytes;
        }

        private async Task<PrintInfo> GetSingleOrderTicketAsync(OrderV2 order, bool isCancelled, bool isReprint)
        {
            bool isTakeAway = order.OrderType.ToUpper().Trim() == OrderTypes.TAKEAWAY;
            var store = await _storeRepository.GetById<Store>(order.Store.Id, "stores");
            var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            var viewBag = new Dictionary<string, object>();
            var info = new PrintInfo
            {
                Store = store
            };
            switch (order.OrderType.Trim().ToUpper())
            {
                case OrderTypes.MESA:
                    viewBag.Add("clientName", order.UserName);
                    viewBag.Add("tableNumber", order.Address);
                    viewBag.Add("orderNumber", order.OrderNumber);
                    viewBag.Add("title", isCancelled ? "orden de mesa cancelada" : "Nueva orden de mesa");
                    int minutes = isReprint ? PRINT_MINUTES_ORDER_REPRINT : PRINT_MINUTES_ORDER_TABLE;
                    info.BeforeAt = Utils.BeforeAt(now, minutes);
                    info.Template = PrintTemplates.NEW_TABLE_ORDER;
                    break;
                case OrderTypes.RESERVA:
                    var bookingData = await _bookingRepository.GetById<Booking>(order.BookingId);
                    viewBag.Add("bookingNumber", bookingData?.BookingNumber.ToString());
                    viewBag.Add("date", order.OrderDate);
                    viewBag.Add("clientName", order.UserName);
                    viewBag.Add("orderNumber", order.OrderNumber);
                    viewBag.Add("title", isCancelled ? "orden de reserva cancelada" : "Nueva orden de reserva");
                    info.BeforeAt = Utils.BeforeAt(now, PRINT_MINUTES_ORDER_BOOKING);
                    info.Template = PrintTemplates.NEW_BOOKING_ORDER;
                    break;
                case OrderTypes.TAKEAWAY:
                    var payment = _menooContext.Payments.FirstOrDefault(f => f.EntityId == order.Id);
                    viewBag.Add("taTime", order.TakeAwayHour);
                    viewBag.Add("clientName", order.UserName);
                    viewBag.Add("orderNumber", order.OrderNumber);
                    if (!isCancelled) 
                    {
                        var qrData = GenerateOrderQR(order);
                        viewBag.Add("qrCode", $"data:image/{qrData.Item2};base64, {qrData.Item1}");
                    }
                    viewBag.Add("title", isCancelled ? "Takeaway cancelado" : "Nuevo TakeAway");
                    info.BeforeAt = Utils.BeforeAt(now, PRINT_MINUTES_ORDER_TA);
                    info.Template = PrintTemplates.NEW_TAKEAWAY_ORDER;
                    break;
            }
            viewBag.Add("orderData", order);
            info.Content = viewBag;
            return info;
        }

        private async Task<PrintInfo> GetUnifiedOrderTicketAsync(List<OrderV2> orders, bool isCancelled, bool isReprint)
        {
            var orderUnified = new OrderV2();
            var items = new List<ItemOrderV2>();
            var ordersByStore = orders.GroupBy(g => g.Store.Id).FirstOrDefault();
            var ordersByOrderNumber = orders.GroupBy(g => g.OrderNumber).FirstOrDefault();
            var ordersByAddress = orders.GroupBy(g => g.Address).FirstOrDefault();
            var ordersByUsername = orders.GroupBy(g => g.UserName).FirstOrDefault();
            orderUnified.OrderNumber = ordersByOrderNumber.Key;
            orderUnified.Address = ordersByAddress.Key;
            orderUnified.UserName = ordersByUsername.Key;
            orderUnified.Store = ordersByStore.ToList().FirstOrDefault().Store;
            orderUnified.OrderType = ordersByStore.ToList().FirstOrDefault().OrderType;
            orders.ForEach(item => {
                items.AddRange(item.Items);
            });
            orderUnified.Items = items;
            var printInfo = await GetSingleOrderTicketAsync(orderUnified, isCancelled, isReprint);
            return printInfo;
        }
        #endregion
    }
}
