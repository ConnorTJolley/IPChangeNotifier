# IPChangeNotifier | Win

## How does it work?
It's very simple, it creates a locally stored "history" (just the last changed IP address) and when executing runs a call to one of 2 public API's (http://wtfismyip.com/text & https://api.ipify.org/) and checks whether your IP address has changed from the previously saved IP address & notifies you if so.

## But why?
If you are working with a secure / locked down network which has firewalls / IP whitelists, you have probably encountered the issue of your ISP rolling your IP address and you can no longer connect to said network, setting up this console application as a Scheduled Task to run shortly after startup would at least give you a quick notification if that occurs, so you can get the whitelist updated with your new IP address.
