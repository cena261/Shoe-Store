namespace ShoeStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCreatedAt : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "createdAt", c => c.DateTime(nullable: false));
            AlterColumn("dbo.UserRoles", "AssignedAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserRoles", "AssignedAt", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Users", "createdAt", c => c.DateTime(nullable: false));
        }
    }
}
