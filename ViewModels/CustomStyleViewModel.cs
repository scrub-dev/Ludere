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
    class CustomStyleViewModel : ObservableObject
    {
        public bool CustomStyleChecked { get => GetCustomStyleChecked(); set { SetCustomStyleChecked(value); OnPropertyChanged(); }}

        private void SetCustomStyleChecked(bool customStyleChecked)
        {
            Settings.Default.UseCustomStyle = customStyleChecked;
            Settings.Default.Save();
        }
        private bool GetCustomStyleChecked() => Settings.Default.UseCustomStyle;
    }
}
