namespace HumanMobility.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Credentials : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserCredentials",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        HasAccelerometer = c.Boolean(nullable: false),
                        HasTemperatureSensor = c.Boolean(nullable: false),
                        DeviceInfo = c.String(nullable: false),
                        Version = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserCredentials", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserCredentials", new[] { "UserId" });
            DropTable("dbo.UserCredentials");
        }
    }
}
