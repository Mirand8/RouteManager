using Newtonsoft.Json;
using RouteManager.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace RouteManager.Services
{
    public class CityService
    {
        readonly static string baseUri = "https://localhost:44328/api/";

        public static async Task<List<CityViewModel>> Get()
        {
            var cities = new List<CityViewModel>();
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseUri);
                var response = await httpClient.GetAsync("Cities");
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    cities = JsonConvert.DeserializeObject<List<CityViewModel>>(responseBody);
                }
            }

            return cities;
        }

        public static async Task<CityViewModel> Get(string id)
        {
            var city = new CityViewModel();

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseUri);
                var response = await httpClient.GetAsync($"Cities/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    city = JsonConvert.DeserializeObject<CityViewModel>(responseBody);
                }
            }

            return city;
        }

        public static async Task<CityViewModel> GetByName(string name)
        {
            var city = new CityViewModel();

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseUri);

                var response = await httpClient.GetAsync($"Cities/{name}");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    city = JsonConvert.DeserializeObject<CityViewModel>(responseBody);
                }
                else return null;
            }

            return city;
        }

        public static async Task<HttpResponseMessage> Create(CityViewModel city)
        {
            var httpClient = new HttpClient();
            if (httpClient.BaseAddress == null) httpClient.BaseAddress = new Uri(baseUri);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.PostAsJsonAsync("Cities", city);
            return response;
        }

        public static async Task<HttpResponseMessage> Update(string id, CityViewModel city)
        {
            using (var httpClient = new HttpClient())
            {
                if (httpClient.BaseAddress == null) httpClient.BaseAddress = new Uri(baseUri);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                return await httpClient.PutAsJsonAsync($"Cities/{id}", city);
            }
        }

        public static async Task<HttpResponseMessage> Delete(string id)
        {
            using (var httpClient = new HttpClient())
            {
                if (httpClient.BaseAddress == null) httpClient.BaseAddress = new Uri(baseUri);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                return await httpClient.DeleteAsync($"Cities/{id}");
            }
        }
    }
}
