using Donation_Management_System_PassionProject.Models;
using Donation_Management_System_PassionProject.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Donation_Management_System_PassionProject.Controllers
{
    public class DonationController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static DonationController()
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

        // GET: Donation/List

        public ActionResult List()
        {
            //objective: communicate with our animal data api to retrieve a list of donations
            //curl https://localhost:44306/api/donationdata/listdonations


            string url = "donationdata/listdonations";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<DonationDto> donations = response.Content.ReadAsAsync<IEnumerable<DonationDto>>().Result;
            //Debug.WriteLine("Number of donations received : ");
            //Debug.WriteLine(donations.Count());


            return View(donations);
        }
        // GET: Donation/Details/5
        public ActionResult Details(int id)
        {
            DetailsDonation ViewModel = new DetailsDonation();

            // Fetch the selected donation details
            string url = "donationdata/finddonation/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                DonationDto SelectedDonation = response.Content.ReadAsAsync<DonationDto>().Result;
                ViewModel.SelectedDonation = SelectedDonation;

                // Fetch available donors
                url = "donordata/listdonors";
                response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    ViewModel.AvailableDonors = response.Content.ReadAsAsync<IEnumerable<DonorDto>>().Result;
                }

                // Fetch available campaigns
                url = "campaigndata/listcampaigns";
                response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    ViewModel.AvailableCampaigns = response.Content.ReadAsAsync<IEnumerable<CampaignDto>>().Result;
                }

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Donation/ListForDonor/3
        public ActionResult ListForDonor(int id)
        {
            string url = "donationdata/listdonationsfordonor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<DonationDto> donations = response.Content.ReadAsAsync<IEnumerable<DonationDto>>().Result;
            return View(donations);
        }

        // GET: Donation/ListForCampaign/5
        public ActionResult ListForCampaign(int id)
        {
            string url = "donationdata/listdonationsforcampaign/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<DonationDto> donations = response.Content.ReadAsAsync<IEnumerable<DonationDto>>().Result;
            return View(donations);
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

        // GET: Donation/New
        [Authorize]
        public ActionResult New()
        {
            GetApplicationCookie();//get token credentials
            NewDonation ViewModel = new NewDonation();

            // Get information about all donors in the system.
            string url = "donordata/listdonors";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<DonorDto> DonorOptions = response.Content.ReadAsAsync<IEnumerable<DonorDto>>().Result;
            ViewModel.DonorOptions = DonorOptions;

            // Get information about all campaigns in the system.
            url = "campaigndata/listcampaigns";
            response = client.GetAsync(url).Result;
            IEnumerable<CampaignDto> CampaignOptions = response.Content.ReadAsAsync<IEnumerable<CampaignDto>>().Result;
            ViewModel.CampaignOptions = CampaignOptions;

            return View(ViewModel);
        }
        // POST: Donation/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Donation donation)
        {
            GetApplicationCookie();//get token credentials
            string url = "donationdata/adddonation";
            string jsonpayload = jss.Serialize(donation);

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

        // GET: Donation/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            GetApplicationCookie();//get token credentials
            UpdateDonation ViewModel = new UpdateDonation();

            string url = "donationdata/finddonation/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            DonationDto SelectedDonation = response.Content.ReadAsAsync<DonationDto>().Result;
            ViewModel.SelectedDonation = SelectedDonation;

            url = "donordata/listdonors";
            response = client.GetAsync(url).Result;
            IEnumerable<DonorDto> DonorOptions = response.Content.ReadAsAsync<IEnumerable<DonorDto>>().Result;

            url = "campaigndata/listcampaigns";
            response = client.GetAsync(url).Result;
            IEnumerable<CampaignDto> CampaignOptions = response.Content.ReadAsAsync<IEnumerable<CampaignDto>>().Result;

            ViewModel.DonorOptions = DonorOptions;
            ViewModel.CampaignOptions = CampaignOptions;

            return View(ViewModel);
        }

        // POST: Donation/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Donation donation)
        {
            GetApplicationCookie();//get token credentials
            string url = "donationdata/updatedonation/" + id;
            string jsonpayload = jss.Serialize(donation);
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

        // GET: Donation/Delete/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            GetApplicationCookie();//get token credentials
            string url = "donationdata/finddonation/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            DonationDto selectedDonation = response.Content.ReadAsAsync<DonationDto>().Result;
            return View(selectedDonation);
        }

        // POST: Donation/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();//get token credentials
            string url = "donationdata/deletedonation/" + id;
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

        public ActionResult Error()
        {
            return View();
        }
    }
}
