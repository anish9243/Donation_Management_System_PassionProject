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

        // GET: Donor/New
        public ActionResult New()
        {
            return View();
        }

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

        // GET: Donor/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "donordata/finddonor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            DonorDto selecteddonor = response.Content.ReadAsAsync<DonorDto>().Result;
            return View(selecteddonor);
        }

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