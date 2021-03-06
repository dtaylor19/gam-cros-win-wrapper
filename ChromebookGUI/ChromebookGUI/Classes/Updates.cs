﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ChromebookGUI
{
    class Updates
    {
        public static Dictionary<string, string> IsNewestVersion()
        {
            Console.WriteLine("Starting newest version check...");
            HttpClient http = Globals.HttpClientObject;
            Task<string> newestVersionsString = http.GetStringAsync("https://iamtheyammer.github.io/gam-cros-win-wrapper/releases.json");
            Dictionary<string, string> newestVersions = null;
            try
            {
                newestVersionsString.Wait();
                newestVersions = JsonConvert.DeserializeObject<Dictionary<string, string>>(newestVersionsString.Result);
            } catch /*(Exception e)*/
            {
                GetInput.ShowInfoDialog("Internet Issue", "No internet connection", "Could not connect to the internet to check for updates.\nPlease make sure that github.io is accessible on your network.\n\nThe full URL this software is trying to access is:\nhttps://iamtheyammer.github.io/gam-cros-win-wrapper/releases.json");
                return new Dictionary<string, string>()
                {
                    ["error"] = "true",
                    ["errorCode"] = "2" // no internet
                };
            }

            if(newestVersions == null)
            {
                GetInput.ShowInfoDialog("Error", "error checking for updates", "error!");
            }
            string currentVersion = Software.Version;
            string newestVersion = newestVersions[Software.Type];
            if (newestVersion == currentVersion)
            {
                return new Dictionary<string, string>()
                {
                    ["isNewestVersion"] = "true"
                };
            } else
            {
                Console.WriteLine(newestVersions[Software.Type]);
                Console.WriteLine(String.IsNullOrEmpty(newestVersions[Software.Type]));
                return new Dictionary<string, string>()
                {
                    ["isNewestVersion"] = "false",
                    ["newestVersion"] = !String.IsNullOrEmpty(newestVersions[Software.Type]) ? newestVersions[Software.Type] : "(could not be determined)",
                    ["changes"] = !String.IsNullOrEmpty(newestVersions[Software.Type + "Changes"]) ? newestVersions[Software.Type + "Changes"] : "(there was an issue getting the changelog)"
                };
            }
        }

        /// <summary>
        /// Returns "true" if an update is available, "false" if one is not and "error" if there was an error.
        /// String because I need more options. If a new update is available or there was an error it already opens a dialog!
        /// </summary>
        /// <returns></returns>
        public static string CheckForUpdates()
        {
            Dictionary<string, string> isNewestVersion = Updates.IsNewestVersion();
            if (isNewestVersion["isNewestVersion"] == "false")
            {
                bool extraButtonPressed = GetInput.ShowInfoDialog(
                    "Update Available", 
                    "An update for this app is currently available.", 
                    "You are running " + Software.Type + " " + Software.Version + " and the newest version is " + isNewestVersion["newestVersion"] + ".\nClick the button on the bottom left to upgrade.\n\nNew in version " + isNewestVersion["newestVersion"] + ":\n" + isNewestVersion["changes"],
                    new Button { IsEnabled = true, Text = "Download newest build" });
                if(extraButtonPressed)
                {
                    switch(Software.Type)
                    {
                        case "alpha":
                            Process.Start("https://github.com/iamtheyammer/gam-cros-win-wrapper/raw/master/ChromebookGUI/Installer/Tool+Installer/Installer.msi");
                            break;
                        default:
                            Process.Start("https://github.com/iamtheyammer/releases/latest");
                            break;
                    }
                }
                return "true";
            }
            else if (isNewestVersion["isNewestVersion"] == "error")
            {
                return "error";
                // nothing
            }
            else
            {
                return "false";
                // nothing
            }
        }
    }
}
