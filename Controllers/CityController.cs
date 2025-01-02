using APIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace APIConsume.Controllers
{
    public class CityController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5163/api");
        private readonly HttpClient _client;

        public CityController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }



        [HttpGet]
        public IActionResult CityList()
        {
            List<CityModel> city = new List<CityModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/City").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                /*  dynamic jsonObject = JsonConvert.DeserializeObject(data);*/

                city = JsonConvert.DeserializeObject<List<CityModel>>(data);
            }

            return View("CityList", city);
        }

        [HttpGet("{CityID}")]
        [Route("oooo")]

        public IActionResult CityAddEdit(int? CityID)
        {
            CityModel cityById = null;

            if (CityID != null)
            {
                HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/City/{CityID}").Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;

                    // Deserialize the data as a list of CityModel
                    var cities = JsonConvert.DeserializeObject<List<CityModel>>(data);

                    // Get the first city from the list if it exists
                    cityById = cities?.FirstOrDefault();
                }

                return View("CityAddEdit", cityById);
            }
            return View("CityAddEdit", new CityModel());

        }




        [HttpPost]
        public async Task<IActionResult> Save(CityModel city)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(city);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (city.CityID == null)
                    response = await _client.PostAsync($"{_client.BaseAddress}/City", content);
                else
                    response = await _client.PutAsync($"{_client.BaseAddress}/City", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("CityList");
            }
            await LoadCountryList();
            return View("CityAddEdit", city);
        }

        public async Task<IActionResult> Delete(int CityID)
        {
            var response = await _client.DeleteAsync($"{_client.BaseAddress}/City/?CityID={CityID}");
            return RedirectToAction("CityList");
        }

        private async Task LoadCountryList()
        {
            var response = await _client.GetAsync($"{_client.BaseAddress}/Country");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var countries = JsonConvert.DeserializeObject<List<CountryDropDownModel>>(data);
                ViewBag.CountryList = countries;
            }
        }

        /* [HttpPost]
         public async Task<JsonResult> GetStatesByCountry(int CountryID)
         {
             var states = await GetStatesByCountryID(CountryID);
             return Json(states);
         }*/

        [HttpPost]
        public async Task<JsonResult> GetStatesByCountry([FromBody] int CountryID)
        {
            // Fetch states based on the CountryID
            var states = await GetStatesByCountryID(CountryID);

            // Return as JSON
            return Json(states);
        }

        private async Task<List<StateDropDownModel>> GetStatesByCountryID(int CountryID)
        {
            // Example: Replace with your API/service logic
            var response = await _client.GetAsync($"{_client.BaseAddress}/State/by-country/{CountryID}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<StateDropDownModel>>(data);
            }
            return new List<StateDropDownModel>();
        }



        private async Task<List<CountryDropDownModel>> GetCountryList()
        {
            var response = await _client.GetAsync($"{_client.BaseAddress}/Country/LoadList");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<CountryDropDownModel>>(data);
            }
            return new List<CountryDropDownModel>();
        }

        public async Task<IActionResult> Testing(int cid =2)
        {
            var country = await GetCountryList();
            var state = await GetStatesByCountryID(cid);
            ViewBag.countrylist = country;

            ViewBag.statelist = state;
            return View("testing");
        }
    




    }
}
