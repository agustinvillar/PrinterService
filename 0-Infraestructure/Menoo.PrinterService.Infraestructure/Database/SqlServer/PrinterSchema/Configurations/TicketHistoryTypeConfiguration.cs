using System.Data.Entity.ModelConfiguration;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Configurations
{
    public class TicketHistoryTypeConfiguration : EntityTypeConfiguration<Entities.TicketHistory>
    {
        public TicketHistoryTypeConfiguration()
        {
            HasKey(d => d.Id);
            Property(d => d.Id).HasColumnOrder(0).IsRequired();
            Property(d => d.PrintEvent).HasColumnOrder(1).IsRequired();
            Property(d => d.Copies).HasColumnOrder(2).IsRequired();
            Property(d => d.PrinterName).HasColumnOrder(3).IsRequired();
            Property(d => d.TicketImage).HasColumnOrder(4).IsRequired();
            Property(d => d.StoreName).HasColumnOrder(5).IsRequired();
            Property(d => d.StoreId).HasColumnOrder(6).IsRequired();
            Property(d => d.CreatedAt).HasColumnOrder(7).IsRequired();
        }
    }
}
