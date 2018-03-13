namespace HumanMobility.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LocationIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Locations", new[] { "UserId" });
            CreateIndex("dbo.Locations", new[] { "UserId", "SaveTime" }, unique: true, name: "UserAndSaveTime_Index");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Locations", "UserAndSaveTime_Index");
            CreateIndex("dbo.Locations", "UserId");
        }
    }
}
