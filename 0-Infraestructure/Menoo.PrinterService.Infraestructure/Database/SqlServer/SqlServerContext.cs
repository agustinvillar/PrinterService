using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer
{
    public class SqlServerContext : DbContext
    {
        public DbSet<Entities.TicketHistory> TicketHistory { get; set; }

        public DbSet<Entities.TicketHistory> TicketHistorySettings { get; set; }

        internal SqlServerContext() 
            : base(GlobalConfig.ConfigurationManager.GetSetting("Microsoft.SQLServer.Print.ConnectionString")) 
        {
        }
        
        internal SqlServerContext(DbConnection dbConnection) 
            : base(dbConnection, true) 
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("printer");
            modelBuilder.Configurations.AddFromAssembly(typeof(SqlServerContext).Assembly);
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }

        public override async Task<int> SaveChangesAsync()
        {
            try
            {
                return await base.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }
    }
}
