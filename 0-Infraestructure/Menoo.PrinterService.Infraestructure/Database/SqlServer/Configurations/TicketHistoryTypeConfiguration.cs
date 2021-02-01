using System.Data.Entity.ModelConfiguration;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.Configurations
{
    public class TicketHistoryTypeConfiguration : EntityTypeConfiguration<Entities.TicketHistory>
    {
        public TicketHistoryTypeConfiguration()
        {
            HasKey(d => d.Id);

            Property(d => d.Id).HasColumnOrder(0).IsRequired();
            Property(d => d.ExternalId).HasColumnOrder(1).IsRequired();
            Property(d => d.PrintEvent).HasColumnOrder(2).IsRequired();
            Property(d => d.CreatedAt).IsOptional();
            Property(d => d.UpdatedAt).IsOptional();

            HasMany(d => d.TicketHistorySettings)
                .WithRequired(r => r.TicketHistory)
                .HasForeignKey(r => r.TicketHistoryId);
        }
    }
}
