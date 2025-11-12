namespace ShoeStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateOrders : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "OrderDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "OrderDate", c => c.DateTime(nullable: false));
        }
    }
}
