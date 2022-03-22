using System.Data.Entity.ModelConfiguration;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Configurations
{
    public class TicketHistoryTypeConfiguration : EntityTypeConfiguration<Entities.TicketHistory>
    {
        public TicketHistoryTypeConfiguration()
        {
            HasKey(d => d.Id);
            Property(d => d.Id)
                .HasColumnOrder(0)
                .IsRequired();
            Property(d => d.StoreId)
                .HasColumnOrder(1)
                .IsRequired();
            Property(d => d.StoreName)
                .HasColumnOrder(2)
                .HasMaxLength(50)
                .IsRequired();
            Property(d => d.SectorName)
                .HasColumnOrder(3)
                .HasMaxLength(50)
                .IsRequired();
            Property(d => d.PrintEvent)
                .HasColumnOrder(4)
                .HasMaxLength(25)
                .IsRequired();
            Property(d => d.PrinterName)
                .HasColumnOrder(5)
                .HasMaxLength(25)
                .IsRequired();
            Property(d => d.Copies)
                .HasColumnOrder(6)
                .IsRequired();
            Property(d => d.TicketImage)
                .HasColumnOrder(7)
                .IsRequired();
            Property(d => d.IsPrinted)
                .HasColumnOrder(8)
                .IsRequired();
            Property(d => d.IsReprinted)
                .HasColumnOrder(9)
                .IsRequired();
            Property(d => d.CreatedAt)
                .HasColumnOrder(10)
                .IsRequired();
            Property(d => d.UpdatedAt)
                .HasColumnOrder(11)
                .IsOptional();
        }
    }
}
