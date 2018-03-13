namespace HumanMobility.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SaveRAWData : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Activities", "Actigraphy_Index");
            AddColumn("dbo.Activities", "X", c => c.Single(nullable: false));
            AddColumn("dbo.Activities", "Y", c => c.Single(nullable: false));
            AddColumn("dbo.Activities", "Z", c => c.Single(nullable: false));
            CreateIndex("dbo.Activities", "UserId");
            DropColumn("dbo.Activities", "Summary");
            DropColumn("dbo.Activities", "Count");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Activities", "Count", c => c.Short(nullable: false));
            AddColumn("dbo.Activities", "Summary", c => c.Double(nullable: false));
            DropIndex("dbo.Activities", new[] { "UserId" });
            DropColumn("dbo.Activities", "Z");
            DropColumn("dbo.Activities", "Y");
            DropColumn("dbo.Activities", "X");
            CreateIndex("dbo.Activities", new[] { "UserId", "SaveTime" }, unique: true, name: "Actigraphy_Index");
        }
    }
}
