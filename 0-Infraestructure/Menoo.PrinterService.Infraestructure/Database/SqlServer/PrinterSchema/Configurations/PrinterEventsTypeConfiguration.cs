using System.Data.Entity.ModelConfiguration;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Configurations
{
    public class PrinterEventsTypeConfiguration : EntityTypeConfiguration<Entities.PrinterEvents>
    {
        public PrinterEventsTypeConfiguration()
        {
            HasKey(d => d.Id);
            Property(d => d.Id).HasColumnOrder(0).IsRequired();
            Property(d => d.Name).HasColumnOrder(1).IsRequired();
        }
    }
}
