namespace ShoeStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ValidBackToNormal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "PasswordHash", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Users", "Phone", c => c.String(maxLength: 20));
            CreateIndex("dbo.Users", "Email", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", new[] { "Email" });
            AlterColumn("dbo.Users", "Phone", c => c.String());
            AlterColumn("dbo.Users", "PasswordHash", c => c.String(nullable: false));
        }
    }
}
