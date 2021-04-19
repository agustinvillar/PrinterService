using Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema.Entities;
using System.Data.Common;
using System.Data.Entity;
using Payment = Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema.Entities.Payment;
using Store = Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema.Entities.Store;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema
{
    public class MenooContext : DbContext
    {
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<Incremental> Incrementals { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<SmsVerification> SmsVerifications { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoicesItems { get; set; }
        public DbSet<ContractBase> ContractBases { get; set; }
        public DbSet<ContractType> ContractsTypes { get; set; }
        public DbSet<ContractPeriod> ContractsPeriods { get; set; }
        public DbSet<AccountState> AccountStates { get; set; }
        public DbSet<PaymentRenglon> Renglones { get; set; }

        static MenooContext()
        {
            System.Data.Entity.Database.SetInitializer<MenooContext>(null);
        }

        public MenooContext()
            : base("name=Microsoft.SQLServer.Menoo.ConnectionString")
        {
        }

        public MenooContext(DbConnection dbConnection)
            : base(dbConnection, true)
        {
        }
    }
}