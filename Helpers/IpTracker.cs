namespace IPChangeNotifier.Helpers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Microsoft.Toolkit.Uwp.Notifications;
    using Newtonsoft.Json;

    public static class IpTracker
    {
        private static readonly string _dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IPChangeNotifier");

        private static readonly string _fileName = Path.Combine(_dir, "ipHistory.json");

        /// <summary>
        /// Handles checking if the IP tracking file exists or not
        /// </summary>
        /// <remarks>
        /// Also handles creating the Directory that will be used if it does not already exist
        /// </remarks>
        /// <returns>True if it does, False if not</returns>
        public static bool DoesOldIpExist()
        {
            try
            {
                if (!Directory.Exists(_dir))
                {
                    Directory.CreateDirectory(_dir);
                    return false;
                }

                var files = Directory.GetFiles(_dir);
                if (!files.Any())
                {
                    return false;
                }

                return files.Any(f => f.Equals(_fileName));
            }
            catch (Exception _)
            {
                Console.WriteLine($"Unable to determine if an old IP address exists, re-creating.");
                return false;
            }
        }

        /// <summary>
        /// Handles recording a record of the IP address to the file
        /// </summary>
        /// <param name="ip">The IP address to Save</param>
        public static void SaveIp(string ip)
        {
            try
            {
                if (!Directory.Exists(_dir))
                {
                    Directory.CreateDirectory(_dir);
                }

                if (File.Exists(_fileName))
                {
                    File.Delete(_fileName);
                }

                var _ = new
                {
                    IpAddress = ip
                };

                File.WriteAllText(_fileName, JsonConvert.SerializeObject(_));
            }
            catch (Exception _)
            {
                Console.WriteLine("Issue saving record of IP");
            }
        }

        /// <summary>
        /// Handles determening whether or not the IP address has changed
        /// </summary>
        /// <param name="ip">The current IP to check</param>
        /// <returns>
        /// <see cref="bool"/> for whether or not it has changed. 
        /// <see cref="string"/> for the old IP address if applicable, <see cref="string.Empty"/> if no IP recorded
        /// </returns>
        public static (bool, string) HasIpChanged(string ip)
        {
            try
            {
                if (!Directory.Exists(_dir) || !File.Exists(_fileName))
                {
                    return (true, string.Empty);
                }

                var _ = File.ReadAllText(_fileName);
                var oldIp = JsonConvert.DeserializeAnonymousType(_, new { IpAddress = string.Empty }).IpAddress;
                return (!oldIp.Equals(ip), oldIp);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to determine if IP has changed: {ex.Message + ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Handles retrieving the public IP address of the current network
        /// </summary>
        /// <returns>The IP address if able to be retrieved, <see cref="string.Empty"/> if network issues are encountered</returns>
        public static string WhatIsMyIp()
        {
            using (var client = new WebClient())
            {
                try
                {
                    return client.DownloadString("http://wtfismyip.com/text");
                }
                catch (WebException)
                {
                }

                try
                {
                    return client.DownloadString("https://api.ipify.org/");
                }
                catch (WebException)
                {
                }

                new ToastContentBuilder()
                    .AddText("Unable to check IP Address", AdaptiveTextStyle.Caption)
                    .AddText("Attempted multiple tries, please check network connection and relaunch.")
                    .Show();

                return string.Empty;
            }
        }
    }
}