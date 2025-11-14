namespace ShoeStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addValidation1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Users", new[] { "Email" });
            AlterColumn("dbo.Users", "PasswordHash", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "Phone", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "Phone", c => c.String(maxLength: 20));
            AlterColumn("dbo.Users", "PasswordHash", c => c.String(nullable: false, maxLength: 255));
            CreateIndex("dbo.Users", "Email", unique: true);
        }
    }
}
