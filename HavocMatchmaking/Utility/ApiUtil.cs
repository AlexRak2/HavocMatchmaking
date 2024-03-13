using System.Text;
using Newtonsoft.Json;

namespace HavocMatchmaking.Utility
{
    public static class ApiUtil
    {
        public static async Task<string> QueryApi(string url, Dictionary<string, object> parameters, string reqType)
        {
            var apiUrl = ConstructApiUrl(url, parameters);
            var jsonBody = JsonConvert.SerializeObject(parameters);

            using var client = new HttpClient();

            try
            {
                var response = reqType == "get" ? await client.GetAsync(apiUrl) : await client.PostAsync(url, new StringContent(jsonBody, Encoding.UTF8, "application/json"));

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

        private static string ConstructApiUrl(string url, Dictionary<string, object> parameters)
        {
            return url + "?" + string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
        }

        internal static void SendLobbyToWebServer(string hostedLobbyId, List<string> steamIds)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "key", SteamApiUtil.SECRET_KEY },
                    { "lobby_id", hostedLobbyId },
                    { "clients", steamIds }
                };

                var response = ApiUtil.QueryApi(SteamApiUtil.LOBBY_ENDPOINT, parameters, "post").GetAwaiter().GetResult();

                foreach (var client in steamIds)
                    Console.WriteLine(client);
                Console.WriteLine($"Sent lobby to web server {response}");
            }
            catch (Exception ex)
            {
                //Returns false if steam if is not valid, will be better once we authenticate user first
                Console.WriteLine($"Exception during Steam API verification: {ex.Message}");
            }
        }
    }
}