using Menoo.Printer.Builder.Orders.Constants;
using Menoo.Printer.Builder.Orders.Extensions;
using Menoo.Printer.Builder.Orders.Repository;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema;
using Menoo.PrinterService.Infraestructure.Enums;
using Menoo.PrinterService.Infraestructure.Extensions;
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
using System.Text;
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

        private readonly PrinterContext _printerContext;

        private readonly EventLog _generalWriter;

        private readonly BookingRepository _bookingRepository;

        private readonly ItemRepository _itemRepository;

        private readonly OrderRepository _orderRepository;

        private readonly StoreRepository _storeRepository;

        private readonly TicketRepository _ticketRepository;

        public OrderBuilder(
            MenooContext menooContext,
            PrinterContext printerContext,
            StoreRepository storeRepository,
            TicketRepository ticketRepository,
            BookingRepository bookingRepository,
            OrderRepository orderRepository,
            ItemRepository itemRepository)
        {
            _menooContext = menooContext;
            _printerContext = printerContext;
            _storeRepository = storeRepository;
            _ticketRepository = ticketRepository;
            _bookingRepository = bookingRepository;
            _orderRepository = orderRepository;
            _itemRepository = itemRepository;
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("builder");
        }

        public async Task<List<PrintInfo>> BuildAsync(string id, PrintMessage message)
        {
            if (message.Builder != PrintBuilder.ORDER_BUILDER)
            {
                return null;
            }
            var dataToPrint = new List<PrintInfo>();
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
                await GetUnifiedOrderTicketAsync(orders, isCancelled, isReprint, dataToPrint);
            }
            else
            {
                var order = await _orderRepository.GetOrderById(message.DocumentId);
                await GetSingleOrderTicketAsync(order, isCancelled, isReprint, dataToPrint);
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
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(JsonConvert.SerializeObject(orderInfoQR, Formatting.Indented), QRCodeGenerator.ECCLevel.Q);
            var imgType = Base64QRCode.ImageType.Jpeg;
            Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            string qrCodeImageAsBase64 = qrCode.GetGraphic(20, Color.Black, Color.White, true, imgType);
            //string htmlPictureTag = $"<img alt=\"Embedded QR Code\" src=\"data:image/{imgType.ToString().ToLower()};base64,{qrCodeImageAsBase64}\" style=\"width: 50%; height: 50%\"/>";
            string imageType = imgType.ToString().ToLower();
            return new Tuple<string, string>(qrCodeImageAsBase64, imageType);
        }

        private async Task GetSingleOrderTicketAsync(OrderV2 order, bool isCancelled, bool isReprint, List<PrintInfo> dataToPrint)
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
                    viewBag.Add("title", "Nueva orden de mesa");
                    viewBag.Add("extraData", order);
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
                    viewBag.Add("title", "Nueva orden de reserva");
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
                        viewBag.Add("qrCode", qrData.Item1);
                        viewBag.Add("qrCodeImgType", qrData.Item2);
                    }
                    viewBag.Add("title", "Nuevo TakeAway");
                    info.BeforeAt = Utils.BeforeAt(now, PRINT_MINUTES_ORDER_TA);
                    info.Template = PrintTemplates.NEW_TAKEAWAY_ORDER;
                    break;
            }
            info.Content = viewBag;
            dataToPrint.Add(info);
        }

        private async Task GetUnifiedOrderTicketAsync(List<OrderV2> orders, bool isCancelled, bool isReprint, List<PrintInfo> dataToPrint)
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
            await GetSingleOrderTicketAsync(orderUnified, isCancelled, isCancelled, dataToPrint);
        }
        #endregion
    }
}
