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

        public DateTime CampaignStartDate { get; set; }

        public DateTime CampaignEndDate { get; set; }

        // Many to Many Realtion property for related donations
        public virtual ICollection<Donation> Donations { get; set; }
    }
        public class CampaignDto
        {
            public int CampaignId { get; set; }
            public string CampaignName { get; set; }
            public string CampaignDescription { get; set; }
            public DateTime CampaignStartDate { get; set; }
            public DateTime CampaignEndDate { get; set; }


        // Additional properties if needed\
        public ICollection<DonationDto> Donations { get; set; }
        public int DonorId { get; internal set; }
    }
    }
