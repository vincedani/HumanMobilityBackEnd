namespace HumanMobility.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Location : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        SaveTime = c.DateTime(nullable: false),
                        DetectionTime = c.DateTime(nullable: false),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        Accuary = c.Single(nullable: false),
                        Error = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Locations", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Locations", new[] { "UserId" });
            DropTable("dbo.Locations");
        }
    }
}
