using Donation_Management_System_PassionProject.Models;
using Donation_Management_System_PassionProject.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
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
            client = new HttpClient();
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

        // GET: Donation/New
        public ActionResult New()
        {
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
        public ActionResult Create(Donation donation)
        {
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
        public ActionResult Edit(int id)
        {
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
        public ActionResult Update(int id, Donation donation)
        {
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
        public ActionResult DeleteConfirm(int id)
        {
            string url = "donationdata/finddonation/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            DonationDto selectedDonation = response.Content.ReadAsAsync<DonationDto>().Result;
            return View(selectedDonation);
        }

        // POST: Donation/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
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
