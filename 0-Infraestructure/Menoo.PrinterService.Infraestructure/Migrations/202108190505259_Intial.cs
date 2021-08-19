namespace Menoo.PrinterService.Infraestructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Intial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "printer.TicketHistory",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PrintEvent = c.String(nullable: false),
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
