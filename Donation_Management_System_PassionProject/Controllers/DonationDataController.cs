using Donation_Management_System_PassionProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Donation_Management_System_PassionProject.Controllers
{
    public class DonationDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all donations in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: All donations in the database.
        /// </returns>
        /// <example>
        /// GET: api/DonationData/ListDonations
        /// </example>
        [HttpGet]
        [Route("api/DonationData/ListDonations")]
        public IEnumerable<DonationDto> ListDonations()
        {
            List<Donation> donations = db.Donations.ToList();
            List<DonationDto> donationDtos = new List<DonationDto>();

            donations.ForEach(d => donationDtos.Add(new DonationDto()
            {
                DonationId = d.DonationId,
                DonationAmount = d.DonationAmount,
                DonationDate = d.DonationDate,
                DonorId = d.DonorId,
                DonorName = d.Donor.DonorName,
                CampaignId = d.CampaignId,
                CampaignName = d.Campaign.CampaignName
            }));

            return donationDtos;
        }

        /// <summary>
        /// Finds a specific donation by ID.
        /// </summary>
        /// <param name="id">The ID of the donation.</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: A donation matching the provided ID.
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// GET: api/DonationData/FindDonation/5
        /// </example>
        [ResponseType(typeof(DonationDto))]
        [HttpGet]
        [Route("api/DonationData/FindDonation/{id}")]
        public IHttpActionResult FindDonation(int id)
        {
            Donation donation = db.Donations.Find(id);
            if (donation == null)
            {
                return NotFound();
            }

            DonationDto donationDto = new DonationDto()
            {
                DonationId = donation.DonationId,
                DonationAmount = donation.DonationAmount,
                DonationDate = donation.DonationDate,
                DonorId = donation.DonorId,
                DonorName = donation.Donor.DonorName,
                CampaignId = donation.CampaignId,
                CampaignName = donation.Campaign.CampaignName
            };

            return Ok(donationDto);
        }

        /// <summary>
        /// Updates a donation in the system with POST Data input.
        /// </summary>
        /// <param name="id">The ID of the donation to be updated.</param>
        /// <param name="donation">JSON form data of the donation.</param>
        /// <returns>
        /// HEADER: 204 (No Content)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/DonationData/UpdateDonation/5
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/DonationData/UpdateDonation/{id}")]
        [Authorize]
        public IHttpActionResult UpdateDonation(int id, Donation donation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != donation.DonationId)
            {
                return BadRequest();
            }

            db.Entry(donation).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DonationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            Debug.WriteLine("None of the conditions triggered");
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Adds a new donation to the system.
        /// </summary>
        /// <param name="donation">JSON form data of the donation.</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: The created donation data.
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/DonationData/AddDonation
        /// </example>
        [ResponseType(typeof(Donation))]
        [HttpPost]
        [Route("api/DonationData/AddDonation")]
        [Authorize]
        public IHttpActionResult AddDonation(Donation donation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Donations.Add(donation);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Deletes a donation from the system by ID.
        /// </summary>
        /// <param name="id">The ID of the donation to be deleted.</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/DonationData/DeleteDonation/5
        /// </example>
        [ResponseType(typeof(Donation))]
        [HttpPost]
        [Route("api/DonationData/DeleteDonation/{id}")]
        [Authorize]
        public IHttpActionResult DeleteDonation(int id)
        {
            Donation donation = db.Donations.Find(id);
            if (donation == null)
            {
                return NotFound();
            }

            db.Donations.Remove(donation);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Returns all donations related to a specific donor ID.
        /// </summary>
        /// <param name="id">The donor ID.</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: All donations related to the specified donor ID.
        /// </returns>
        /// <example>
        /// GET: api/DonationData/ListDonationsForDonor/3
        /// </example>
        [HttpGet]
        [Route("api/DonationData/ListDonationsForDonor/{id}")]
        public IEnumerable<DonationDto> ListDonationsForDonor(int id)
        {
            List<Donation> donations = db.Donations.Where(d => d.DonorId == id).ToList();
            List<DonationDto> donationDtos = new List<DonationDto>();

            donations.ForEach(d => donationDtos.Add(new DonationDto()
            {
                DonationId = d.DonationId,
                DonationAmount = d.DonationAmount,
                DonationDate = d.DonationDate,
                DonorId = d.DonorId,
                DonorName = d.Donor.DonorName,
                CampaignId = d.CampaignId,
                CampaignName = d.Campaign.CampaignName
            }));

            return donationDtos;
        }

        /// <summary>
        /// Returns all donations related to a specific campaign ID.
        /// </summary>
        /// <param name="id">The campaign ID.</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: All donations related to the specified campaign ID.
        /// </returns>
        /// <example>
        /// GET: api/DonationData/ListDonationsForCampaign/5
        /// </example>
        [HttpGet]
        [Route("api/DonationData/ListDonationsForCampaign/{id}")]
        public IEnumerable<DonationDto> ListDonationsForCampaign(int id)
        {
            List<Donation> donations = db.Donations.Where(d => d.CampaignId == id).ToList();
            List<DonationDto> donationDtos = new List<DonationDto>();

            donations.ForEach(d => donationDtos.Add(new DonationDto()
            {
                DonationId = d.DonationId,
                DonationAmount = d.DonationAmount,
                DonationDate = d.DonationDate,
                DonorId = d.DonorId,
                DonorName = d.Donor.DonorName,
                CampaignId = d.CampaignId,
                CampaignName = d.Campaign.CampaignName
            }));

            return donationDtos;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DonationExists(int id)
        {
            return db.Donations.Count(e => e.DonationId == id) > 0;
        }
    }
}
