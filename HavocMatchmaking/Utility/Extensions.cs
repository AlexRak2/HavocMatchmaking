using System.Text.Json;
using CopperMatchmaking.Data;

namespace HavocMatchmaking.Utility;

public static class Extensions
{
    public static bool OwnsApp(this ConnectedClient client)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "key", SteamApiUtil.STEAM_PUBLISHER_WEB_API_KEY },
                { "steamid", client.PlayerId.ToString() },
                { "appid", SteamApiUtil.STEAM_APP_ID }
            };

            var response = SteamApiUtil.QueryApi("ISteamUser/CheckAppOwnership/v2/", parameters).GetAwaiter().GetResult();

            var jsonObject = JsonDocument.Parse(response).RootElement;
            var ownsApp = jsonObject.GetProperty("appownership").GetProperty("ownsapp").GetBoolean();

            return ownsApp;
        }
        catch (Exception ex)
        {
            //Returns false if steam if is not valid, will be better once we authenticate user first
            Console.WriteLine($"Exception during Steam API verification: {ex.Message}");
            return false;
        }
    }

    public static (bool, int) GetRank(this ConnectedClient client)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "key", SteamApiUtil.STEAM_PUBLISHER_WEB_API_KEY },
                { "steamid", client.PlayerId.ToString() },
                { "appid", SteamApiUtil.STEAM_APP_ID },
                { "uncache", Program.Random.Next(0, 1000) }
            };

            var response = SteamApiUtil.QueryApi("ISteamUserStats/GetUserStatsForGame/v2/", parameters).GetAwaiter().GetResult();

            var jsonObject = JsonDocument.Parse(response).RootElement;
            var statsArray = jsonObject.GetProperty("playerstats").GetProperty("stats");
            var rankStat = statsArray.EnumerateArray().FirstOrDefault(item => item.TryGetProperty("name", out var name) && name.GetString() == "rank");
            var rank = rankStat.GetProperty("value").GetInt32();
            Console.WriteLine("Rank: " + rank);


            return (true, rank);
        }
        catch (Exception ex)
        {
            //Returns false if steam if is not valid, will be better once we authenticate user first
            Console.WriteLine($"Exception during Steam API verification: {ex.Message}");
            return (false, 0);
        }
    }
}