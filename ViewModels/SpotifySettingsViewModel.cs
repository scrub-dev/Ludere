using NowPlaying.Core;
using NowPlaying.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowPlaying.ViewModels
{
    class SpotifySettingsViewModel : ObservableObject
    {
        public string ClientID { get => Settings.Default.SpotifyClientID; set { Settings.Default.SpotifyClientID = value; OnPropertyChanged(); } }
        public string ClientSecret { get => Settings.Default.SpotifyClientSecret; set { Settings.Default.SpotifyClientSecret = value; OnPropertyChanged(); } }


        public RelayCommand SaveButtonCommand { get; set; }
        public RelayCommand ClearButtonCommand { get; set; }

        public SpotifySettingsViewModel()
        {
            SaveButtonCommand = new((o) => { SaveProperties(); });
            ClearButtonCommand = new((o) => { ClearProperties(); });
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
