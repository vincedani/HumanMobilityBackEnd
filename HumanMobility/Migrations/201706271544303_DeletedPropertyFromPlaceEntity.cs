namespace HumanMobility.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeletedPropertyFromPlaceEntity : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Places", "Deleted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Places", "Deleted", c => c.Boolean(nullable: false));
        }
    }
}
