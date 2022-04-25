using ModelsLib;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TeamApiService.Services
{
    public class CitiesService
    {
        public static async Task<City> Get(string id)
        {
            var httpClient = new HttpClient();
            if (httpClient.BaseAddress == null) httpClient.BaseAddress = new Uri($"https://localhost:44328/api/Cities/{id}");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.GetAsync(httpClient.BaseAddress);

            if (response.StatusCode != System.Net.HttpStatusCode.OK) return null;
            var responseBody = await response.Content.ReadAsStringAsync();
            return ExtractCityJsonData(responseBody);
        }

        public static City ExtractCityJsonData(string jsonObject) => JsonConvert.DeserializeObject<City>(jsonObject) ?? null;
    }
}
