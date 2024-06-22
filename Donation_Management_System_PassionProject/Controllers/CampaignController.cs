using Donation_Management_System_PassionProject.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace Donation_Management_System_PassionProject.Controllers
{
    public class CampaignController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static CampaignController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44306/api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Displays a list of campaigns.
        /// </summary>
        /// <returns>The view containing the list of campaigns.</returns>
        /// <example>GET: Campaign/List</example>
        public ActionResult List()
        {
            string url = "CampaignData/ListCampaigns";
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                IEnumerable<CampaignDto> campaigns = response.Content.ReadAsAsync<IEnumerable<CampaignDto>>().Result;
                return View(campaigns);
            }
            else
            {
                Debug.WriteLine("Failed to retrieve campaigns. Error: " + response.StatusCode);
                return View("Error");
            }
        }

        /// <summary>
        /// Displays details of a specific campaign.
        /// </summary>
        /// <param name="id">The ID of the campaign to be displayed.</param>
        /// <returns>The view containing details of the specified campaign.</returns>
        /// <example>GET: Campaign/Details/5</example>
        public ActionResult Details(int id)
        {
            string url = "CampaignData/FindCampaign/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                CampaignDto campaign = response.Content.ReadAsAsync<CampaignDto>().Result;
                return View(campaign);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return View("NotFound");
            }
            else
            {
                Debug.WriteLine("Failed to retrieve campaign details. Error: " + response.StatusCode);
                return View("Error");
            }
        }
        /// <summary>
        /// Displays a form for creating a new campaign.
        /// </summary>
        /// <returns>The view containing the form for creating a new campaign.</returns>
        /// <example>GET: Campaign/New</example>
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// Creates a new campaign.
        /// </summary>
        /// <param name="campaign">The campaign to be created.</param>
        /// <returns>Redirects to the list of campaigns upon successful creation, otherwise redirects to an error page.</returns>
        /// <example>POST: Campaign/Create</example>
        [HttpPost]
        public ActionResult Create(Campaign campaign)
        {
            string url = "CampaignData/AddCampaign";
            string jsonpayload = jss.Serialize(campaign);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                Debug.WriteLine("Failed to add campaign. Error: " + response.StatusCode);
                return View("Error");
            }
        }

        /// <summary>
        /// Displays a form for editing a campaign.
        /// </summary>
        /// <param name="id">The ID of the campaign to be edited.</param>
        /// <returns>The view containing the form for editing the specified campaign.</returns>
        /// <example>GET: Campaign/Edit/5</example>
        public ActionResult Edit(int id)
        {
            string url = "CampaignData/FindCampaign/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                CampaignDto campaign = response.Content.ReadAsAsync<CampaignDto>().Result;
                return View(campaign);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return View("NotFound");
            }
            else
            {
                Debug.WriteLine("Failed to retrieve campaign details for editing. Error: " + response.StatusCode);
                return View("Error");
            }
        }

        /// <summary>
        /// Updates a campaign.
        /// </summary>
        /// <param name="id">The ID of the campaign to be updated.</param>
        /// <param name="campaign">The updated campaign data.</param>
        /// <returns>Redirects to the details page of the updated campaign upon successful update, otherwise returns to the edit page.</returns>
        /// <example>POST: Campaign/Update/5</example>
        [HttpPost]
        public ActionResult Update(int id, Campaign campaign)
        {
            string url = "CampaignData/UpdateCampaign/" + id;
            string jsonpayload = jss.Serialize(campaign);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                Debug.WriteLine("Failed to update campaign. Error: " + response.StatusCode);
                return View("Error");
            }
        }

        /// <summary>
        /// Displays a confirmation page for deleting a campaign.
        /// </summary>
        /// <param name="id">The ID of the campaign to be deleted.</param>
        /// <returns>The view containing the confirmation message for deleting the specified campaign.</returns>
        /// <example>GET: Campaign/Delete/5</example>
        public ActionResult DeleteConfirm(int id)
        {
            string url = "CampaignData/FindCampaign/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                CampaignDto campaign = response.Content.ReadAsAsync<CampaignDto>().Result;
                return View(campaign);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return View("NotFound");
            }
            else
            {
                Debug.WriteLine("Failed to retrieve campaign details for deletion confirmation. Error: " + response.StatusCode);
                return View("Error");
            }
        }

        /// <summary>
        /// Deletes a campaign.
        /// </summary>
        /// <param name="id">The ID of the campaign to be deleted.</param>
        /// <returns>Redirects to the list of campaigns upon successful deletion, otherwise redirects to an error page.</returns>
        /// <example>POST: Campaign/Delete/5</example>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "CampaignData/DeleteCampaign/" + id;
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
