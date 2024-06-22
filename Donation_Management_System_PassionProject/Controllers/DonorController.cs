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
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };

            client = new HttpClient(handler);
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
            //curl https://localhost:44306/api/donordata/listdonors


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
            string url = "DonorData/FindDonor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                DonorDto donor = response.Content.ReadAsAsync<DonorDto>().Result;
                return View(donor);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return View("NotFound");
            }
            else
            {
                Debug.WriteLine("Failed to retrieve donor details. Error: " + response.StatusCode);
                return View("Error");
            }
        }


        public ActionResult Error()
        {

            return View();
        }
        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// For proper WebAPI authentication, you can send a post request with login credentials to the WebAPI and log the access token from the response. The controller already knows this token, so we're just passing it up the chain.
        /// 
        /// Here is a descriptive article which walks through the process of setting up authorization/authentication directly.
        /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
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
        [Authorize]
        public ActionResult New()
        {
            GetApplicationCookie();//get token credentials
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
        [Authorize]
        public ActionResult Create(Donor donor)
        {
            GetApplicationCookie();//get token credentials
            Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(donor.DonorName);
            //objective: add a new donor into our system using the API
            //curl -H "Content-Type:application/json" -d @donor.json https://localhost:44306/api/donordata/adddonor 
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
        [Authorize]
        public ActionResult Edit(int id)
        {
            GetApplicationCookie();//get token credentials
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
        [Authorize]
        public ActionResult Update(int id, Donor donor)
        {
            GetApplicationCookie();//get token credentials

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
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            GetApplicationCookie();//get token credentials
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
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();//get token credentials
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