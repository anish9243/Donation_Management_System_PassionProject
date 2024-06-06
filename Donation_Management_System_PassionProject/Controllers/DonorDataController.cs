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

        // GET: api/DonorData/ListDonors
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

        // GET: api/DonorData/FindDonor/5
        [ResponseType(typeof(Donor))]
        [HttpGet]
        [Route("api/DonorData/FindDonor/{id}")]
        public IHttpActionResult FindDonor(int id)
        {
            Donor Donor = db.Donors.Find(id);
            if (Donor == null)
            {
                return NotFound();
            }

            DonorDto DonorDto = new DonorDto()
            {
                DonorId = Donor.DonorId,
                DonorName = Donor.DonorName,
                DonorEmail = Donor.DonorEmail
            };

            return Ok(DonorDto);
        }

        // POST: api/DonorData/UpdateDonor/5
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

        // POST: api/DonorData/AddDonor
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

        // POST: api/DonorData/DeleteDonor/5
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
