namespace Menoo.PrinterService.Infraestructure.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class PrinterSchema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "printer.PrinterConfiguration",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    StoreId = c.Guid(nullable: false),
                    Name = c.String(nullable: false),
                    AllowLogo = c.Boolean(nullable: false),
                    Copies = c.Int(nullable: false),
                    IsHtml = c.Boolean(nullable: false),
                    Printer = c.String(nullable: false),
                    PrintEvents = c.String(nullable: false),
                    CreatedAt = c.DateTime(nullable: false),
                    UpdatedAt = c.DateTime(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "printer.PrinterEvents",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "printer.TicketHistory",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    StoreId = c.String(nullable: false),
                    StoreName = c.String(nullable: false),
                    SectorName = c.String(nullable: false),
                    PrintEvent = c.String(nullable: false),
                    PrinterName = c.String(nullable: false),
                    Copies = c.Int(nullable: false),
                    TicketImage = c.String(nullable: false),
                    IsPrinted = c.Boolean(nullable: false),
                    IsReprinted = c.Boolean(nullable: false),
                    CreatedAt = c.DateTime(nullable: false),
                    UpdatedAt = c.DateTime(),
                })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropTable("printer.TicketHistory");
            DropTable("printer.PrinterEvents");
            DropTable("printer.PrinterConfiguration");
        }
    }
}
