using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.IO;
using static System.Environment; 



namespace footballHero.Services
{
    
    public static class TokenStorage
    {
        private static readonly string FilePath =
            Path.Combine(GetFolderPath(SpecialFolder.ApplicationData), "footballHeroToken.txt");

        public static void SaveToken(string token)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            File.WriteAllText(FilePath, token);
        }

        public static string LoadToken()
        {
            if (!File.Exists(FilePath)) return null;
            return File.ReadAllText(FilePath);
        }

        public static void DeleteToken()
        {
            if (File.Exists(FilePath)) File.Delete(FilePath);
        }
    }
    
    public class ConnectToFastApi
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://footballhero.ch")
        };
        public async Task<T> GetAsync<T>(string url)
        {
            try
            {
                var token = TokenStorage.LoadToken();

                // Build the request manually so we can attach the Authorization header
                using var request = new HttpRequestMessage(HttpMethod.Get, url);

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<T>();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("401 Unauthorized — token expired or missing");
                    TokenStorage.DeleteToken(); // optional: force re-login
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

        public async Task<TResponse> PostAsync<TResponse, TRequest>(string url, TRequest data)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(url, data);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<TResponse>();
                    if (result == null)
                    {
                        throw new Exception("The server returned an empty or unparsable response.");
                    }
                    return result;
                }

                // Read the error message from the backend if available
                string errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Server returned {(int)response.StatusCode} ({response.ReasonPhrase}). Details: {errorContent}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[PostAsync Error] {e}");
                throw;
            }
        }
    }
}