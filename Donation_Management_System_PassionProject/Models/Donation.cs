using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Donation_Management_System_PassionProject.Models
{
    public class Donation
    {
        [Key]
        public int DonationId { get; set; }  // Primary key

        public decimal DonationAmount { get; set; }

        public DateTime DonationDate { get; set; }

        // Navigation properties
        [ForeignKey("Donor")]
        public int DonorId { get; set; }  // Foreign key to Donor
        public virtual Donor Donor { get; set; }  // Navigation property to Donor

        [ForeignKey("Campaign")]
        public int CampaignId { get; set; }  // Foreign key to Campaign
        public virtual Campaign Campaign { get; set; }  // Navigation property to Campaign
    }
}