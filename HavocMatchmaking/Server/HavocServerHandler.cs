using CopperMatchmaking.Data;
using CopperMatchmaking.Info;
using CopperMatchmaking.Server;
using HavocMatchmaking.Utility;

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

    public override void LobbyCreated(CreatedLobby lobby)
    {
        var steamClients = new List<string>();
        lobby.LobbyClients.ForEach(client => steamClients.Add(client.PlayerId.ToString()));
        ApiUtil.SendLobbyToWebServer(lobby.LobbyId.ToString(), steamClients);
    }
}