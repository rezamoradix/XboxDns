using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XboxDns
{
    public class DnsManager
    {
        private NetworkInterface getActiveEthernetOrWifiNetworkInterface()
        {
            var Nic = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(
                a => a.OperationalStatus == OperationalStatus.Up &&
                (a.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || a.NetworkInterfaceType == NetworkInterfaceType.Ethernet) &&
                a.GetIPProperties().GatewayAddresses.Any(g => g.Address.AddressFamily.ToString() == "InterNetwork"));

            return Nic;
        }

        public void SetDNS(string PrimaryIP, string SecondaryIP)
        {
            SimpleExec.Command.Run("powershell", $@"Get-NetConnectionProfile | select interfaceindex | % {{$ix = $_.interfaceindex.tostring(); Set-DnsClientServerAddress -InterfaceIndex $ix -ServerAddresses ('{PrimaryIP}','{SecondaryIP}')}}", createNoWindow: true);
        }

        public string GetDNS()
        {
            return string.Join(',', getActiveEthernetOrWifiNetworkInterface().GetIPProperties().DnsAddresses);
        }

        public void ResetDNS()
        {
            SimpleExec.Command.Run("powershell", @"Get-NetConnectionProfile | select interfaceindex | % {$ix = $_.interfaceindex.tostring(); Set-DnsClientServerAddress -InterfaceIndex $ix -ResetServerAddresses}", createNoWindow: true);
            SimpleExec.Command.Run("powershell", @"ipconfig /flushdns", createNoWindow: true);
        }

        public string GetActiveNetwork()
        {
            return getActiveEthernetOrWifiNetworkInterface().Name;
        }
    }
}
