using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Donation_Management_System_PassionProject.Models
{
    public class Campaign
    {
        [Key]
        public int CampaignId { get; set; }  // Primary key

        public string CampaignName { get; set; }

        public string CampaignDescription { get; set; }

        // Many to Many Realtion property for related donations
        public virtual ICollection<Donation> Donations { get; set; }
    }
}