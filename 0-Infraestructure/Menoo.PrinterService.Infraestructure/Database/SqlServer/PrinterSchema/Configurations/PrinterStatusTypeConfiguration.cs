using System.Data.Entity.ModelConfiguration;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Configurations
{
    public class PrinterStatusTypeConfiguration : EntityTypeConfiguration<Entities.PrinterStatus>
    {
        public PrinterStatusTypeConfiguration()
        {
            HasKey(d => d.Id);
            Property(d => d.Id)
                .HasColumnOrder(0)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)
                .IsRequired();
            Property(d => d.Name)
                .HasColumnOrder(1)
                .HasMaxLength(25)
                .IsRequired();
        }
    }
}
