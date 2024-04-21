using NowPlaying.Core;
using NowPlaying.Properties;
using NowPlaying.Server;
using NowPlaying.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace NowPlaying.ViewModels
{
    internal class ServerSettingsViewModel: ObservableObject
    {

        public string? _errorText;
        public string ErrorText { get => _errorText!; set { _errorText = value; OnPropertyChanged(); }}

        public int ServerRefreshInterval { get => Settings.Default.ServerRefreshInterval; set { Settings.Default.ServerRefreshInterval = value; OnPropertyChanged(); } }

        public int HostPort { get => Settings.Default.SourcePort!; set { Settings.Default.SourcePort = value; OnPropertyChanged(); } }

        public int UpdateServicePort { get => Settings.Default.SocketPort!; set { Settings.Default.SocketPort = value; OnPropertyChanged(); } }

        public RelayCommand SaveButtonCommand { get; set; }

        public ServerSettingsViewModel()
        {
            SaveButtonCommand = new((o) => {
                RunValidation();
            });
        }

        public void OnTextChanged()
        {
            Util.SetErrorUI("", s => ErrorText = s);
        }

        public void RunValidation()
        {
            ArrayList ErrorList = [];

            if (!PropertyValidator.IsRefreshIntervalValid(ServerRefreshInterval)) ErrorList.Add($"Invalid Refresh Interval (>{PropertyValidator.UPPER_REFRESH_INTERVAL_RANGE} or <{PropertyValidator.LOWER_REFRESH_INTERVAL_RANGE})");
            if (PropertyValidator.ArePortsSame(HostPort, UpdateServicePort)) ErrorList.Add("Host port and Update Service port must be different");

            if (!PropertyValidator.IsPortInValidRange(HostPort) 
                || !PropertyValidator.IsPortInValidRange(UpdateServicePort)) ErrorList.Add($"Ports must be >{PropertyValidator.LOWER_PORT_RANGE} or <{PropertyValidator.UPPER_PORT_RANGE}");

            if (ErrorList.Count > 0) Util.SetErrorUI(string.Join(", ", ErrorList.ToArray()), (s) => ErrorText = s);
            else Util.SaveProperties();
        }
    }
}

