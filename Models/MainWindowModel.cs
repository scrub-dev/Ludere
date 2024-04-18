using System.Windows.Controls;

namespace NowPlaying.Models
{
    class MainWindowModel
    {
        private readonly Dictionary<string, Type> _views = [];

        public MainWindowModel ()
        {
            _views.Add("home", typeof(Views.HomeView));
            _views.Add("spotify_settings", typeof(Views.SpotifySettingsView));
            _views.Add("server_settings", typeof(Views.ServerSettingsView));
            _views.Add("style_settings", typeof(Views.StyleSettingsView));
            _views.Add("custom_styles", typeof(Views.CustomStyleView));
        }
        private Type GetViewType(string s)
        {
            Type newType;
            return _views.TryGetValue(s, out newType!) ? newType : _views.First().Value;
        }
        public UserControl GetFrameContent(string frameName) => (UserControl)Activator.CreateInstance(GetViewType(frameName))! ?? new Views.HomeView();
    }
}
