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

        //private void setDns(NetworkInterface NIC, string? DNS)
        //{
        //    ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
        //    ManagementObjectCollection objMOC = objMC.GetInstances();

        //    foreach (ManagementObject objMO in objMOC)
        //    {
        //        if ((bool)objMO["IPEnabled"])
        //        {
        //            if (objMO["Caption"].ToString().Contains(NIC.Description))
        //            {
        //                ManagementBaseObject newDNS =
        //                  objMO.GetMethodParameters("SetDNSServerSearchOrder");
        //                if (newDNS != null)
        //                {
        //                    newDNS["DNSServerSearchOrder"] = DNS?.Split(',');
        //                    objMO.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
        //                }
        //            }
        //        }
        //    }
        //}

        public void SetDNS(string PrimaryIP, string SecondaryIP)
        {
            SimpleExec.Command.Run("powershell", $@"Get-NetConnectionProfile | select interfaceindex | % {{$ix = $_.interfaceindex.tostring(); Set-DnsClientServerAddress -InterfaceIndex $ix -ServerAddresses ('{PrimaryIP}','{SecondaryIP}')}}", createNoWindow: true);
            //setDns(getActiveEthernetOrWifiNetworkInterface(), $"{PrimaryIP},{SecondaryIP}");
        }

        public string GetDNS()
        {
            return string.Join(',', getActiveEthernetOrWifiNetworkInterface().GetIPProperties().DnsAddresses);
        }

        public void ResetDNS()
        {
            SimpleExec.Command.Run("powershell", @"Get-NetConnectionProfile | select interfaceindex | % {$ix = $_.interfaceindex.tostring(); Set-DnsClientServerAddress -InterfaceIndex $ix -ResetServerAddresses}", createNoWindow: true);
        }

        public string GetActiveNetwork()
        {
            return getActiveEthernetOrWifiNetworkInterface().Name;
        }
    }
}
