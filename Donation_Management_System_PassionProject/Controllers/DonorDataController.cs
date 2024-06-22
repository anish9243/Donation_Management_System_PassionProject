using Donation_Management_System_PassionProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Donation_Management_System_PassionProject.Controllers
{
    public class DonorDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all donors in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all donors in the database.
        /// </returns>
        /// <example>
        /// GET: api/DonorData/ListDonors
        /// </example>
        [HttpGet]
        [Route("api/DonorData/ListDonors")]
        public IEnumerable<DonorDto> ListDonors()
        {
            List<Donor> Donors = db.Donors.ToList();
            List<DonorDto> DonorDtos = new List<DonorDto>();

            Donors.ForEach(d => DonorDtos.Add(new DonorDto()
            {
                DonorId = d.DonorId,
                DonorName = d.DonorName,
                DonorEmail = d.DonorEmail
            }));

            return DonorDtos;
        }

        /// <summary>
        /// Finds a specific donor by ID.
        /// </summary>
        /// <param name="id">The ID of the donor.</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: A donor matching the provided ID.
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// GET: api/DonorData/FindDonor/5
        /// </example>
        [ResponseType(typeof(Donor))]
        [HttpGet]
        [Route("api/DonorData/FindDonor/{id}")]
        
        public IHttpActionResult FindDonor(int id)
        {
            Donor donor = db.Donors.Include(d => d.Donations).FirstOrDefault(d => d.DonorId == id);
            if (donor == null)
            {
                return NotFound();
            }

            DonorDto donorDto = new DonorDto()
            {
                DonorId = donor.DonorId,
                DonorName = donor.DonorName,
                DonorEmail = donor.DonorEmail,
                Donations = donor.Donations.Select(d => new DonationDto
                {
                    DonationId = d.DonationId,
                    DonationAmount = d.DonationAmount,
                    CampaignName = d.Campaign.CampaignName,
                    DonationDate = d.DonationDate
                }).ToList()
            };

            return Ok(donorDto);
        }

        /// <summary>
        /// Updates a donor in the system with POST Data input.
        /// </summary>
        /// <param name="id">The ID of the donor to be updated.</param>
        /// <param name="donor">JSON form data of the donor.</param>
        /// <returns>
        /// HEADER: 204 (No Content)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/DonorData/UpdateDonor/5
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/DonorData/UpdateDonor/{id}")]
        public IHttpActionResult UpdateDonor(int id, Donor donor)
        {
            Debug.WriteLine("I have reached the update donor method!");
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is invalid");
                return BadRequest(ModelState);
            }

            if (id != donor.DonorId)
            {
                Debug.WriteLine("ID mismatch");
                Debug.WriteLine("GET parameter" + id);
                Debug.WriteLine("POST parameter" + donor.DonorId);
                Debug.WriteLine("POST parameter" + donor.DonorName);
                Debug.WriteLine("POST parameter " + donor.DonorEmail);
                return BadRequest();
            }

            db.Entry(donor).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DonorExists(id))
                {
                    Debug.WriteLine("Donor not found");
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
        /// Adds a new donor to the system.
        /// </summary>
        /// <param name="donor">JSON form data of the donor.</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: The created donor data.
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/DonorData/AddDonor
        /// </example>
        [ResponseType(typeof(Donor))]
        [HttpPost]
        [Route("api/DonorData/AddDonor")]
        public IHttpActionResult AddDonor(Donor donor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Donors.Add(donor);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Deletes a donor from the system by ID.
        /// </summary>
        /// <param name="id">The ID of the donor to be deleted.</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/DonorData/DeleteDonor/5
        /// </example>
        [ResponseType(typeof(Donor))]
        [HttpPost]
        [Route("api/DonorData/DeleteDonor/{id}")]
        public IHttpActionResult DeleteDonor(int id)
        {
            Donor donor = db.Donors.Find(id);
            if (donor == null)
            {
                return NotFound();
            }

            db.Donors.Remove(donor);
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

        private bool DonorExists(int id)
        {
            return db.Donors.Count(e => e.DonorId == id) > 0;
        }
    }
}
