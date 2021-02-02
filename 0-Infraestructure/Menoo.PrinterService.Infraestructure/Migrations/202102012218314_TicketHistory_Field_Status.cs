namespace Menoo.PrinterService.Infraestructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TicketHistory_Field_Status : DbMigration
    {
        public override void Up()
        {
            AddColumn("printer.TicketHistory", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("printer.TicketHistory", "Status");
        }
    }
}
