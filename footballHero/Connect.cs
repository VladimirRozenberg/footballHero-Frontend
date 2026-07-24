using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace footballHero
{
    public class Connect
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://footballhero.ch")
        };

        public static void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public static void ClearAuthToken()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<T> GetAsync<T>(string url)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<T>();
                }
                Console.WriteLine($"Response: {response}");
                return default;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return default;
            }

        }
    }
}