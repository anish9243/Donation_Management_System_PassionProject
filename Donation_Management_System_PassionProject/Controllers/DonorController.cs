using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
using Donation_Management_System_PassionProject.Models;
using System.Web.Script.Serialization;



namespace Donation_Management_System_PassionProject.Controllers
{
    public class DonorController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static DonorController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44306/api/");
        }

        /// <summary>
        /// Displays a list of donors.
        /// </summary>
        /// <returns>
        /// The view containing the list of donors.
        /// </returns>
        /// <example>
        /// GET: Donor/List
        /// </example>

        // GET: Donor/List
        public ActionResult List()
        {
            //objective: communicate with our donor data api to retrieve a list of donors
            //curl https://localhost:44324/api/donordata/listdonors


            string url = "donordata/listdonors";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<DonorDto> donors = response.Content.ReadAsAsync<IEnumerable<DonorDto>>().Result;
            //Debug.WriteLine("Number of donors received : ");
            //Debug.WriteLine(donors.Count());


            return View(donors);
        }

        /// <summary>
        /// Displays details of a specific donor.
        /// </summary>
        /// <param name="id">The ID of the donor to be displayed.</param>
        /// <returns>
        /// The view containing details of the specified donor.
        /// </returns>
        /// <example>
        /// GET: Donor/Details/5
        /// </example>

        // GET: Donor/Details/5
        public ActionResult Details(int id)
        {

            string url = "donordata/finddonor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            DonorDto selecteddonor = response.Content.ReadAsAsync<DonorDto>().Result;
            Debug.WriteLine("donor received : ");
            Debug.WriteLine(selecteddonor.DonorName);


            return View(selecteddonor);
        }


        public ActionResult Error()
        {

            return View();
        }

        /// <summary>
        /// Displays a form for creating a new donor.
        /// </summary>
        /// <returns>
        /// The view containing the form for creating a new donor.
        /// </returns>
        /// <example>
        /// GET: Donor/New
        /// </example>

        // GET: Donor/New
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// Creates a new donor.
        /// </summary>
        /// <param name="donor">The donor to be created.</param>
        /// <returns>
        /// Redirects to the list of donors upon successful creation, otherwise redirects to an error page.
        /// </returns>
        /// <example>
        /// POST: Donor/Create
        /// </example>

        // POST: Donor/Create
        [HttpPost]
        public ActionResult Create(Donor donor)
        {
            Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(donor.DonorName);
            //objective: add a new donor into our system using the API
            //curl -H "Content-Type:application/json" -d @donor.json https://localhost:44324/api/donordata/adddonor 
            string url = "donordata/adddonor";


            string jsonpayload = jss.Serialize(donor);
            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
 
        }

        /// <summary>
        /// Displays a form for editing a donor.
        /// </summary>
        /// <param name="id">The ID of the donor to be edited.</param>
        /// <returns>
        /// The view containing the form for editing the specified donor.
        /// </returns>
        /// <example>
        /// GET: Donor/Edit/5
        /// </example>

        // GET: Donor/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "donordata/finddonor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            DonorDto selecteddonor = response.Content.ReadAsAsync<DonorDto>().Result;
            Debug.WriteLine("donor received : ");
            Debug.WriteLine(selecteddonor.DonorName);

            return View(selecteddonor);
        }

        /// <summary>
        /// Updates a donor.
        /// </summary>
        /// <param name="id">The ID of the donor to be updated.</param>
        /// <param name="donor">The updated donor data.</param>
        /// <returns>
        /// Redirects to the details page of the updated donor upon successful update, otherwise returns to the edit page.
        /// </returns>
        /// <example>
        /// POST: Donor/Update/5
        /// </example>

        // POST: Donor/Update/5
        [HttpPost]
        public ActionResult Update(int id, Donor donor)
        {

            try
            {
                Debug.WriteLine("The new donor info is:");
                Debug.WriteLine(donor.DonorId);
                Debug.WriteLine(donor.DonorEmail);
                Debug.WriteLine(donor.DonorName);

                //serialize into JSON
                //Send the request to the API

                string url = "donordata/UpdateDonor/" + id;


                string jsonpayload = jss.Serialize(donor);
                Debug.WriteLine(jsonpayload);

                HttpContent content = new StringContent(jsonpayload);
                content.Headers.ContentType.MediaType = "application/json";

                //POST: api/DonorData/UpdateDonor/{id}
                //Header : Content-Type: application/json
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                return RedirectToAction("Details/" + id);
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Displays a confirmation page for deleting a donor.
        /// </summary>
        /// <param name="id">The ID of the donor to be deleted.</param>
        /// <returns>
        /// The view containing the confirmation message for deleting the specified donor.
        /// </returns>
        /// <example>
        /// GET: Donor/Delete/5
        /// </example>

        // GET: Donor/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "donordata/finddonor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            DonorDto selecteddonor = response.Content.ReadAsAsync<DonorDto>().Result;
            return View(selecteddonor);
        }

        /// <summary>
        /// Deletes a donor.
        /// </summary>
        /// <param name="id">The ID of the donor to be deleted.</param>
        /// <returns>
        /// Redirects to the list of donors upon successful deletion, otherwise redirects to an error page.
        /// </returns>
        /// <example>
        /// POST: Donor/Delete/5
        /// </example>

        // POST: Donor/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "donordata/deletedonor/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}