using Microsoft.Graph.Models;
using NowPlaying.Core;
using NowPlaying.Properties;
using NowPlaying.Server;
using NowPlaying.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static System.Formats.Asn1.AsnWriter;

namespace NowPlaying.ViewModels
{
    class SpotifySettingsViewModel : ObservableObject
    {
        public string ClientID { get => Settings.Default.SpotifyClientID; set { Settings.Default.SpotifyClientID = value; OnPropertyChanged(); } }
        public string ClientSecret { get => Settings.Default.SpotifyClientSecret; set { Settings.Default.SpotifyClientSecret = value; OnPropertyChanged(); } }

        public RelayCommand SaveButtonCommand { get; set; }
        public RelayCommand ClearButtonCommand { get; set; }
        public RelayCommand AuthorizeButtonCommand { get; set; }
        public RelayCommand ClearAuthorizationButtonCommand { get; set; }


        public SpotifySettingsViewModel()
        {
            SaveButtonCommand = new((o) => { SaveProperties(); });
            ClearButtonCommand = new((o) => { ClearProperties(); });
            AuthorizeButtonCommand = new((o) => { AuthorizeClient(); });
            ClearAuthorizationButtonCommand = new((o) => {
                Settings.Default.SpotifyAccessToken = null;
                Settings.Default.SpotifyRefreshToken = null;
                SaveProperties();
            });

        }

        public void AuthorizeClient()
        {
            string queryString = "";
            queryString += "response_type=code&";
            queryString += $"client_id={Settings.Default.SpotifyClientID}&";
            queryString += "scope=user-read-currently-playing&";
            queryString += $"redirect_uri=http%3A%2F%2Flocalhost%3A{Settings.Default.SourcePort}%2Fcallback&";
            queryString += $"state={CredentialManager.GenerateState()}";

            string requestString = "https://accounts.spotify.com/authorize?" + queryString;
            Util.OpenUri(requestString);
        }


        public void SaveProperties()
        {
            Settings.Default.Save();
        }
        public void ClearProperties()
        {
            ClientID = "";
            ClientSecret = "";
            SaveProperties();
        }
    }
}
