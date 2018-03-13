namespace HumanMobility.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BugReports : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BugReports",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        Message = c.String(nullable: false, maxLength: 300),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BugReports", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.BugReports", new[] { "UserId" });
            DropTable("dbo.BugReports");
        }
    }
}
