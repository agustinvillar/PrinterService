using System.Data.Entity.ModelConfiguration;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Configurations
{
    public class TicketHistoryDetailConfiguration : EntityTypeConfiguration<Entities.TicketHistoryDetail>
    {
        public TicketHistoryDetailConfiguration()
        {
            HasKey(d => d.Id);

            Property(d => d.Id).HasColumnOrder(0).IsRequired();
            Property(d => d.EntityId).HasColumnOrder(2).IsRequired();

            HasRequired(d => d.TicketHistory)
                    .WithMany(d => d.TicketHistoryDetail)
                    .HasForeignKey(c => c.TicketHistoryId);
        }
    }
}
