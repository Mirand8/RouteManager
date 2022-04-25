using ModelsLib;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PersonApiService.Services
{
    public class TeamService
    {
        public static async Task UpdateMemberName(string personId, string newName = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (httpClient.BaseAddress == null) httpClient.BaseAddress = new Uri($"https://localhost:44340/api/Teams/");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                await httpClient.PutAsync($"UpdateMemberName/{personId}/{newName}", null);
            }
        }

        // PESSOA NÃO É REMOVIDA DO TIME AO SER EXCLUIDA!!
        public static async Task UpdateToRemoveMember(string id, Person person)
        {
            using (var httpClient = new HttpClient())
            {
                if (httpClient.BaseAddress == null) httpClient.BaseAddress = new Uri("https://localhost:44340/api/");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                var personAsJson = new JavaScriptSerializer().Serialize(person);
                var content = new StringContent(personAsJson, Encoding.UTF8, "application/json");
                await httpClient.PutAsJsonAsync($"Teams/RemoveMember/{id}", content);
            }
        }

        public static async Task<Team> GetByName(string name)
        {
            var httpClient = new HttpClient();
            if (httpClient.BaseAddress == null) httpClient.BaseAddress = new Uri($"https://localhost:44340/api/Teams/{name}");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.GetAsync(httpClient.BaseAddress);

            if (response.StatusCode != System.Net.HttpStatusCode.OK) return null;
            var responseBody = await response.Content.ReadAsStringAsync();
            return ExtractTeamJsonData(responseBody);
        }

        public static Team ExtractTeamJsonData(string jsonObject) => JsonConvert.DeserializeObject<Team>(jsonObject) ?? null;
    }
}
