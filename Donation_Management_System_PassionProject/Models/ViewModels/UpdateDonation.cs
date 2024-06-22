using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Donation_Management_System_PassionProject.Models.ViewModels
{
    public class UpdateDonation
    {
        // This viewmodel is a class which stores information that we need to present to /Donation/Update/{}

        // The existing donation information
        public DonationDto SelectedDonation { get; set; }

        // All donors to choose from when updating this donation
        public IEnumerable<DonorDto> DonorOptions { get; set; }

        // All campaigns to choose from when updating this donation
        public IEnumerable<CampaignDto> CampaignOptions { get; set; }
    }
}
