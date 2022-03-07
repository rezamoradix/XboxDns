using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using System.IO;
using System.Windows.Input;
using System.Windows;

namespace XboxDns
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        ObservableCollection<Dns> dnsCollection = new ObservableCollection<Dns>();

        public ObservableCollection<Dns> DNSCollection { get { return dnsCollection; } set { dnsCollection = value; OnProp("DNSCollection"); } }

        public string ActiveNetwork
        {
            get
            {
                return new DnsManager().GetActiveNetwork();
            }
        }

        private string newDnsName;

        public string NewDnsName
        {
            get { return newDnsName; }
            set { newDnsName = value; OnProp("NewDnsName"); }
        }

        private string newDnsPrimaryIP;

        public string NewDnsPrimaryIP
        {
            get { return newDnsPrimaryIP; }
            set { newDnsPrimaryIP = value; OnProp("NewDnsPrimaryIP"); }
        }

        private string newDnsSecondaryIP = "0.0.0.0";

        public string NewDnsSecondaryIP
        {
            get { return newDnsSecondaryIP; }
            set { newDnsSecondaryIP = value; OnProp("NewDnsSecondaryIP"); }
        }

        #region Commands
        public ICommand ResetDnsCommand { get; set; } = new Commander(x =>
        {
            DnsManager dnsManager = new DnsManager();
            dnsManager.ResetDNS();
            MessageBox.Show("DNS reset successfully!");
        });

        public ICommand ApplyDnsCommand { get; set; } = new Commander(x =>
        {
            Dns dns = (Dns)x;
            DnsManager dnsManager = new DnsManager();
            dnsManager.SetDNS(dns.PrimaryIP, dns.SecondaryIP);
            MessageBox.Show("DNS applied!");
        });
        public ICommand SaveDnsCommand
        {
            get
            {
                return new Commander(x =>
                {
                    if (!ValidateIPv4(NewDnsPrimaryIP))
                    {
                        MessageBox.Show("PrimaryIP is not valid");
                        return;
                    }
                    if (!ValidateIPv4(NewDnsSecondaryIP))
                    {
                        MessageBox.Show("SecondaryIP is not valid");
                        return;
                    }
                    if (string.IsNullOrEmpty(NewDnsName.Trim()))
                    {
                        MessageBox.Show("Name is empty");
                        return;
                    }
                    Dns dns = new Dns { Name = NewDnsName.Trim(), PrimaryIP = NewDnsPrimaryIP, SecondaryIP = NewDnsSecondaryIP };
                    UpdateDnsCollection(new DnsYamler().AddDns(dns));
                });
            }
        }

        public ICommand DeleteDnsCommand
        {
            get
            {
                return new Commander(x =>
                {
                    Dns dns = (Dns)x;
                    UpdateDnsCollection(new DnsYamler().DeleteDns(dns));
                });
            }
        }
        #endregion

        public ViewModel()
        {
            UpdateDnsCollection(new DnsYamler().GetDnsList());
        }

        void UpdateDnsCollection(List<Dns> dnsList)
        {
            if (dnsList != null) DNSCollection = new ObservableCollection<Dns>(dnsList);
        }

        void OnProp(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }
    }
}
