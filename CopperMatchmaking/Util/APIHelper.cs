using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CopperMatchmaking.Util
{
    public static class APIHelper
    {
        public static async Task<string> QueryApi(string _url, Dictionary<string, object> parameters, string reqType)
        {
            string url = ConstructApiUrl(_url, parameters);
            string jsonBody = JsonConvert.SerializeObject(parameters);

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = reqType == "get" ? await client.GetAsync(url) : await client.GetAsync(url);

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

        static string ConstructApiUrl(string _url, Dictionary<string, object> parameters)
        {
            string url = _url + "?" + string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
            return url;
        }
    }
}
