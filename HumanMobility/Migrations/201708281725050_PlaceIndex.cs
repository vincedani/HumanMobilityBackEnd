namespace HumanMobility.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlaceIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Places", new[] { "UserId" });
            CreateIndex("dbo.Places", new[] { "UserId", "Latitude", "Longitude" }, unique: true, name: "UserAndLocation_Index");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Places", "UserAndLocation_Index");
            CreateIndex("dbo.Places", "UserId");
        }
    }
}
