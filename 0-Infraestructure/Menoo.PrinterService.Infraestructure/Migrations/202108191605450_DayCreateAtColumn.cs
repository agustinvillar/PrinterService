namespace Menoo.PrinterService.Infraestructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DayCreateAtColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("printer.TicketHistory", "DayCreatedAt", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("printer.TicketHistory", "DayCreatedAt");
        }
    }
}
