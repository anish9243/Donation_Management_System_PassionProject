namespace Donation_Management_System_PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial_Migration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Campaigns",
                c => new
                    {
                        CampaignId = c.Int(nullable: false, identity: true),
                        CampaignName = c.String(),
                        CampaignDescription = c.String(),
                    })
                .PrimaryKey(t => t.CampaignId);
            
            CreateTable(
                "dbo.Donations",
                c => new
                    {
                        DonationId = c.Int(nullable: false, identity: true),
                        DonationAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DonationDate = c.DateTime(nullable: false),
                        DonorId = c.Int(nullable: false),
                        CampaignId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DonationId)
                .ForeignKey("dbo.Campaigns", t => t.CampaignId, cascadeDelete: true)
                .ForeignKey("dbo.Donors", t => t.DonorId, cascadeDelete: true)
                .Index(t => t.DonorId)
                .Index(t => t.CampaignId);
            
            CreateTable(
                "dbo.Donors",
                c => new
                    {
                        DonorId = c.Int(nullable: false, identity: true),
                        DonorName = c.String(),
                        DonorEmail = c.String(),
                    })
                .PrimaryKey(t => t.DonorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Donations", "DonorId", "dbo.Donors");
            DropForeignKey("dbo.Donations", "CampaignId", "dbo.Campaigns");
            DropIndex("dbo.Donations", new[] { "CampaignId" });
            DropIndex("dbo.Donations", new[] { "DonorId" });
            DropTable("dbo.Donors");
            DropTable("dbo.Donations");
            DropTable("dbo.Campaigns");
        }
    }
}
