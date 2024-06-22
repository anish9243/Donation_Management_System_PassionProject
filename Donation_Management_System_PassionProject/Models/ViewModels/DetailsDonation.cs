using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Donation_Management_System_PassionProject.Models.ViewModels
{
    public class DetailsDonation
    {
        // This viewmodel is a class which stores information that we need to present to /Donation/Details/{}

        // The selected donation details
        public DonationDto SelectedDonation { get; set; }

        // Donor details associated with the selected donation
        public DonorDto AssociatedDonor { get; set; }

        // Campaign details associated with the selected donation
        public CampaignDto AssociatedCampaign { get; set; }

        // All available donors (for possible reassignment or information)
        public IEnumerable<DonorDto> AvailableDonors { get; set; }

        // All available campaigns (for possible reassignment or information)
        public IEnumerable<CampaignDto> AvailableCampaigns { get; set; }
    }
}