using System.Data.Entity.ModelConfiguration;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Configurations
{
    public class PrinterLogTypeConfiguration : EntityTypeConfiguration<Entities.PrinterLog>
    {
        public PrinterLogTypeConfiguration()
        {
            HasKey(d => d.Id);
            Property(d => d.Id)
                .HasColumnOrder(0)
                .IsRequired();
            Property(d => d.StoreId)
                .HasColumnOrder(1)
                .IsRequired();
            Property(d => d.PrintEvent)
                .HasColumnOrder(2)
                .HasMaxLength(25)
                .IsRequired();
            Property(d => d.Details)
                .HasColumnOrder(3)
                .IsRequired();
            Property(d => d.CreatedAt)
                .HasColumnOrder(4)
                .IsRequired();
            this.HasRequired(d => d.Status)
                .WithMany(d => d.EntriesLog);
        }
    }
}
