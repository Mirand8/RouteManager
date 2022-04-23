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
    public class TeamService
    {
        readonly static string _baseUri = "https://localhost:44340/api/";

        public static async Task<IEnumerable<TeamViewModel>> Get()
        {
            var teams = new List<TeamViewModel>();
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_baseUri);
                var response = await httpClient.GetAsync("Teams");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    teams = JsonConvert.DeserializeObject<List<TeamViewModel>>(response.Content.ReadAsStringAsync().Result);
            }
            return teams;
        }

        public static async Task<TeamViewModel> Get(string id)
        {
            var teams = new TeamViewModel();
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_baseUri);
                var response = await httpClient.GetAsync($"Teams/{id}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    teams = JsonConvert.DeserializeObject<TeamViewModel>(response.Content.ReadAsStringAsync().Result);
            }

            return teams;
        }

        public static async Task Create(TeamViewModel team)
        {
            using (var httpClient = new HttpClient())
            {
                if (httpClient.BaseAddress == null) httpClient.BaseAddress = new Uri(_baseUri);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                await httpClient.PostAsJsonAsync("Teams", team);
            }
        }

        public static async Task<HttpResponseMessage> Update(string id, TeamViewModel team)
        {
            using (var httpClient = new HttpClient())
            {
                if (httpClient.BaseAddress == null) httpClient.BaseAddress = new Uri(_baseUri);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                return await httpClient.PutAsJsonAsync($"Teams/{id}", team);
            }
        }

        public static async Task<HttpResponseMessage> UpdateToAddMember(string id, PersonViewModel person)
        {
            using (var httpClient = new HttpClient())
            {
                if (httpClient.BaseAddress == null) httpClient.BaseAddress = new Uri(_baseUri);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                return await httpClient.PutAsJsonAsync($"Teams/{id}/AddNewMember", person);
            }
        }

        public static async Task<HttpResponseMessage> UpdateToRemoveMember(string id, PersonViewModel person)
        {
            using (var httpClient = new HttpClient())
            {
                if (httpClient.BaseAddress == null) httpClient.BaseAddress = new Uri(_baseUri);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                return await httpClient.PutAsJsonAsync($"Teams/{id}/RemoveMember", person);
            }
        }

        public static async Task<HttpResponseMessage> Delete(string id)
        {
            using (var httpClient = new HttpClient())
            {
                if (httpClient.BaseAddress == null) httpClient.BaseAddress = new Uri(_baseUri);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                return await httpClient.DeleteAsync($"Teams/{id}");
            }
        }

    }
}
