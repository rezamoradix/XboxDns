using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using YamlDotNet.Serialization;
using System.Windows;

namespace XboxDns
{
    public class DnsYamler
    {
        static string DnsYamlLocation = AppDomain.CurrentDomain.BaseDirectory + "\\dns.yaml";
        public List<Dns>? GetDnsList()
        {
            if (File.Exists(DnsYamlLocation))
            {
                return (List<Dns>?) new Deserializer().Deserialize(File.ReadAllText(DnsYamlLocation), typeof(List<Dns>));
            }
            return null;
        }

        private void writeDnsYamlFile(List<Dns> dnsList)
        {
            if (File.Exists(DnsYamlLocation)) File.Delete(DnsYamlLocation);
            using (TextWriter tw = File.CreateText(DnsYamlLocation))
            {
                new Serializer().Serialize(tw, dnsList, typeof(List<Dns>));
            }
        }

        public List<Dns> AddDns(Dns dns)
        {
            List<Dns> dnsList = GetDnsList() ?? new List<Dns>();
            if (dnsList.FirstOrDefault(x => x.Name.Equals(dns.Name)) != null)
            {
                MessageBox.Show("DNS name Exists! use another name");
                return dnsList;
            }
            dnsList.Add(dns);
            writeDnsYamlFile(dnsList);
            return dnsList;
        }
        public List<Dns> DeleteDns(Dns dns)
        {
            List<Dns> dnsList = (GetDnsList() ?? new List<Dns>()).Where(x => !x.Name.Equals(dns.Name)).ToList();
            writeDnsYamlFile(dnsList);
            return dnsList;
        }
    }
}
