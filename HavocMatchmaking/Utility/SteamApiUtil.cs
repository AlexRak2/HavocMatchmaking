namespace HavocMatchmaking.Utility;

internal static class SteamApiUtil
{
    // ReSharper disable InconsistentNaming
    public static readonly string STEAM_API_URL = Environment.GetEnvironmentVariable("STEAM_API_URL") ?? "https://partner.steam-api.com/";
    public static readonly string STEAM_APP_ID = Environment.GetEnvironmentVariable("STEAM_APP_ID") ?? "480";
    public static readonly string STEAM_PUBLISHER_WEB_API_KEY = Environment.GetEnvironmentVariable("STEAM_PUBLISHER_WEB_API_KEY") ?? "";
    public static readonly string SECRET_KEY = Environment.GetEnvironmentVariable("SECRET_KEY") ?? "";
    public static readonly string LOBBY_ENDPOINT = Environment.GetEnvironmentVariable("LOBBY_ENDPOINT") ?? ""; 
    // ReSharper restore InconsistentNaming

    public static async Task<string> QueryApi(string endPoint, Dictionary<string, object> parameters)
    {
        var url = ConstructApiUrl(endPoint, parameters);

        using var client = new HttpClient();
            
        try
        {
            var response = await client.GetAsync(url);

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

    private static string ConstructApiUrl(string endPoint, Dictionary<string, object> parameters)
    {
        return STEAM_API_URL + endPoint + "?" + string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
    }
}