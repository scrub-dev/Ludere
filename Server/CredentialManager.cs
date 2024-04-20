using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;

namespace NowPlaying.Server
{
    internal class CredentialManager
    {
        public static string SPOTIFY_AUTH_API_URL = "https://accounts.spotify.com";
        public static string SPOTIFY_AUTH_ENDPOINT = "/api/token";

        public static string TESTING_URL = "https://dummy.restapiexample.com/api/v1";
        public static string TESTING_ENDPOINT = "/employee/1";
        public static (string?, DateTime?, string?) GetAccessToken(string clientID, string clientSecret)
        {
            string EncodedCredentialString = EncodeB64($"{clientID}:{clientSecret}");

            HttpClient client = GetHttpClient(SPOTIFY_AUTH_API_URL);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", EncodedCredentialString);
            
            FormUrlEncodedContent formContent = new([new KeyValuePair<string, string>("grant_type", "client_credentials")]);
            HttpRequestMessage HttpRequest = new(HttpMethod.Post, SPOTIFY_AUTH_ENDPOINT) {
                Content = formContent
            };
            
            HttpResponseMessage resp = client.Send(HttpRequest);

            if (!resp.IsSuccessStatusCode)
            {
                APIErrorResponse? errorObj = JsonConvert.DeserializeObject<APIErrorResponse>(new StreamReader(resp.Content.ReadAsStream()).ReadToEnd());
                return (null, null, $"Spotify {errorObj?.error_description}");
            }

            using StreamReader r = new(resp.Content.ReadAsStream());
            string res = r.ReadToEnd();

            APIResponse? respObj = JsonConvert.DeserializeObject<APIResponse>(res);
            Debug.Write(res);
            Debug.Write(respObj?.ToString());

            return (respObj!.access_token, DateTime.Now.AddSeconds(respObj!.expires_in - 60), null);
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

        class APIResponse
        {
            public required string access_token { get; set; }
            public required string token_type { get; set; }
            public required int expires_in { get; set; }
        }
        class APIErrorResponse
        {
            public required string error { get; set; }
            public required string error_description { get; set; }
        }
    }
}
