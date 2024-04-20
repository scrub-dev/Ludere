using NowPlaying.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NowPlaying.Utility
{
    public partial class Util
    {
        public static bool IsValidUri(string uri)
        {
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute)) return false;
            if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri? tmp)) return false;

            return tmp.Scheme == Uri.UriSchemeHttp || tmp.Scheme == Uri.UriSchemeHttps;
        }

        public static bool OpenUri(string uri)
        {
            if (!IsValidUri(uri))
                return false;
            Process.Start(new ProcessStartInfo
            {
                FileName = uri,
                UseShellExecute = true
            });

            return true;
        }

        //public static bool DoesPropertyExist (string propString) => Settings.Default.Context.ContainsKey(propString);
        public static bool DoesPropertyExist(string propString) => Settings.Default[propString] is not null;
        public static string GetProperty(string propString) 
        {
            if (!DoesPropertyExist(propString)) throw new MissingFieldException();
            return (string)Settings.Default[propString];
        }
        public static void SetProperty(string propString, string newValue) 
        {
            if (!DoesPropertyExist(propString)) throw new MissingFieldException();
            Settings.Default[propString] = newValue;
        }

        public static void SaveProperties()
        {
            Settings.Default.Save();
        }


        public static void TimedSetErrorUI(string errorString, int TimedLength, Action<string> UpdateUIAction)
        {
            Thread t = new(new ThreadStart(() => {
                SetErrorUI(errorString, UpdateUIAction);
                Thread.Sleep(TimedLength);
                SetErrorUI("", UpdateUIAction);
            }));
            t.Start();
        }

        public static void SetErrorUI(string errorString, Action<string> UpdateUIAction)
        {
            UpdateUIAction.Invoke(errorString);
        }


        public static void HandleRegexValidation(Regex r, object sender, TextCompositionEventArgs e)
        {
            e.Handled = r.IsMatch(e.Text);
        }
        public static void HandleRegexValidationNumbersOnly(object sender, TextCompositionEventArgs e)
        {
            Regex r = NumbersOnlyRegex();
            HandleRegexValidation(r, sender, e);
        }

        [GeneratedRegex("[^0-9]+")]
        private static partial Regex NumbersOnlyRegex();
    }
}
