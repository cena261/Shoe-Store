namespace ShoeStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPasswordResetToken : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PasswordResetTokens",
                c => new
                    {
                        TokenId = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 255),
                        Code = c.String(nullable: false, maxLength: 6),
                        CreatedAt = c.DateTime(nullable: false),
                        ExpiresAt = c.DateTime(nullable: false),
                        IsUsed = c.Boolean(nullable: false),
                        UsedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.TokenId)
                .Index(t => t.Email);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.PasswordResetTokens", new[] { "Email" });
            DropTable("dbo.PasswordResetTokens");
        }
    }
}
