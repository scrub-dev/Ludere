using NowPlaying.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowPlaying.Utility
{
    internal class FeatureHandler
    {
        [Flags]
        public enum Feature
        {
            SONG_NAME = 1 << 1,
            ARTIST_NAME = 1 << 2,
            ALBUM_NAME = 1 << 3,
            ARTWORK = 1 << 4,
            SONG_PROGRESSION = 1 << 5,
            NOT_PLAYING_SHOWN = 1 << 6,
        }
        public static void ToggleFeature(Feature f, bool val)
        {
            if (val) EnableFeature(f);
            else DisabledFeature(f);
        }
        public static void DisabledFeature(Feature f)
        {
            Settings.Default.EnabledFeatures &= ~(int)f;
            Settings.Default.Save();
        }
        public static void EnableFeature(Feature f)
        {
            Settings.Default.EnabledFeatures |= (int)f;
            Settings.Default.Save();
        }
        public static bool IsFeatureEnabled(Feature f) => (Settings.Default.EnabledFeatures & (int)f) != 0;
    }
}
