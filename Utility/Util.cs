using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowPlaying.Utility
{
    public class Util
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
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = uri,
                UseShellExecute = true
            });

            return true;
        }

    }
}
