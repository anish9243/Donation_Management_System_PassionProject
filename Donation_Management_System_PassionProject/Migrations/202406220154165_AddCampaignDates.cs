namespace Donation_Management_System_PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCampaignDates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Campaigns", "CampaignStartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Campaigns", "CampaignEndDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Campaigns", "CampaignEndDate");
            DropColumn("dbo.Campaigns", "CampaignStartDate");
        }
    }
}
