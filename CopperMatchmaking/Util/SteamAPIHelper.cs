using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CopperMatchmaking.Util
{
    internal class SteamAPIHelper
    {
        public static string STEAM_API_URL = Environment.GetEnvironmentVariable("STEAM_API_URL") ?? "https://partner.steam-api.com/";
        public static string STEAM_APP_ID = Environment.GetEnvironmentVariable("STEAM_APP_ID") ?? "480";
        public static string STEAM_PUBLISHER_WEB_API_KEY = Environment.GetEnvironmentVariable("STEAM_PUBLISHER_WEB_API_KEY") ?? "";
        public static string SECRET_KEY = Environment.GetEnvironmentVariable("SECRET_KEY") ?? "";
        public static string LOBBY_ENDPOINT = Environment.GetEnvironmentVariable("LOBBY_ENDPOINT") ?? ""; 

        public static async Task<string> QueryApi(string endPoint, Dictionary<string, object> parameters)
        {
            string url = ConstructApiUrl(endPoint, parameters);

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    return "";
                }

                return "";
            }
        }

        static string ConstructApiUrl(string endPoint, Dictionary<string, object> parameters)
        {
            string url = STEAM_API_URL + endPoint + "?" + string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
            return url;
        }
    }
}
