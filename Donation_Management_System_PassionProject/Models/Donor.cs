using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Donation_Management_System_PassionProject.Models
{
    public class Donor
    {
        [Key]
        public int DonorId { get; set; }  // Primary key

        public string DonorName { get; set; }

        public string DonorEmail { get; set; }

        // Many To Many Realtionship for related donations
        public virtual ICollection<Donation> Donations { get; set; }

    }
        public class DonorDto
        {
            public int DonorId { get; set; }
            public string DonorName { get; set; }
            public string DonorEmail { get; set; }

        // Additional properties if needed
        // Add a list of donations
        public ICollection<DonationDto> Donations { get; set; }

    }
}