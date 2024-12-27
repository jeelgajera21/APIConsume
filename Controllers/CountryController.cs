using APIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace APIConsume.Controllers
{
    public class CountryController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5163/api");
        private readonly HttpClient _client;

        public CountryController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        
        [HttpGet]
        public IActionResult CountryList()
        {
            List<CountryModel> country = new List<CountryModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Country").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                /*  dynamic jsonObject = JsonConvert.DeserializeObject(data);*/

                country = JsonConvert.DeserializeObject<List<CountryModel>>(data);
            }

            return View("CountryList", country);
        }
        [HttpGet("{CountryID}")]
        [Route("hooh")]

        public IActionResult CountryAddEdit(int? CountryID)
        {
            CountryModel countrybyid = null;

            if (CountryID != null)
            {
                HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/Country/{CountryID}").Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;

                    // Deserialize the data as a list of CityModel
                    var countries = JsonConvert.DeserializeObject<List<CountryModel>>(data);

                    // Get the first city from the list if it exists
                    countrybyid = countries?.FirstOrDefault();
                }

                return View("CountryAddEdit", countrybyid);
            }
            return View("CountryAddEdit", new CountryModel());

        }




        [HttpPost]
        public async Task<IActionResult> Save(CountryModel country)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(country);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (country.CountryID== null)
                    response = await _client.PostAsync($"{_client.BaseAddress}/Country", content);
                else
                    response = await _client.PutAsync($"{_client.BaseAddress}/Country", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("CountryList");
            }
           
            return View("CountryAddEdit", country);
        }

        public async Task<IActionResult> Delete(int CountryID)
        {
            var response = await _client.DeleteAsync($"{_client.BaseAddress}/Country/?CountryID={CountryID}");
            return RedirectToAction("CountryList");
        }


    }
}
