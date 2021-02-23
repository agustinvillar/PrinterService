using System.Data.Entity.ModelConfiguration;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.Configurations
{
    public class TicketHistorySettingsTypeConfiguration : EntityTypeConfiguration<Entities.TicketHistorySettings>
    {
        public TicketHistorySettingsTypeConfiguration()
        {
            HasKey(d => d.Id);

            Property(d => d.Name).HasColumnOrder(0).IsRequired();
            Property(d => d.Value).HasColumnOrder(1).IsRequired();

            HasRequired(d => d.TicketHistory)
                    .WithMany(d => d.TicketHistorySettings)
                    .HasForeignKey(c => c.TicketHistoryId);
        }
    }
}
