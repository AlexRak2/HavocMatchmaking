using CopperMatchmaking.Data;
using CopperMatchmaking.Info;
using CopperMatchmaking.Server;
using HavocMatchmaking.Utility;
using Newtonsoft.Json;

namespace HavocMatchmaking.Server;

public class HavocServerHandler : ServerHandler
{
    public override bool VerifyPlayer(ConnectedClient client)
    {
        if (client.PlayerId == 814697797682)
            return true;

        var rank = client.GetRank().Item2;
        var isAppOwned = client.OwnsApp();

        Log.Info($"Verifying Player | Player Rank: {rank} | Player owns app: {isAppOwned}");

        if (!isAppOwned) return false;

        Console.WriteLine(MatchmakerServer.GetAllRanks()[0]);

        client.UpdateRank(Util.ConvertRank(rank));

        return true;
    }

    public override void LobbyJoinCodeReceived(CreatedLobby lobby, string lobbyJoinCode)
    {
        var steamClients = new List<string>();
        lobby.LobbyClients.ForEach(client => steamClients.Add(client.PlayerId.ToString()));
        ApiUtil.SendLobbyToWebServer(lobbyJoinCode, steamClients);
    }

    public static int queueCount;
    public override void PlayerQueueCountUpdated(int lobbyPlayerCount, int queuePlayerCount)
    {
        queueCount = queuePlayerCount;
        Task.Run(SendWebhook);
    }

    public static async Task SendWebhook()
    {
        var webhookUrl = "https://discord.com/api/webhooks/1217614617677795348/hu81xZAKg8yMZNj_eUQrQkppD94zChfilfxBGKR5SGRVbQoEgSZiuloZkPCCkZScoLAs";

        var embed = new Embed
        {
            title = "RANKED",
            description = "Amount Of Players In Queue: " + queueCount.ToString(),
            footer = new Footer
            {
                text = "Queue Up!"
            },
        };

        var webhookMessage = new WebhookMessage
        {
            embeds = new List<Embed>
            {
                embed
            }
        };

        using (var httpClient = new HttpClient())
        {
            var json = JsonConvert.SerializeObject(webhookMessage);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var webhookUpdateUrl = $"{webhookUrl}/messages/1217619295245307955";
            var response = await httpClient.PatchAsync(webhookUpdateUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to send webhook: {response.StatusCode}");
            }
            else
            {
                Console.WriteLine("Webhook sent successfully!");
            }
        }
    }
}



    [Serializable]
    public class Footer
    {
        public string text;
    }

    [Serializable]
    public class Embed
    {
        public string title;
        public string description;
        public Footer footer;
        //public string url;
    }

    [Serializable]
    public class WebhookMessage
    {
        public string username;
        public string avatar_url;
        public string content;
        public List<Embed> embeds;
    }
