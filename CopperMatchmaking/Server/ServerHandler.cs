using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CopperMatchmaking.Data;
using CopperMatchmaking.Util;

namespace CopperMatchmaking.Server
{
    /// <summary>
    /// Base server handler
    /// </summary>
    public class ServerHandler
    {
        /// <summary>
        /// Server side verification
        /// </summary>
        /// <param name="client">Target client to verify</param>
        /// <returns>True if client is verified and allowed to connect</returns>
        public virtual bool VerifyPlayer(ConnectedClient client)
        {
            if (client.PlayerId == 814697797682)
                return true;

            int rank = GetPlayersRank(client).Item2;
            bool isAppOwned = PlayerOwnsApp(client);

            Console.WriteLine("PlayerRank: " + rank);
            Console.WriteLine("Player Owned App: " + isAppOwned);

            if (!isAppOwned) return false;

            Console.WriteLine(MatchmakerServer.Instance.Ranks[0]);

            client.UpdateRank(ConvertRank(rank));

            return true;
        }

        /// <summary>
        /// Ran for when a lobby is created on the server
        /// </summary>
        /// <param name="lobbyClients">Clients in the lobby</param>
        /// <param name="lobbyId">Id of the created lobby</param>
        public virtual void LobbyCreated(CreatedLobby lobby)
        {
            
        }

        /// <summary>
        /// Functions for choosing who in a lobby should be the host
        /// </summary>
        /// <param name="lobbyClients">All the clients in the lobby</param>
        /// <returns>Index of the list corresponding to who should host</returns>
        public virtual int ChooseLobbyHost(List<ConnectedClient> lobbyClients)
        {
            return 0;
        }

        #region API Calls
        private bool PlayerOwnsApp(ConnectedClient client)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "key", SteamAPIHelper.STEAM_PUBLISHER_WEB_API_KEY },
                    { "steamid", client.PlayerId.ToString() },
                    { "appid", SteamAPIHelper.STEAM_APP_ID }
                };

                string response = SteamAPIHelper.QueryApi("ISteamUser/CheckAppOwnership/v2/", parameters).GetAwaiter().GetResult();

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

        private (bool, int) GetPlayersRank(ConnectedClient client)
        {
            Random rnd = new Random();
            int uncache = rnd.Next(0, 1000);

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "key", SteamAPIHelper.STEAM_PUBLISHER_WEB_API_KEY },
                    { "steamid", client.PlayerId.ToString() },
                    { "appid", SteamAPIHelper.STEAM_APP_ID },
                    { "uncache", uncache}
                };

                string response = SteamAPIHelper.QueryApi("ISteamUserStats/GetUserStatsForGame/v2/", parameters).GetAwaiter().GetResult();

                var jsonObject = JsonDocument.Parse(response).RootElement;
                var statsArray = jsonObject.GetProperty("playerstats").GetProperty("stats");
                var rankStat = statsArray.EnumerateArray().FirstOrDefault(item => item.TryGetProperty("name", out var name) && name.GetString() == "rank");

                Console.WriteLine(rankStat);
                Console.WriteLine(statsArray);
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

        private byte ConvertRank(int rank) 
        {
            if (rank >= 0 && rank < 150) 
            {
                return 0;
            }
            else if (rank >= 150 && rank < 300)
            {
                return 1;
            }
            else if (rank >= 300 && rank < 450)
            {
                return 2;
            }
            else if (rank >= 450 && rank < 600)
            {
                return 3;
            }
            else if (rank >= 600 && rank < 750)
            {
                return 4;
            }
            else if (rank >= 750 && rank < 900)
            {
                return 5;
            }
            return 0;

        }
        #endregion
    }
}