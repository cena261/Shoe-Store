namespace ShoeStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSKU : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProductVariants", new[] { "SKU" });
            AddColumn("dbo.ProductVariants", "StyleColor", c => c.String(maxLength: 50));
            AlterColumn("dbo.ProductVariants", "SKU", c => c.String(nullable: false, maxLength: 250));
            CreateIndex("dbo.ProductVariants", "SKU", unique: true);
            DropColumn("dbo.ProductVariants", "HexCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductVariants", "HexCode", c => c.String(maxLength: 50));
            DropIndex("dbo.ProductVariants", new[] { "SKU" });
            AlterColumn("dbo.ProductVariants", "SKU", c => c.String(nullable: false, maxLength: 64));
            DropColumn("dbo.ProductVariants", "StyleColor");
            CreateIndex("dbo.ProductVariants", "SKU", unique: true);
        }
    }
}
