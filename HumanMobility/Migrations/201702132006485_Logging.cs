namespace HumanMobility.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Logging : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        AdminId = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Time = c.DateTime(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.AdminId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.AdminId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Logs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Logs", "AdminId", "dbo.AspNetUsers");
            DropIndex("dbo.Logs", new[] { "UserId" });
            DropIndex("dbo.Logs", new[] { "AdminId" });
            DropTable("dbo.Logs");
        }
    }
}
