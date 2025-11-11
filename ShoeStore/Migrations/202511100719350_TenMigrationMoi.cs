namespace ShoeStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TenMigrationMoi : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProductVariants", "UQ_Product_Size_Color");
            AlterColumn("dbo.ProductVariants", "ColorName", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.ProductVariants", "HexCode", c => c.String(maxLength: 50));
            AlterColumn("dbo.Products", "Rating", c => c.Decimal(precision: 3, scale: 2));
            CreateIndex("dbo.ProductVariants", new[] { "ProductID", "SizeValue", "ColorName" }, unique: true, name: "UQ_Product_Size_Color");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ProductVariants", "UQ_Product_Size_Color");
            AlterColumn("dbo.Products", "Rating", c => c.Decimal(nullable: false, precision: 3, scale: 2));
            AlterColumn("dbo.ProductVariants", "HexCode", c => c.String(maxLength: 7));
            AlterColumn("dbo.ProductVariants", "ColorName", c => c.String(nullable: false, maxLength: 50));
            CreateIndex("dbo.ProductVariants", new[] { "ProductID", "SizeValue", "ColorName" }, unique: true, name: "UQ_Product_Size_Color");
        }
    }
}
