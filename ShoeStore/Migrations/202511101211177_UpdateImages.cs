namespace ShoeStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateImages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductImages", "StyleColor", c => c.String(maxLength: 50));
            AlterColumn("dbo.ProductImages", "DisplayOrder", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductImages", "DisplayOrder", c => c.Int(nullable: false));
            DropColumn("dbo.ProductImages", "StyleColor");
        }
    }
}
