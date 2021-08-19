using System.Data.Entity.ModelConfiguration;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Configurations
{
    public class TicketHistoryTypeConfiguration : EntityTypeConfiguration<Entities.TicketHistory>
    {
        public TicketHistoryTypeConfiguration()
        {
            HasKey(d => d.Id);

            Property(d => d.Id).HasColumnOrder(0).IsRequired();
            Property(d => d.PrintEvent).HasColumnOrder(2).IsRequired();
            Property(d => d.CreatedAt).HasColumnOrder(3).IsRequired();
        }
    }
}
