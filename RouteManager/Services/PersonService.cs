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
    public class PersonService
    {
        readonly static string baseUri = "https://localhost:44379/api/";

        public static async Task<List<PersonViewModel>> Get()
        {
            var people = new List<PersonViewModel>();

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseUri);
                var response = await httpClient.GetAsync("People");
                if (response.IsSuccessStatusCode)
                    people = JsonConvert.DeserializeObject<List<PersonViewModel>>(response.Content.ReadAsStringAsync().Result);
            }
            return people;
        }


        public static async Task<PersonViewModel> Get(string id)
        {
            var people = new PersonViewModel();
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(baseUri);
                var response = await httpClient.GetAsync($"People/{id}");
                if (response.IsSuccessStatusCode)
                    people = JsonConvert.DeserializeObject<PersonViewModel>(response.Content.ReadAsStringAsync().Result);
            }
            return people;
        }


        public static async Task Create(PersonViewModel person)
        {
            using (var httpClient = new HttpClient())
            {
                if (httpClient.BaseAddress == null) httpClient.BaseAddress = new Uri(baseUri);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                await httpClient.PostAsJsonAsync("People", person);
            }
        }


        public static async Task<HttpResponseMessage> Update(string id, PersonViewModel person)
        {
            using (var httpClient = new HttpClient())
            {
                if (httpClient.BaseAddress == null) httpClient.BaseAddress = new Uri(baseUri);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                return await httpClient.PutAsJsonAsync($"People/{id}", person);
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
                return await httpClient.DeleteAsync($"People/{id}");
            }
        }
    }
}
