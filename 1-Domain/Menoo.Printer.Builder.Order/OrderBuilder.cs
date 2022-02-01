using Menoo.Backend.Integrations.Constants;
using Menoo.Backend.Integrations.Messages;
using Menoo.Printer.Builder.Orders.Repository;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Repository;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OrderTypes = Menoo.PrinterService.Infraestructure.Constants.OrderTypes;

namespace Menoo.Printer.Builder.Orders
{
    [Handler]
    public class OrderBuilder : ITicketBuilder
    {
        private const int PRINT_MINUTES_ORDER_TABLE = 60;

        private const int PRINT_MINUTES_ORDER_TA = 30;

        private const int PRINT_MINUTES_ORDER_BOOKING = 60;

        private const int PRINT_MINUTES_ORDER_REPRINT = 120;

        private readonly int _queueDelay;

        private readonly BookingRepository _bookingRepository;

        private readonly OrderRepository _orderRepository;

        private readonly PaymentRepository _paymentRepository;

        private readonly StoreRepository _storeRepository;

        private readonly MenooContext _menooContext;

        public OrderBuilder(
            MenooContext menooContext,
            StoreRepository storeRepository,
            BookingRepository bookingRepository,
            OrderRepository orderRepository,
            PaymentRepository paymentRepository)
        {
            _queueDelay = int.Parse(GlobalConfig.ConfigurationManager.GetSetting("queueDelay"));
            _menooContext = menooContext;
            _storeRepository = storeRepository;
            _bookingRepository = bookingRepository;
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<PrintInfo> BuildAsync(PrintMessage message)
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
                var orders = new List<Order>();
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
        private Tuple<string, string> GenerateOrderQR(Order order)
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

        private async Task<PrintInfo> GetSingleOrderTicketAsync(Order order, bool isCancelled, bool isReprint)
        {
            isCancelled = !isCancelled ? order.Status.ToLower().Contains("cancelado") || order.Status.ToLower().Contains("reembolsado") : isCancelled;
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
                    viewBag.Add("taTime", order.TakeAwayHour);
                    viewBag.Add("clientName", order.UserName);
                    viewBag.Add("orderNumber", order.OrderNumber);
                    viewBag.Add("isCancelled", isCancelled);
                    var qrData = GenerateOrderQR(order);
                    viewBag.Add("qrCode", $"data:image/{qrData.Item2};base64, {qrData.Item1}");
                    viewBag.Add("title", isCancelled ? "Takeaway cancelado" : "Nuevo TakeAway");
                    long paymentId = _menooContext.Payments.FirstOrDefault(f => f.EntityId == order.Id)?.PaymentId ?? 0;
                    if (paymentId > 0)
                    {
                        Thread.Sleep(_queueDelay);
                        var paymentData = await _paymentRepository.GetPaymentByIdAsync(paymentId);
                        var takeAwayOpening = paymentData.TaOpening;
                        viewBag.Add("paymentMethod", $"{paymentData.PaymentType}: {paymentData.PaymentMethod}");
                        if (paymentData.Surcharge != null && paymentData.Surcharge > 0) 
                        {
                            viewBag.Add("sucharge", $"${paymentData.Surcharge.GetValueOrDefault().ToString("N2", CultureInfo.CreateSpecificCulture("en-US"))}");
                        }
                        if (takeAwayOpening.DiscountByCouponAmount != null && takeAwayOpening.OfferCoupon != null) 
                        {
                            viewBag.Add("couponName", "Descuento Cupón " + takeAwayOpening.OfferCoupon.Code);
                            viewBag.Add("couponAmount", $"-${takeAwayOpening.DiscountByCouponAmount.GetValueOrDefault().ToString("N2", CultureInfo.CreateSpecificCulture("en-US"))}");
                        }
                        if (paymentData.Discounts != null && paymentData.Discounts.Count > 0) 
                        {
                            viewBag.Add("discounts", paymentData.Discounts);
                        }
                        var subtotal = Convert.ToDecimal(takeAwayOpening.SubTotal).ToString("N2", CultureInfo.CreateSpecificCulture("en-US"));
                        var total = Convert.ToDecimal(takeAwayOpening.TotalToPay).ToString("N2", CultureInfo.CreateSpecificCulture("en-US"));
                        viewBag.Add("subtotal", $"${subtotal}");
                        viewBag.Add("total", $"${total}");
                    }
                    info.BeforeAt = Utils.BeforeAt(now, PRINT_MINUTES_ORDER_TA);
                    info.Template = PrintTemplates.NEW_TAKEAWAY_ORDER;
                    break;
            }
            viewBag.Add("orderData", order);
            info.Content = viewBag;
            return info;
        }

        private async Task<PrintInfo> GetUnifiedOrderTicketAsync(List<Order> orders, bool isCancelled, bool isReprint)
        {
            var orderUnified = new Order();
            var items = new List<ItemOrder>();
            var ordersByStore = orders.GroupBy(g => g.Store.Id).FirstOrDefault();
            var ordersByOrderNumber = orders.GroupBy(g => g.OrderNumber).FirstOrDefault();
            var ordersByAddress = orders.GroupBy(g => g.Address).FirstOrDefault();
            var ordersByUsername = orders.GroupBy(g => g.UserName).FirstOrDefault();
            var ordersStatus = orders.GroupBy(g => g.Status).FirstOrDefault();
            orderUnified.OrderNumber = ordersByOrderNumber.Key;
            orderUnified.Address = ordersByAddress.Key;
            orderUnified.UserName = ordersByUsername.Key;
            orderUnified.Store = ordersByStore.ToList().FirstOrDefault().Store;
            orderUnified.OrderType = ordersByStore.ToList().FirstOrDefault().OrderType;
            orderUnified.Status = ordersStatus.Key;
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
