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
            Property(d => d.DayCreatedAt).HasColumnOrder(3).IsRequired();
            Property(d => d.CreatedAt).HasColumnOrder(4).IsRequired();
            Property(d => d.DocumentPrinted).HasColumnOrder(5).IsOptional();

            HasMany(d => d.TicketHistoryDetail)
                .WithRequired(r => r.TicketHistory)
                .HasForeignKey(r => r.TicketHistoryId);
        }
    }
}
