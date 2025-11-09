namespace ShoeStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        AddressID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Phone = c.String(nullable: false, maxLength: 20),
                        TenDuong = c.String(nullable: false, maxLength: 100),
                        XaQuan = c.String(nullable: false, maxLength: 100),
                        TinhThanh = c.String(nullable: false, maxLength: 100),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AddressID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        userId = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 255),
                        PasswordHash = c.String(nullable: false, maxLength: 255),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Phone = c.String(maxLength: 20),
                        createdAt = c.DateTime(nullable: false),
                        lastLogin = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.userId)
                .Index(t => t.Email, unique: true);
            
            CreateTable(
                "dbo.Cart",
                c => new
                    {
                        CartID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        VariantID = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        AddedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CartID)
                .ForeignKey("dbo.ProductVariants", t => t.VariantID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => new { t.UserID, t.VariantID }, unique: true, name: "UQ_Cart_User_Variant");
            
            CreateTable(
                "dbo.ProductVariants",
                c => new
                    {
                        VariantID = c.Int(nullable: false, identity: true),
                        ProductID = c.Int(nullable: false),
                        SizeValue = c.String(nullable: false, maxLength: 10),
                        ColorName = c.String(nullable: false, maxLength: 50),
                        HexCode = c.String(maxLength: 7),
                        SKU = c.String(nullable: false, maxLength: 64),
                        StockQty = c.Int(nullable: false),
                        PriceOverride = c.Decimal(precision: 12, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.VariantID)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => new { t.ProductID, t.SizeValue, t.ColorName }, unique: true, name: "UQ_Product_Size_Color")
                .Index(t => t.SKU, unique: true);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductID = c.Int(nullable: false, identity: true),
                        ProductName = c.String(nullable: false, maxLength: 100),
                        Slug = c.String(nullable: false, maxLength: 150),
                        Description = c.String(),
                        Price = c.Decimal(nullable: false, precision: 12, scale: 2),
                        PromotionPrice = c.Decimal(precision: 12, scale: 2),
                        CategoryID = c.Int(nullable: false),
                        Rating = c.Decimal(nullable: false, precision: 3, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ProductID)
                .ForeignKey("dbo.Categories", t => t.CategoryID)
                .Index(t => t.Slug, unique: true)
                .Index(t => t.CategoryID);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryID = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 255),
                        ImageURL = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.CategoryID)
                .Index(t => t.CategoryName, unique: true);
            
            CreateTable(
                "dbo.ProductImages",
                c => new
                    {
                        ImageID = c.Int(nullable: false, identity: true),
                        ProductID = c.Int(nullable: false),
                        ImageURL = c.String(nullable: false, maxLength: 500),
                        DisplayOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ImageID)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        ReviewID = c.Int(nullable: false, identity: true),
                        ProductID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        Rating = c.Int(nullable: false),
                        Comment = c.String(),
                        ReviewDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ReviewID)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserID)
                .Index(t => new { t.ProductID, t.UserID }, unique: true, name: "UQ_Review_User_Product");
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 12, scale: 2),
                        OrderStatus = c.String(maxLength: 50),
                        ShippingName = c.String(nullable: false, maxLength: 100),
                        ShippingPhone = c.String(nullable: false, maxLength: 20),
                        ShippingTenDuong = c.String(nullable: false, maxLength: 255),
                        ShippingXaQuan = c.String(nullable: false, maxLength: 100),
                        ShippingTinhThanh = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        OrderItemID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        VariantID = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 12, scale: 2),
                        Subtotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.OrderItemID)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .ForeignKey("dbo.ProductVariants", t => t.VariantID)
                .Index(t => t.OrderID)
                .Index(t => t.VariantID);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        UserID = c.Int(nullable: false),
                        RoleID = c.Int(nullable: false),
                        AssignedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserID, t.RoleID })
                .ForeignKey("dbo.Roles", t => t.RoleID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.RoleID);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleID = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.RoleID)
                .Index(t => t.RoleName, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Addresses", "UserID", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "UserID", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "RoleID", "dbo.Roles");
            DropForeignKey("dbo.Orders", "UserID", "dbo.Users");
            DropForeignKey("dbo.OrderItems", "VariantID", "dbo.ProductVariants");
            DropForeignKey("dbo.OrderItems", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.Cart", "UserID", "dbo.Users");
            DropForeignKey("dbo.Cart", "VariantID", "dbo.ProductVariants");
            DropForeignKey("dbo.ProductVariants", "ProductID", "dbo.Products");
            DropForeignKey("dbo.Reviews", "UserID", "dbo.Users");
            DropForeignKey("dbo.Reviews", "ProductID", "dbo.Products");
            DropForeignKey("dbo.ProductImages", "ProductID", "dbo.Products");
            DropForeignKey("dbo.Products", "CategoryID", "dbo.Categories");
            DropIndex("dbo.Roles", new[] { "RoleName" });
            DropIndex("dbo.UserRoles", new[] { "RoleID" });
            DropIndex("dbo.UserRoles", new[] { "UserID" });
            DropIndex("dbo.OrderItems", new[] { "VariantID" });
            DropIndex("dbo.OrderItems", new[] { "OrderID" });
            DropIndex("dbo.Orders", new[] { "UserID" });
            DropIndex("dbo.Reviews", "UQ_Review_User_Product");
            DropIndex("dbo.ProductImages", new[] { "ProductID" });
            DropIndex("dbo.Categories", new[] { "CategoryName" });
            DropIndex("dbo.Products", new[] { "CategoryID" });
            DropIndex("dbo.Products", new[] { "Slug" });
            DropIndex("dbo.ProductVariants", new[] { "SKU" });
            DropIndex("dbo.ProductVariants", "UQ_Product_Size_Color");
            DropIndex("dbo.Cart", "UQ_Cart_User_Variant");
            DropIndex("dbo.Users", new[] { "Email" });
            DropIndex("dbo.Addresses", new[] { "UserID" });
            DropTable("dbo.Roles");
            DropTable("dbo.UserRoles");
            DropTable("dbo.OrderItems");
            DropTable("dbo.Orders");
            DropTable("dbo.Reviews");
            DropTable("dbo.ProductImages");
            DropTable("dbo.Categories");
            DropTable("dbo.Products");
            DropTable("dbo.ProductVariants");
            DropTable("dbo.Cart");
            DropTable("dbo.Users");
            DropTable("dbo.Addresses");
        }
    }
}
