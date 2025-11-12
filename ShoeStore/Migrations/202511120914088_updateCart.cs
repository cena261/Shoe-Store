namespace ShoeStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateCart : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Cart", "AddedAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Cart", "AddedAt", c => c.DateTime(nullable: false));
        }
    }
}
