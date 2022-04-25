using ModelsLib;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CitiesApiService.Services
{
    public class TeamService
    {
        public static async Task<IEnumerable<Team>> GetTeamsByCity(string cityId)
        {
            var httpClient = new HttpClient();
            if (httpClient.BaseAddress == null) httpClient.BaseAddress = new Uri($"https://localhost:44340/api/Teams/City/{cityId}");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.GetAsync(httpClient.BaseAddress);

            if (response.StatusCode != System.Net.HttpStatusCode.OK) return null;
            var responseBody = await response.Content.ReadAsStringAsync();
            return ExtractTeamJsonData(responseBody);
        }

        // ****
        public static async void Update(string id, City city)
        {
            var httpClient = new HttpClient();
            await httpClient.PutAsJsonAsync($"https://localhost:44340/api/Teams/City/{id}", city);
        }

        public static List<Team> ExtractTeamJsonData(string jsonObject) => JsonConvert.DeserializeObject<List<Team>>(jsonObject) ?? null;
    }
}
