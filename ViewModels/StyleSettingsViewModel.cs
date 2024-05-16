using Microsoft.Graph;
using NowPlaying.Core;
using NowPlaying.Properties;
using NowPlaying.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowPlaying.ViewModels
{
    internal class StyleSettingsViewModel : ObservableObject
    {

        public bool SongProgressionChecked { get => FeatureHandler.IsFeatureEnabled(FeatureHandler.Feature.SONG_PROGRESSION); set { FeatureHandler.ToggleFeature(FeatureHandler.Feature.SONG_PROGRESSION, value) ; OnPropertyChanged(); } }
        public bool ArtworkChecked { get => FeatureHandler.IsFeatureEnabled(FeatureHandler.Feature.ARTWORK); set { FeatureHandler.ToggleFeature(FeatureHandler.Feature.ARTWORK, value); OnPropertyChanged(); } }
        public bool ArtistChecked { get => FeatureHandler.IsFeatureEnabled(FeatureHandler.Feature.ARTIST_NAME); set { FeatureHandler.ToggleFeature(FeatureHandler.Feature.ARTIST_NAME, value); OnPropertyChanged(); } }
        public bool AlbumChecked { get => FeatureHandler.IsFeatureEnabled(FeatureHandler.Feature.ALBUM_NAME); set { FeatureHandler.ToggleFeature(FeatureHandler.Feature.ALBUM_NAME, value); OnPropertyChanged(); } }
        public bool SongNameChecked { get => FeatureHandler.IsFeatureEnabled(FeatureHandler.Feature.SONG_NAME); set { FeatureHandler.ToggleFeature(FeatureHandler.Feature.SONG_NAME, value); OnPropertyChanged(); } }
        public bool NotPlayingChecked { get => FeatureHandler.IsFeatureEnabled(FeatureHandler.Feature.NOT_PLAYING_SHOWN); set { FeatureHandler.ToggleFeature(FeatureHandler.Feature.NOT_PLAYING_SHOWN, value); OnPropertyChanged(); } }


        public int OutputHeight { get => Settings.Default.DisplayHeight ; set { Settings.Default.DisplayHeight = value; Settings.Default.Save(); OnPropertyChanged(); } }
        public int OutputWidth { get => Settings.Default.DisplayWidth; set { Settings.Default.DisplayWidth = value; Settings.Default.Save();  OnPropertyChanged(); } }
    }
}
