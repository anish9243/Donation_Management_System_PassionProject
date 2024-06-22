using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Donation_Management_System_PassionProject.Models.ViewModels
{
    public class NewDonation
    {
        // All donors to choose from when creating this donation
        public IEnumerable<DonorDto> DonorOptions { get; set; }

        // All campaigns to choose from when creating this donation
        public IEnumerable<CampaignDto> CampaignOptions { get; set; }
    }
}
