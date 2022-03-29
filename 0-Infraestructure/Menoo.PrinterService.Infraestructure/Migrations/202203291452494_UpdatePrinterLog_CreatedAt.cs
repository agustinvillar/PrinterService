namespace Menoo.PrinterService.Infraestructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePrinterLog_CreatedAt : DbMigration
    {
        public override void Up()
        {
            AddColumn("printer.PrinterLog", "CreatedAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("printer.PrinterLog", "CreatedAt");
        }
    }
}
