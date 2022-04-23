using ModelsLib;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TeamApiService.Services
{
    public static class PersonService
    {
        public static async Task UpdateCurrentTeam(string id, string newTeam)
        {
            var httpClient = new HttpClient();

            if (httpClient.BaseAddress == null) httpClient.BaseAddress = new Uri("https://localhost:44379/api/People/");

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            await httpClient.PutAsync($"CurrentTeam/{id}/{newTeam}", null);
        }

        public static async Task<Person> Get(string id)
        {
            var httpClient = new HttpClient();
            if (httpClient.BaseAddress == null) httpClient.BaseAddress = new Uri($"https://localhost:44379/api/People/{id}");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.GetAsync(httpClient.BaseAddress);

            if (response.StatusCode != System.Net.HttpStatusCode.OK) return null;
            var responseBody = await response.Content.ReadAsStringAsync();
            return ExtractPersonJsonData(responseBody);
        }

        public static Person ExtractPersonJsonData(string jsonObject) => JsonConvert.DeserializeObject<Person>(jsonObject) ?? null;
    }
}
