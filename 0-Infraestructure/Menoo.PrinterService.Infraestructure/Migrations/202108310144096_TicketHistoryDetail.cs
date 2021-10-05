namespace Menoo.PrinterService.Infraestructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TicketHistoryDetail : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "printer.TicketHistoryDetail",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EntityId = c.String(nullable: false),
                        TicketHistoryId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("printer.TicketHistory", t => t.TicketHistoryId, cascadeDelete: true)
                .Index(t => t.TicketHistoryId);
            
            AddColumn("printer.TicketHistory", "DocumentPrinted", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("printer.TicketHistoryDetail", "TicketHistoryId", "printer.TicketHistory");
            DropIndex("printer.TicketHistoryDetail", new[] { "TicketHistoryId" });
            DropColumn("printer.TicketHistory", "DocumentPrinted");
            DropTable("printer.TicketHistoryDetail");
        }
    }
}
