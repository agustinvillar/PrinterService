using System.Data.Entity.ModelConfiguration;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Configurations
{
    public class PrinterConfigurationTypeConfiguration : EntityTypeConfiguration<Entities.PrinterConfiguration>
    {
        public PrinterConfigurationTypeConfiguration()
        {
            HasKey(d => d.Id);
            Property(d => d.Id)
                .HasColumnOrder(0)
                .IsRequired();
            Property(d => d.StoreId)
                .HasColumnOrder(1)
                .IsRequired();
            Property(d => d.Name)
                .HasColumnOrder(2)
                .HasMaxLength(25)
                .IsRequired();
            Property(d => d.AllowLogo)
                .HasColumnOrder(3)
                .IsRequired();
            Property(d => d.AllowPrintQR)
                .HasColumnOrder(4)
                .IsRequired();
            Property(d => d.Copies)
                .HasColumnOrder(5)
                .IsRequired();
            Property(d => d.IsHtml)
                .HasColumnOrder(6)
                .IsRequired();
            Property(d => d.Printer)
                .HasColumnOrder(7)
                .HasMaxLength(25)
                .IsRequired();
            Property(d => d.PrintEvents)
                .HasColumnOrder(8)
                .HasMaxLength(25)
                .IsRequired();
            Ignore(d => d.PrintEventsId);
            Property(d => d.CreatedAt).HasColumnOrder(9).IsRequired();
            Property(d => d.UpdatedAt).HasColumnOrder(10).IsOptional();
        }
    }
}
