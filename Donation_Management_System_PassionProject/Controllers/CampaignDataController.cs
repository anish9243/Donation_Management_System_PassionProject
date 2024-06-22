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
    public class CampaignDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all campaigns in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: All campaigns in the database.
        /// </returns>
        /// <example>
        /// GET: api/CampaignData/ListCampaigns
        /// </example>
        [HttpGet]
        [Route("api/CampaignData/ListCampaigns")]
        public IEnumerable<CampaignDto> ListCampaigns()
        {
            List<Campaign> campaigns = db.Campaigns.ToList();
            List<CampaignDto> campaignDtos = new List<CampaignDto>();

            campaigns.ForEach(c => campaignDtos.Add(new CampaignDto()
            {
                CampaignId = c.CampaignId,
                CampaignName = c.CampaignName,
                CampaignDescription = c.CampaignDescription,
            }));

            return campaignDtos;
        }

        /// <summary>
        /// Finds a specific campaign by ID.
        /// </summary>
        /// <param name="id">The ID of the campaign.</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: A campaign matching the provided ID.
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// GET: api/CampaignData/FindCampaign/5
        /// </example>
        [ResponseType(typeof(Campaign))]
        [HttpGet]
        [Route("api/CampaignData/FindCampaign/{id}")]

        public IHttpActionResult FindCampaign(int id)
        {
            Campaign campaign = db.Campaigns.Include(c => c.Donations.Select(d => d.Donor)).FirstOrDefault(c => c.CampaignId == id);
            if (campaign == null)
            {
                return NotFound();
            }

            CampaignDto campaignDto = new CampaignDto()
            {
                CampaignId = campaign.CampaignId,
                CampaignName = campaign.CampaignName,
                CampaignDescription = campaign.CampaignDescription,
                Donations = campaign.Donations.Select(d => new DonationDto
                {
                    DonationId = d.DonationId,
                    DonationAmount = d.DonationAmount,
                    DonationDate = d.DonationDate,
                    DonorId = d.DonorId,
                    DonorName = d.Donor.DonorName
                }).ToList()
            };

            return Ok(campaignDto);
        }

        /// <summary>
        /// Updates a campaign in the system with POST Data input.
        /// </summary>
        /// <param name="id">The ID of the campaign to be updated.</param>
        /// <param name="campaign">JSON form data of the campaign.</param>
        /// <returns>
        /// HEADER: 204 (No Content)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/CampaignData/UpdateCampaign/5
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/CampaignData/UpdateCampaign/{id}")]
        public IHttpActionResult UpdateCampaign(int id, Campaign campaign)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != campaign.CampaignId)
            {
                return BadRequest();
            }

            db.Entry(campaign).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CampaignExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Adds a new campaign to the system.
        /// </summary>
        /// <param name="campaign">JSON form data of the campaign.</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: The created campaign data.
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/CampaignData/AddCampaign
        /// </example>
        [ResponseType(typeof(Campaign))]
        [HttpPost]
        [Route("api/CampaignData/AddCampaign")]
        public IHttpActionResult AddCampaign(Campaign campaign)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Campaigns.Add(campaign);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Deletes a campaign from the system by ID.
        /// </summary>
        /// <param name="id">The ID of the campaign to be deleted.</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/CampaignData/DeleteCampaign/5
        /// </example>
        [ResponseType(typeof(Campaign))]
        [HttpPost]
        [Route("api/CampaignData/DeleteCampaign/{id}")]

        public IHttpActionResult DeleteCampaign(int id)
        {
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null)
            {
                return NotFound();
            }

            db.Campaigns.Remove(campaign);
            db.SaveChanges();

            return Ok();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CampaignExists(int id)
        {
            return db.Campaigns.Count(e => e.CampaignId == id) > 0;
        }
    }
}
