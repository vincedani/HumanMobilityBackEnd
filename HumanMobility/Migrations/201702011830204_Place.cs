namespace HumanMobility.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Place : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Places",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Type = c.String(nullable: false),
                        Title = c.String(nullable: false),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        Radius = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Places", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Places", new[] { "UserId" });
            DropTable("dbo.Places");
        }
    }
}
