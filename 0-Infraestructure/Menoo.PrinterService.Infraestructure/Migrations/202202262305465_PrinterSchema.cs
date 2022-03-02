namespace Menoo.PrinterService.Infraestructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrinterSchema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "printer.TicketHistory",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PrintEvent = c.String(nullable: false),
                        Copies = c.Int(nullable: false),
                        PrinterName = c.String(nullable: false),
                        TicketImage = c.String(nullable: false),
                        StoreName = c.String(nullable: false),
                        StoreId = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("printer.TicketHistory");
        }
    }
}
