using CopperMatchmaking.Components;
using CopperMatchmaking.Data;
using CopperMatchmaking.Server;
using HavocMatchmaking.Data;
using HavocMatchmaking.Server;

namespace HavocMatchmaking;

public static class Program
{
    internal static readonly Random Random = new(Guid.NewGuid().GetHashCode());

    public static void Main()
    {
        var server = new MatchmakerServer(new HavocServerHandler(), GetLobbySize())
        {
            new LobbyTimeoutComponent(),
            new InfoLoggerComponent()
        };

        server.RegisterRanks(
            new Rank("Unranked", RankIds.Unranked), // 0
            new Rank("Bronze", RankIds.Bronze), // 1
            new Rank("Silver", RankIds.Silver), // 2
            new Rank("Gold", RankIds.Gold), // 3
            new Rank("Platinum", RankIds.Platinum), // 4
            new Rank("Diamond", RankIds.Diamond), // 5
            new Rank("Master", RankIds.Master), // 6
            new Rank("Chaos", RankIds.Chaos)); // 7

        while (true)
        {
            server.Update();
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private static byte GetLobbySize(byte defaultValue = 10)
    {
        return byte.TryParse((Environment.GetEnvironmentVariable("SERVER_LOBBY_SIZE") ?? "10"), out var parseResult) ? parseResult : defaultValue;
    }
}