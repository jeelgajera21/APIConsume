using APIConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace APIConsume.Controllers
{
    public class StateController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5163/api");
        private readonly HttpClient _client;

        public StateController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
       
        [HttpGet]
        public IActionResult StateList()
        {
            List<StateModel> state = new List<StateModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/State").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                /*  dynamic jsonObject = JsonConvert.DeserializeObject(data);*/

                state = JsonConvert.DeserializeObject<List<StateModel>>(data);
            }

            return View("StateList", state);
        }

        [HttpGet("{StateID}")]
        [Route("soooo")]

        public IActionResult StateAddEdit(int? StateID)
        {
            StateModel statebyid = null;

            if (StateID != null)
            {
                HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/State/{StateID}").Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;

                    // Deserialize the data as a list of CityModel
                    var states = JsonConvert.DeserializeObject<List<StateModel>>(data);

                    // Get the first city from the list if it exists
                    statebyid = states?.FirstOrDefault();
                }

                return View("StateAddEdit", statebyid);
            }
            return View("StateAddEdit", new StateModel());

        }




        [HttpPost]
        public async Task<IActionResult> Save(StateModel state)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(state);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (state.StateID == null)
                    response = await _client.PostAsync($"{_client.BaseAddress}/State", content);
                else
                    response = await _client.PutAsync($"{_client.BaseAddress}/State", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("StateList");
            }
           
            return View("StateAddEdit", state);
        }

        public async Task<IActionResult> Delete(int StateID)
        {
            var response = await _client.DeleteAsync($"{_client.BaseAddress}/State/Delete?StateID={StateID}");
            return RedirectToAction("StateList");
        }

    }
}
