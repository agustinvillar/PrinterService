namespace Menoo.PrinterService.Infraestructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrinterDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "printer.TicketHistory",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ExternalId = c.String(nullable: true),
                        PrintEvent = c.String(nullable: false),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "printer.TicketHistorySettings",
                c => new
                    {
                        Name = c.String(nullable: false),
                        Value = c.String(nullable: false),
                        Id = c.Guid(nullable: false),
                        TicketHistoryId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("printer.TicketHistory", t => t.TicketHistoryId, cascadeDelete: true)
                .Index(t => t.TicketHistoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("printer.TicketHistorySettings", "TicketHistoryId", "printer.TicketHistory");
            DropIndex("printer.TicketHistorySettings", new[] { "TicketHistoryId" });
            DropTable("printer.TicketHistorySettings");
            DropTable("printer.TicketHistory");
        }
    }
}
