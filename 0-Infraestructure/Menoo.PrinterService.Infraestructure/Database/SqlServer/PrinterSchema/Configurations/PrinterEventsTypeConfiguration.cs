using System.Data.Entity.ModelConfiguration;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Configurations
{
    public class PrinterEventsTypeConfiguration : EntityTypeConfiguration<Entities.PrinterEvents>
    {
        public PrinterEventsTypeConfiguration()
        {
            HasKey(d => d.Id);
            Property(d => d.Id)
                .HasColumnOrder(0)
                .IsRequired();
            Property(d => d.DisplayName)
                .HasColumnOrder(1)
                .HasMaxLength(25)
                .IsRequired();
            Property(d => d.Value)
                .HasColumnOrder(2)
                .IsRequired();
        }
    }
}
