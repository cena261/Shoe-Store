namespace ShoeStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUniqueKeys : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ProductImages", "ImageURL", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.ProductImages", new[] { "ImageURL" });
        }
    }
}
