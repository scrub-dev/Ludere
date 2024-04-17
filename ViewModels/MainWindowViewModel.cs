using NowPlaying.Core;
using NowPlaying.Models;
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
        // View
        private readonly MainWindowModel mainWindowModel = new();

        #region Private Values
        private UserControl? _currentView;
        private string? _currentState;
        #endregion

        #region Public Values


        public UserControl CurrentView { get => _currentView!; set{ _currentView = value; OnPropertyChanged();}}
        #endregion

        #region Commands
        public RelayCommand? SetViewCommand { get; set; }
        public RelayCommand? CloseApplicationCommand { get; set; }
        public RelayCommand? HelpButtonCommand { get; set; }
        #endregion


        #region Server Bindings
        public RelayCommand? StartServerCommand { get; set; }
        public RelayCommand? StopServerCommand { get; set; }
        public string CurrentState { get => (_currentState is not null) ? _currentState : "Offline"; set => _currentState = value; }

        #endregion

        public MainWindowViewModel ()
        {

            SetFrameContent("home");

            CloseApplicationCommand = new((o) => Application.Current.Shutdown());
            HelpButtonCommand = new((o) => Util.OpenUri(Properties.Settings.Default.HelpURI));
            SetViewCommand = new((o) => SetFrameContent((string)o));

        }

        private void SetFrameContent (string newFrameName)
        {
            CurrentView = mainWindowModel.GetFrameContent(newFrameName);
        }
    }
} 
