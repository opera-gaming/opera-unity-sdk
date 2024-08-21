using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Opera
{
    public sealed class UrlOpener
    {
        private readonly IUserInterface userInterface;

        public UrlOpener(IUserInterface userInterface)
        {
            this.userInterface = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
        }

        private void OpenBrowser(string _url)
        {
            var _urlQuotes = '"' + _url + '"';

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Process.Start("xdg-open", _urlQuotes);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Process.Start("explorer", _urlQuotes);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Process.Start("open", _urlQuotes);
        } // end OpenBrowser

        public void OpenOperaGXBrowser(string _url)
        {
            userInterface.Log("OpenOperaGXBrowser");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                List<string> candidateBrowsers = new List<string>();
                List<string> displayNames = new List<string>();
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    candidateBrowsers.Add("/" + Path.Combine("Applications", "Opera GX.app", "Contents", "MacOS", "Opera"));
                    displayNames.Add("Opera GX");
                }
                else
                {
                    string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    candidateBrowsers.Add(Path.Combine(localAppData, "Programs", "Opera GX", "launcher.exe"));
                    candidateBrowsers.Add(Path.Combine(localAppData, "Programs", "Opera GX beta", "launcher.exe"));
                    candidateBrowsers.Add(Path.Combine(localAppData, "Programs", "Opera GX developer", "launcher.exe"));
                } // end else
                string browserPath = string.Empty;
                string displayName = string.Empty;
                int count = 0;
                foreach (var s in candidateBrowsers)
                {
                    if (File.Exists(s))
                    {
                        browserPath = s;
                        if (displayNames.Count > count)
                            displayName = displayNames[count];
                        break;
                    } // end if
                    ++count;
                } // end foreach

                if (string.IsNullOrEmpty(browserPath))
                {
                    userInterface.Log("Unable to find Opera GX Browser at paths:");
                    string paths = string.Join(Environment.NewLine, candidateBrowsers);
                    userInterface.Log(paths);
                    OpenBrowser(_url);
                }
                else
                {
                    try
                    {
                        // The reason to use "osascript" for MacOS: if a browser is already running then
                        // a simple call Process.Start(path, url) will open a new instance of a browser.
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            // this is for the later version of .NET. Let's keep it in case any engine developer switches to the later version.
                            // This Apple Script is supposed to be executed in the following format
                            // osascript -e 'tell application ...'
                            // The only way to pass the arguments now is to use the sequence of the arguments without single quotes.
                            // 
                            // Passing all arguments as a single string like this this doesn't work anymore for some reason:
                            // Process.Start("osascript", "-e \'tell application...\'");    // - it doesn't work
                            //
                            //Process.Start("osascript", new[] { "-e", string.Format("tell application \"{0}\" to open location \"{1}\"", displayName, _url) });

                            Process.Start(new ProcessStartInfo("osascript", string.Format("-e \'tell application \"{0}\" to open location \"{1}\"\'", displayName, _url)));
                        }
                        else
                        {
                            Process.Start(browserPath, _url);
                        } // end else
                    }
                    catch (Exception _ex)
                    {
                        userInterface.Log(string.Format("OpenOperaGXBrowser:{0} exception: {1}", browserPath, _ex.Message));
                    }
                } // end else
            } // end block
            else
            {
                OpenBrowser(_url);
            }
        } // end OpenBrowser
    }
}
