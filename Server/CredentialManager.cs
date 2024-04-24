using Newtonsoft.Json;
using NowPlaying.Properties;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;

namespace NowPlaying.Server
{
    internal class CredentialManager
    {
        public static string SPOTIFY_AUTH_API_URL = "https://accounts.spotify.com";
        public static string SPOTIFY_AUTH_ENDPOINT = "/api/token";

        public static string SPOTIFY_API_URL = "https://api.spotify.com/v1/";
        public static string SPOTIFY_NOW_PLAYING_ENDPOINT = "me/player/currently-playing";

        public static (string?, string?) GetAccessToken(string code)
        {
            string EncodedCredentialString = EncodeB64($"{Settings.Default.SpotifyClientID}:{Settings.Default.SpotifyClientSecret}");

            HttpClient client = GetHttpClient(SPOTIFY_AUTH_API_URL);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", EncodedCredentialString);

            FormUrlEncodedContent formContent = new([
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", "http://localhost:5001/callback")
                ]);
            HttpRequestMessage HttpRequest = new(HttpMethod.Post, SPOTIFY_AUTH_ENDPOINT) {
                Content = formContent
        };
            HttpRequest.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded");
            HttpResponseMessage resp = client.Send(HttpRequest);

            using StreamReader r = new(resp.Content.ReadAsStream());
            string res = r.ReadToEnd();

            JSONObjects.APIResponse? respObj = JsonConvert.DeserializeObject<JSONObjects.APIResponse>(res);

            return (respObj!.access_token, respObj!.refresh_token);
        }

        public static void RefreshToken()
        {
            string EncodedCredentialString = EncodeB64($"{Settings.Default.SpotifyClientID}:{Settings.Default.SpotifyClientSecret}");

            HttpClient client = GetHttpClient(SPOTIFY_AUTH_API_URL);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", EncodedCredentialString);

            FormUrlEncodedContent formContent = new([
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", Settings.Default.SpotifyRefreshToken),
                new KeyValuePair<string, string>("client_id", Settings.Default.SpotifyClientID)
                ]);
            HttpRequestMessage HttpRequest = new(HttpMethod.Post, SPOTIFY_AUTH_ENDPOINT)
            {
                Content = formContent
            };
            HttpRequest.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded");
            HttpResponseMessage resp = client.Send(HttpRequest);

            using StreamReader r = new(resp.Content.ReadAsStream());
            string res = r.ReadToEnd();

            JSONObjects.APIResponse? respObj = JsonConvert.DeserializeObject<JSONObjects.APIResponse>(res);
            Settings.Default.SpotifyAccessToken = respObj?.access_token;
            Settings.Default.Save();
        }

        public static string? SendAPIRequestWithToken(string token)
        {
            HttpClient client = GetHttpClient(SPOTIFY_API_URL);
            client.DefaultRequestHeaders.Authorization = new("Bearer", token);

            HttpRequestMessage HttpRequest = new(HttpMethod.Get, SPOTIFY_NOW_PLAYING_ENDPOINT);

            HttpResponseMessage responseMessage = client.Send(HttpRequest);
            using StreamReader r = new(responseMessage.Content.ReadAsStream());
            return r.ReadToEnd();
        }

        public static JSONObjects.SpotifyNowPlaying GetSpotifyNowPlaying (int retryCount = 0)
        {
            string? response = SendAPIRequestWithToken(Settings.Default.SpotifyRefreshToken);
            JSONObjects.SpotifyNowPlaying? nowPlaying = JsonConvert.DeserializeObject<JSONObjects.SpotifyNowPlaying>(response!);
            if (nowPlaying?.error is not null && nowPlaying.error.status == 401 && retryCount < 5)
            {
                RefreshToken();
                GetSpotifyNowPlaying(retryCount + 1);
            }
            return nowPlaying!;
        }

        public static string EncodeB64(string s) => Convert.ToBase64String(Encoding.UTF8.GetBytes(s));

        public static string DecodeB64(string s) => Encoding.UTF8.GetString(Convert.FromBase64String(s));

        public static HttpClient GetHttpClient(string uri)
        {
            return new()
            {
                BaseAddress = new Uri(uri)
            };
        }

        public static string GenerateState()
        {
            return GenerateRandomString(16);
        }

        public static string GenerateRandomString(int length)
        {
            string lc = "abcdefghijklmnopqrstuvwxyz";
            string uc = "abcdefghijklmnopqrstuvwxyz".ToUpper();
            string allowedChars = lc + uc;

            return new(Enumerable.Repeat(allowedChars, length).Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        public static void HandleAuthorizationCallback(List<string> x)
        {
            Dictionary<string, string> callbackParameters = [];

            x.ForEach(e =>
            {
                if (e.Length < 1) return;
                if (e[0] == '?') e = e[1..e.Length];
                string[] q = e.Split('=');
                callbackParameters.Add(q[0], q[1]);
            });

            (string? AccessToken, string? RefreshToken) = CredentialManager.GetAccessToken(callbackParameters["code"]);
            Settings.Default.SpotifyAccessToken = AccessToken;
            Settings.Default.SpotifyRefreshToken = RefreshToken;
            Settings.Default.Save();
        }
    }
}
