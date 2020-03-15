using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BackupSaves.Utilities
{
    public class APIUtil
    {
        public class RootObj
        {
            public Response response { get; set; }
        }
        
        public class Response
        {
            public int game_count { get; set; }
            public List<Game> games { get; set; }
        }

        public class Game
        {
            public int appid { get; set; }
            public string name { get; set; }
            public int playtime_forever { get; set; }
            public string img_icon_url { get; set; }
            public string img_logo_url { get; set; }
            public bool has_community_visible_stats { get; set; }
            public int playtime_windows_forever { get; set; }
            public int playtime_mac_forever { get; set; }
            public int playtime_linux_forever { get; set; }
        }

        public static async Task<Response> GetSteamApps()
        {
            HttpClient _client = new HttpClient
            {
                BaseAddress = new Uri("https://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key=EA2DE8B677990527C7B93F095BF2A8C1&steamid=76561198022727943&include_appinfo=1")
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string response = await _client.GetStringAsync(_client.BaseAddress);

            RootObj obj = JsonConvert.DeserializeObject<RootObj>(response);

            return obj.response;
        }
    }
}
