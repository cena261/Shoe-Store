namespace ShoeStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateOrderItems : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OrderItems", "Subtotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OrderItems", "Subtotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
