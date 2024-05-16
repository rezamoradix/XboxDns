using System.Linq;
using System.Net.NetworkInformation;

namespace XboxDns
{
    public class DnsManager
    {
        private NetworkInterface? GetActiveEthernetOrWifiNetworkInterface()
        {
            var nic = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(
                a => a.OperationalStatus == OperationalStatus.Up &&
                (a.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || a.NetworkInterfaceType == NetworkInterfaceType.Ethernet) &&
                a.GetIPProperties().GatewayAddresses.Any(g => g.Address.AddressFamily.ToString() == "InterNetwork"));

            return nic;
        }

        public void SetDns(string? primaryIp, string? secondaryIp)
        {
            SimpleExec.Command.Run("powershell", $@"Get-NetConnectionProfile | select interfaceindex | % {{$ix = $_.interfaceindex.tostring(); Set-DnsClientServerAddress -InterfaceIndex $ix -ServerAddresses ('{primaryIp}','{secondaryIp}')}}", createNoWindow: true);
        }

        public string? GetDns()
        {
            var ipAddressCollection = GetActiveEthernetOrWifiNetworkInterface()?.GetIPProperties().DnsAddresses;

            return ipAddressCollection != null ? string.Join(',', ipAddressCollection) : null;
        }

        public void ResetDns()
        {
            SimpleExec.Command.Run("powershell", @"Get-NetConnectionProfile | select interfaceindex | % {$ix = $_.interfaceindex.tostring(); Set-DnsClientServerAddress -InterfaceIndex $ix -ResetServerAddresses}", createNoWindow: true);
            SimpleExec.Command.Run("powershell", @"ipconfig /flushdns", createNoWindow: true);
        }

        public string? GetActiveNetwork()
        {
            return GetActiveEthernetOrWifiNetworkInterface()?.Name;
        }

        public void FlushDns()
        {
            SimpleExec.Command.Run("powershell", @"ipconfig /flushdns", createNoWindow: true);
        }

        public void ReleaseIp()
        {
            SimpleExec.Command.Run("powershell", @"ipconfig /release", createNoWindow: true);
        }

        public void RenewIp()
        {
            SimpleExec.Command.Run("powershell", @"ipconfig /renew", createNoWindow: true);
        }
    }
}
