using NowPlaying.Core;
using NowPlaying.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NowPlaying.ViewModels
{
    internal class MainWindowViewModel : ObservableObject
    {

        #region Private Values
        private object? _currentView;
        private string? _currentState;
        #endregion
        #region Public Values
        public string CurrentState { get => (_currentState is not null) ? _currentState : "Offline"; set => _currentState = value; }

        #endregion
        #region Commands
        public RelayCommand? SetViewCommandCommand { get; set; }
        public RelayCommand? CloseApplicationCommand { get; set; }

        public RelayCommand? HelpButtonCommand { get; set; }

        #endregion

        public MainWindowViewModel ()
        {
            CloseApplicationCommand = new((o) => Application.Current.Shutdown());
            HelpButtonCommand = new((o) => Util.OpenUri(Properties.Settings.Default.HelpURI));
        }
    }
}
