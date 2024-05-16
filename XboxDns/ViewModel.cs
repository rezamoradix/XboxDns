using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows;

namespace XboxDns
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        ObservableCollection<Dns> _dnsCollection = new ObservableCollection<Dns>();

        public ObservableCollection<Dns> DnsCollection { get => _dnsCollection;
            set { _dnsCollection = value; OnProp("DNSCollection"); } }

        public string? ActiveNetwork => new DnsManager().GetActiveNetwork();

        private string? _newDnsName;
        public string? NewDnsName
        {
            get => _newDnsName;
            set { _newDnsName = value; OnProp("NewDnsName"); }
        }

        private string? _newDnsPrimaryIp;
        public string? NewDnsPrimaryIp
        {
            get => _newDnsPrimaryIp;
            set { _newDnsPrimaryIp = value; OnProp("NewDnsPrimaryIP"); }
        }

        private string? _newDnsSecondaryIp = "0.0.0.0";

        public string? NewDnsSecondaryIp
        {
            get => _newDnsSecondaryIp;
            set { _newDnsSecondaryIp = value; OnProp("NewDnsSecondaryIP"); }
        }

        #region Commands
        public ICommand ResetDnsCommand { get; set; } = new Commander(x =>
        {
            DnsManager dnsManager = new DnsManager();
            dnsManager.ResetDns();
            MessageBox.Show("DNS reset successfully!");
        });

        public ICommand FlushDnsCommand { get; set; } = new Commander(x =>
        {
            DnsManager dnsManager = new DnsManager();
            dnsManager.FlushDns();
            MessageBox.Show("DNS Cache flushed successfully!");
        });

        public ICommand ReleaseIpCommand { get; set; } = new Commander(x =>
        {
            DnsManager dnsManager = new DnsManager();
            dnsManager.ReleaseIp();
            MessageBox.Show("IP released successfully!");
        });

        public ICommand RenewIpCommand { get; set; } = new Commander(x =>
        {
            DnsManager dnsManager = new DnsManager();
            dnsManager.RenewIp();
            MessageBox.Show("IP renewed successfully!");
        });

        public ICommand ApplyDnsCommand { get; set; } = new Commander(x =>
        {
            var dns = (Dns) x!;
            DnsManager dnsManager = new DnsManager();
            dnsManager.SetDns(dns.PrimaryIP, dns.SecondaryIP);
            MessageBox.Show("DNS applied!");
        });

        public ICommand SaveDnsCommand
        {
            get
            {
                return new Commander(x =>
                {
                    if (!ValidateIPv4(NewDnsPrimaryIp))
                    {
                        MessageBox.Show("PrimaryIP is not valid");
                        return;
                    }
                    if (!ValidateIPv4(NewDnsSecondaryIp))
                    {
                        MessageBox.Show("SecondaryIP is not valid");
                        return;
                    }
                    if (string.IsNullOrEmpty(NewDnsName?.Trim()))
                    {
                        MessageBox.Show("Name is empty");
                        return;
                    }
                    Dns dns = new Dns { Name = NewDnsName.Trim(), PrimaryIP = NewDnsPrimaryIp, SecondaryIP = NewDnsSecondaryIp };
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
                    var result = MessageBox.Show("Are you sure?", "Delete DNS", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                    if (result == MessageBoxResult.Yes)
                    {
                        Dns dns = (Dns)x!;
                        UpdateDnsCollection(new DnsYamler().DeleteDns(dns));
                    }
                });
            }
        }
        #endregion

        public ViewModel()
        {
            UpdateDnsCollection(new DnsYamler().GetDnsList());
        }

        void UpdateDnsCollection(List<Dns>? dnsList)
        {
            if (dnsList != null) DnsCollection = new ObservableCollection<Dns>(dnsList);
        }

        void OnProp(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool ValidateIPv4(string? ipString)
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
