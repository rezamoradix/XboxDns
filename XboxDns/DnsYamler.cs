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
        public List<Dns>? GetDnsList()
        {
            if (File.Exists("dns.yaml"))
            {
                return (List<Dns>?)new Deserializer().Deserialize(File.ReadAllText("dns.yaml"), typeof(List<Dns>));
            }
            return null;
        }

        private void writeDnsYamlFile(List<Dns> dnsList)
        {
            if (File.Exists("dns.yaml")) File.Delete("dns.yaml");
            using (TextWriter tw = File.CreateText("dns.yaml"))
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
