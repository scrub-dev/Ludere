using NowPlaying.Core;
using NowPlaying.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NowPlaying.ViewModels
{
    internal class MainWindowViewModel : ObservableObject
    {

        #region Private Values
        private UserControl? _currentView;
        private string? _currentState;
        private readonly Dictionary<string, Type> _views = [];
        #endregion

        #region Public Values
        public string CurrentState { get => (_currentState is not null) ? _currentState : "Offline"; set => _currentState = value; }


        public UserControl CurrentView { get => _currentView!; set{ _currentView = value; OnPropertyChanged();}}
        #endregion

        #region Commands
        public RelayCommand? SetViewCommand { get; set; }
        public RelayCommand? CloseApplicationCommand { get; set; }
        public RelayCommand? HelpButtonCommand { get; set; }

        #endregion

        public MainWindowViewModel ()
        {
            _views.Add("home", typeof(Views.HomeView));
            _views.Add("spotify_settings", typeof(Views.SpotifySettingsView));
            _views.Add("server_settings", typeof(Views.ServerSettingsView));
            _views.Add("style_settings", typeof(Views.StyleSettingsView));
            _views.Add("custom_styles", typeof(Views.CustomStyleView));

            SetFrameContent("home");

            CloseApplicationCommand = new((o) => Application.Current.Shutdown());
            HelpButtonCommand = new((o) => Util.OpenUri(Properties.Settings.Default.HelpURI));
            SetViewCommand = new((o) => SetFrameContent((string)o));

            bool test = Util.DoesPropertyExist("HelpURI");

        }

        private void SetFrameContent(string frameLookupString) 
        {
            Type viewType = GetViewType(frameLookupString);
            UserControl FrameToNavigateTo = (UserControl)Activator.CreateInstance(viewType)! ?? new Views.HomeView();
            CurrentView = FrameToNavigateTo;
        }

        private Type GetViewType(string s)
        {
            Type newType;
            return _views.TryGetValue(s, out newType!) ? newType : _views.First().Value;
        }
    }
} 
