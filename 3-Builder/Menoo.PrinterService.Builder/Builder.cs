using Menoo.Printer.Builder.Orders;
using Menoo.Printer.Builder.Orders.Extensions;
using Menoo.Printer.Builder.Orders.Repository;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Exceptions;
using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Menoo.PrinterService.Infraestructure.Repository;
using Menoo.PrinterService.Infraestructure.Services;
using Rebus.Activation;
using Rebus.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;

namespace Menoo.PrinterService.Builder
{
    public partial class Builder : ServiceBase, ISubscriptionService
    {
        private readonly ItemRepository _itemRepository;

        private readonly TicketRepository _ticketRepository;

        private readonly EventLog _generalWriter;

        private readonly BuiltinHandlerActivator _adapter;

        private Timer _timer;

        private readonly string _queueName;

        private readonly string _queueConnectionString;

        private readonly int _queueDelay;

        private long _tickCounter;

        public Builder()
        {
            InitializeService();
            _adapter = new BuiltinHandlerActivator();
            _queueName = GlobalConfig.ConfigurationManager.GetSetting("queueName");
            _queueConnectionString = GlobalConfig.ConfigurationManager.GetSetting("queueConnectionString");
            _queueDelay = int.Parse(GlobalConfig.ConfigurationManager.GetSetting("queueDelay"));
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("builder");
            _ticketRepository = GlobalConfig.DependencyResolver.Resolve<TicketRepository>();
            _itemRepository = GlobalConfig.DependencyResolver.Resolve<ItemRepository>();
        }

        public async Task RecieveAsync(PrintMessage data, Dictionary<string, string> extras = null)
        {
            var builders = GlobalConfig.DependencyResolver.ResolveAll<ITicketBuilder>();
            foreach (var builder in builders)
            {
                if (builder.ToString() == data.Builder)
                {
                    string type = !string.IsNullOrEmpty(data.SubTypeDocument) ? $"{data.TypeDocument}-{data.SubTypeDocument}" : $"{data.TypeDocument}";
                    string documentsId = data.DocumentsId != null && data.DocumentsId.Count > 0 ? string.Join(",", data.DocumentsId) : data.DocumentId;
                    _generalWriter.WriteEntry($"Builder::RecieveAsync(). Activando el builder de: {builder}", EventLogEntryType.Information);
                    _generalWriter.WriteEntry(
                        $"{builder}::BuildAsync(). Nuevo ticket de impresión recibido. {Environment.NewLine}" +
                        $"Evento: {data.PrintEvent}{Environment.NewLine}" +
                        $"Tipo: {type}{Environment.NewLine}" +
                        $"Id colleción printEvents: {documentsId}{Environment.NewLine}", EventLogEntryType.Information);
                    try
                    {
                        await Task.Delay(_queueDelay);
                        var dataToPrint = await builder.BuildAsync(extras["id"], data);
                        await SendToFirebaseAsync(extras["id"], dataToPrint, data.TypeDocument, data.PrintEvent);
                    }
                    catch (UnifiedSectorException sectorException) 
                    {
                        _generalWriter.WriteEntry(
                                $"{builder}::RecieveAsync(). {sectorException.Message}. {Environment.NewLine}" +
                                $"Evento: {data.PrintEvent}{Environment.NewLine}" +
                                $"Tipo: {type}{Environment.NewLine}" +
                                $"PrintEventId: {documentsId}{Environment.NewLine}", EventLogEntryType.Error);
                    }
                    catch (BadImageFormatException imageException)
                    {
                        _generalWriter.WriteEntry(
                            $"{builder}::RecieveAsync(). {imageException.Message}. {Environment.NewLine}" +
                            $"Evento: {data.PrintEvent}{Environment.NewLine}" +
                            $"Tipo: {type}{Environment.NewLine}" +
                            $"PrintEventId: {documentsId}{Environment.NewLine}", EventLogEntryType.Error);
                    }
                    catch (Exception e)
                    {
                        _generalWriter.WriteEntry(
                            $"{builder}::RecieveAsync(). NO se imprimió el ticket de impresión recibido. {Environment.NewLine}" +
                            $"Evento: {data.PrintEvent}{Environment.NewLine}" +
                            $"Tipo: {type}{Environment.NewLine}" +
                            $"PrintEventId: {documentsId}{Environment.NewLine}" +
                            $"Excepción: {e}", EventLogEntryType.Error);
                    }
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            Debugger.Launch();
            _generalWriter.WriteEntry("Builder::OnStart(). Iniciando servicio.", EventLogEntryType.Information);
            ConfigureWorker();
            _timer.Start();
        }

        protected override void OnShutdown()
        {
            if (_adapter != null)
            {
                _adapter.Dispose();
            }
            _generalWriter.WriteEntry("Builder::OnShutdown(). Apagando servicio.", EventLogEntryType.Warning);
            base.OnShutdown();
        }

        protected override void OnStop()
        {
            _timer.Stop();
            if (_adapter != null)
            {
                _adapter.Dispose();
            }
            _generalWriter.WriteEntry("Builder::OnStop(). Deteniendo servicio.", EventLogEntryType.Warning);
        }

        #region private methods
        private void ConfigureTimer()
        {
            double.TryParse(GlobalConfig.ConfigurationManager.GetSetting("serviceInternal"), out double interval);
            if (interval == 0d)
            {
                interval = 10000d;
            }
            _tickCounter = 0L;
            _timer = new Timer(interval);
            _timer.Elapsed += ServiceTimer_Tick;
        }

        private void ConfigureWorker()
        {
            _adapter.Handle<PrintMessage>(async (bus, context, message) =>
            {
                await RecieveAsync(message, context.Headers);
            });
            Configure.With(_adapter)
                .Logging(l => l.Serilog())
                .Transport(t => t.UseRabbitMq(_queueConnectionString, _queueName))
                .Options(o => o.SetMaxParallelism(1))
                .Options(o => o.SetNumberOfWorkers(1))
                .Start();
            _adapter.Bus.Subscribe<PrintMessage>().GetAwaiter().GetResult();
        }

        private void InitializeService()
        {
            try
            {
                AutoLog = false;
                CanHandlePowerEvent = true;
                CanPauseAndContinue = false;
                CanShutdown = true;
                ServiceName = GlobalConfig.ConfigurationManager.GetSetting("serviceBuilderName");
                ConfigureTimer();
            }
            catch (Exception ex)
            {
                EventLog eventLog = new EventLog
                {
                    Source = GlobalConfig.ConfigurationManager.GetSetting("defaultLog")
                };
                eventLog.WriteEntry("Builder::InitializeService()" + Environment.NewLine + ex, EventLogEntryType.Error);
                eventLog.Dispose();
            }
        }

        private async Task SendToFirebaseAsync(string id, PrintInfo data, string typeDocument, string printEvent)
        {
            if (data == null || data.Content.Count == 0)
            {
                return;
            }
            var sectors = new List<PrintSettings>();
            if (typeDocument != PrintTypes.ORDER)
            {
                sectors = data.Store.GetPrintSettings(printEvent);
                await PrintAsync(id, data, printEvent, sectors);
            }
            else 
            {
                var orderData = (Order)data.Content["orderData"];
                string clientName = orderData.UserName;
                switch (printEvent)
                {
                    case PrintEvents.NEW_TAKE_AWAY:
                        sectors = data.Store.GetPrintSettings(PrintEvents.NEW_TAKE_AWAY);
                        await PrintAsync(id, data, printEvent, sectors, true);
                        var items = ItemExtensions.GetPrintSectorByItems(orderData.Items, _itemRepository);
                        await PrintAsync(id, data, clientName, items);
                        break;
                    case PrintEvents.NEW_TABLE_ORDER:
                        var unifiedSector = data.Store.SectorUnifiedTicket();
                        if (unifiedSector == null)
                        {
                            _generalWriter.WriteEntry($"Builder::PrintAsync(). El restaurante {data.Store.Name} no tiene asignado el sector unificado. {Environment.NewLine}" +
                                $"Id del restaurante: {data.Store.Id}{Environment.NewLine}" +
                                $"Evento: {PrintEvents.NEW_TABLE_ORDER}{Environment.NewLine}" +
                                $"Tipo: {SubOrderPrintTypes.ORDER_TABLE}{Environment.NewLine}" +
                                $"PrintEventId: {id}{Environment.NewLine}", EventLogEntryType.Warning);
                        }
                        else
                        {
                            sectors.Add(unifiedSector);
                            await PrintAsync(id, data, PrintEvents.NEW_TABLE_ORDER, sectors);
                        }
                        var sectorsByItems = ItemExtensions.GetPrintSectorByItems(orderData.Items, _itemRepository);
                        await PrintAsync(id, data, clientName, sectorsByItems);
                        break;
                    case PrintEvents.REPRINT_ORDER:
                        printEvent = orderData.OrderType == SubOrderPrintTypes.ORDER_TA ? PrintEvents.NEW_TAKE_AWAY : PrintEvents.NEW_TABLE_ORDER;
                        bool isTakeAway = orderData.OrderType == SubOrderPrintTypes.ORDER_TA;
                        sectors = data.Store.GetPrintSettings(printEvent);
                        await PrintAsync(id, data, printEvent, sectors, isTakeAway);
                        break;
                    case PrintEvents.ORDER_CANCELLED:
                        sectors = data.Store.GetPrintSettings(PrintEvents.ORDER_CANCELLED);
                        await PrintAsync(id, data, printEvent, sectors);
                        break;
                }
            }
        }

        private async Task PrintAsync(string id, PrintInfo data, string printEvent, List<PrintSettings> sectors, bool isTakeAway = false) 
        {
            foreach (var sector in sectors)
            {
                if (isTakeAway) 
                {
                    if (!data.Content.ContainsKey("printQR"))
                    {
                        data.Content.Add("printQR", sector.PrintQR);
                    }
                    else 
                    {
                        data.Content["printQR"] = sector.PrintQR;
                    }
                }
                if (!data.Content.ContainsKey("allowLogo"))
                {
                    data.Content.Add("allowLogo", sector.AllowLogo);
                }
                else
                {
                    data.Content["allowLogo"] = sector.AllowLogo;
                }
                IFormaterService formatterService = FormaterFactory.Resolve(sector.IsHTML.GetValueOrDefault(), data.Content, data.Template);
                string ticket = formatterService.Create();
                var printDocument = new Ticket
                {
                    TicketType = printEvent,
                    PrintBefore = data.BeforeAt,
                    Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                    Copies = sector.Copies,
                    PrinterName = sector.Printer,
                    TicketImage = ticket,
                    StoreName = data.Store.Name,
                    StoreId = data.Store.Id
                };
                _generalWriter.WriteEntry($"Builder::SendToFirebaseAsync(). Enviando a imprimir el ticket con la siguiente información. {Environment.NewLine}" +
                    $"Detalles:{Environment.NewLine}" +
                    $"Nombre de la impresora: {sector.Printer}{Environment.NewLine}" +
                    $"Sector de impresión: {sector.Name}{Environment.NewLine}" +
                    $"Hora de impresión: {printDocument.PrintBefore}{Environment.NewLine}" +
                    $"Restaurante: {printDocument.StoreName}{Environment.NewLine}" +
                    $"URL Ticket: {ticket} {Environment.NewLine}" +
                    $"Id en colección printEvents: {id}", EventLogEntryType.Information);
                await _ticketRepository.SaveAsync(printDocument);
                await _ticketRepository.SetTicketImageAsync(id, ticket);
            }
        }

        private async Task PrintAsync(string id, PrintInfo extraData, string clientName, List<SectorItem> sectorsByItems)
        {
            var orderData = (Order)extraData.Content["orderData"];
            foreach (var item in sectorsByItems)
            {
                var itemData = await _itemRepository.GetById<ItemOrder>(item.ItemId, "items");
                var orderItem = orderData.Items.FirstOrDefault(f => f.Id == itemData.Id);
                var viewData = new Dictionary<string, object>() 
                {
                    { "title", extraData.Content["title"] },
                    { "orderNumber", extraData.Content["orderNumber"] },
                    { "clientName", clientName},
                    { "item", orderItem }
                };
                foreach (var sector in item.Sectors.FindAll(f => f.AllowPrinting))
                {
                    if (!viewData.ContainsKey("allowLogo"))
                    {
                        viewData.Add("allowLogo", sector.AllowLogo);
                    }
                    else
                    {
                        viewData["allowLogo"] = sector.AllowLogo;
                    }
                    IFormaterService formatterService = FormaterFactory.Resolve(sector.IsHTML.GetValueOrDefault(), viewData, PrintTemplates.NEW_ITEM_ORDER);
                    string ticket = formatterService.Create();
                    var printDocument = new Ticket
                    {
                        TicketType = PrintEvents.NEW_TABLE_ORDER_ITEM,
                        PrintBefore = extraData.BeforeAt,
                        Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                        Copies = sector.Copies,
                        PrinterName = sector.Printer,
                        TicketImage = ticket,
                        StoreName = extraData.Store.Name,
                        StoreId = extraData.Store.Id
                    };
                    _generalWriter.WriteEntry($"Builder::SendToFirebaseAsync(). Enviando a imprimir el ticket con la siguiente información. {Environment.NewLine}" +
                        $"Detalles:{Environment.NewLine}" +
                        $"Nombre de la impresora: {sector.Printer}{Environment.NewLine}" +
                        $"Sector de impresión: {sector.Name}{Environment.NewLine}" +
                        $"Hora de impresión: {printDocument.PrintBefore}{Environment.NewLine}" +
                        $"Restaurante: {printDocument.StoreName}{Environment.NewLine}" +
                        $"Id en colección printEvents: {id}", EventLogEntryType.Information);
                    await _ticketRepository.SaveAsync(printDocument);
                    await _ticketRepository.SetTicketImageAsync(id, ticket);
                }
            }
        }

        private void ServiceTimer_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                long memSize = 0L;
                _tickCounter++;
                if (_tickCounter % 60L == 0L)
                {
                    memSize = GC.GetTotalMemory(true);
                    _generalWriter.WriteEntry($"Builder::ServiceTimer_Tick(). Se ha liberado {memSize / 1024 }.", EventLogEntryType.Warning);
                }
                if (_tickCounter % 3600L == 0L)
                {
                    _timer.Stop();
                    //Proceso de mantenimiento de .NET.
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.WaitForFullGCComplete();
                    memSize = GC.GetTotalMemory(true);
                    _generalWriter.WriteEntry($"Builder::ServiceTimer_Tick(). Se ha liberado {memSize / 1024 }.", EventLogEntryType.Warning);
                    _timer.Start();
                }
            }
            catch (Exception ex)
            {
                _generalWriter.WriteEntry("Builder::ServiceTimer_Tick()" + Environment.NewLine + ex, EventLogEntryType.Error);
            }
        }
        #endregion
    }
}
