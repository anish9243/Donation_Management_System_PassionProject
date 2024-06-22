using Donation_Management_System_PassionProject.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;
using System.Web;

namespace Donation_Management_System_PassionProject.Controllers
{
    public class CampaignController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static CampaignController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };

            client = new HttpClient(handler);
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
        /// Displays a form for creating a new campaign.
        /// </summary>
        /// <returns>The view containing the form for creating a new campaign.</returns>
        /// <example>GET: Campaign/New</example>
        [Authorize]
        public ActionResult New()
        {
            GetApplicationCookie();//get token credentials
            return View();
        }

        /// <summary>
        /// Creates a new campaign.
        /// </summary>
        /// <param name="campaign">The campaign to be created.</param>
        /// <returns>Redirects to the list of campaigns upon successful creation, otherwise redirects to an error page.</returns>
        /// <example>POST: Campaign/Create</example>
        [HttpPost]
        [Authorize]
        public ActionResult Create(Campaign campaign)
        {
            GetApplicationCookie();//get token credentials
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
        [Authorize]
        public ActionResult Edit(int id)
        {
            GetApplicationCookie();//get token credentials
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
        [Authorize]
        public ActionResult Update(int id, Campaign campaign)
        {
            GetApplicationCookie();//get token credentials
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
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            GetApplicationCookie();//get token credentials
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
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();//get token credentials
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
