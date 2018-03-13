namespace HumanMobility.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Activities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activities",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        SaveTime = c.DateTime(nullable: false),
                        Summary = c.Double(nullable: false),
                        Count = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => new { t.UserId, t.SaveTime }, unique: true, name: "Actigraphy_Index");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Activities", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Activities", "Actigraphy_Index");
            DropTable("dbo.Activities");
        }
    }
}
