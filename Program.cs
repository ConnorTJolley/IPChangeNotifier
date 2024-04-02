namespace IPChangeNotifier
{
    using System;
    using System.Windows.Forms;
    using IPChangeNotifier.Helpers;
    using Microsoft.Toolkit.Uwp.Notifications;

    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var _ = IpTracker.WhatIsMyIp()
                    .Replace("\t", string.Empty)
                    .Replace(Environment.NewLine, string.Empty)
                    .Trim();

                if (_.Equals(string.Empty))
                {
                    return;
                }

                var oldIpExists = IpTracker.DoesOldIpExist();

                if (!oldIpExists)
                {
                    // Store for next run
                    IpTracker.SaveIp(_);
                }

                var (changed, oldIp) = IpTracker.HasIpChanged(_);

                if (!changed)
                {
                    return;
                }

                // Display notification
                new ToastContentBuilder()
                    .AddText("Your IP has changed!", AdaptiveTextStyle.Caption)
                    .AddText($"Old IP: {oldIp} -> New IP: {_}")
                    .Show();

                // Save new IP
                IpTracker.SaveIp(_);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Issue tracking IP change, {ex.Message + ex.StackTrace}");
            }
        }
    }
}