namespace Menoo.PrinterService.Infraestructure.Migrations
{
    using System;
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
                        StoreId = c.String(nullable: false),
                        Name = c.String(nullable: false, maxLength: 25),
                        AllowLogo = c.Boolean(nullable: false),
                        AllowPrintQR = c.Boolean(nullable: false),
                        Copies = c.Int(nullable: false),
                        IsHtml = c.Boolean(nullable: false),
                        Printer = c.String(nullable: false, maxLength: 25),
                        PrintEvents = c.String(nullable: false),
                        Queue = c.String(nullable: false, maxLength: 25),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "printer.PrinterEvents",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DisplayName = c.String(nullable: false, maxLength: 25),
                        Value = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "printer.PrinterLog",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StoreId = c.String(nullable: false),
                        PrintEvent = c.String(nullable: false, maxLength: 25),
                        Details = c.String(nullable: false),
                        Status_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("printer.PrinterStatus", t => t.Status_Id, cascadeDelete: true)
                .Index(t => t.Status_Id);
            
            CreateTable(
                "printer.PrinterStatus",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 25),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "printer.TicketHistory",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StoreId = c.String(nullable: false),
                        StoreName = c.String(nullable: false, maxLength: 50),
                        SectorName = c.String(nullable: false, maxLength: 50),
                        PrintEvent = c.String(nullable: false, maxLength: 25),
                        PrinterName = c.String(nullable: false, maxLength: 25),
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
            DropForeignKey("printer.PrinterLog", "Status_Id", "printer.PrinterStatus");
            DropIndex("printer.PrinterLog", new[] { "Status_Id" });
            DropTable("printer.TicketHistory");
            DropTable("printer.PrinterStatus");
            DropTable("printer.PrinterLog");
            DropTable("printer.PrinterEvents");
            DropTable("printer.PrinterConfiguration");
        }
    }
}
