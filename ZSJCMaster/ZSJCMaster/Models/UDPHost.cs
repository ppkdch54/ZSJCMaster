using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ZSJCMaster.Models
{
    public class UDPHost: BindableBase
    {
        public int Id { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }

        public static ObservableCollection<UDPHost> GetClients()
        {
            XDocument doc = XDocument.Load("Application.config");
            var clientsElement = doc.Descendants("clients");
            if (clientsElement.Count() == 0)
            {
                return null;
            }
            //重新读取
            clientsElement = doc.Descendants("clients");
            var clients = clientsElement.Descendants("client");
            ObservableCollection<UDPHost> list = new ObservableCollection<UDPHost>();
            foreach (var item in clients)
            {
                UDPHost client = new UDPHost();
                client.Id = int.Parse(item.Attribute("id").Value);
                client.IP = item.Attribute("ip").Value;
                client.Port = int.Parse(item.Attribute("port").Value);
                list.Add(client);
            }
            return list;
        }

        public static void AddClient(UDPHost client)
        {
            XDocument doc = XDocument.Load("Application.config");
            var clientsElement = doc.Descendants("clients");
            //如果没有，说明是第一次添加Clients
            if (clientsElement.Count() == 0)
            {
                var element = new XElement("clients");
                doc.Add(element);
            }
            //重新读取
            clientsElement = doc.Descendants("clients");
            var clients = clientsElement.Descendants("client");
            var clientElement = clients.SingleOrDefault(c => c.Attribute("ip").Value == client.IP);
            if(clientElement == null)
            {
                clientElement = new XElement("client");
                clientElement.SetAttributeValue("id",client.Id);
                clientElement.SetAttributeValue("ip", client.IP);
                clientElement.SetAttributeValue("port", client.Port);
                clientsElement.Single().Add(clientElement);
            }
            doc.Save("Application.config");
            
        }

        public static void UpdateClient(UDPHost client)
        {
            XDocument doc = XDocument.Load("Application.config");
            var clients = doc.Descendants("clients").Descendants("client");
            var element = clients.SingleOrDefault(c => c.Attribute("id").Value == client.Id.ToString());
            if (element == null) { return; }
            element.SetAttributeValue("ip", client.IP);
            element.SetAttributeValue("port", client.Port);
            doc.Save("Application.config");
        }

        public static void DeleteClient(int id)
        {
            XDocument doc = XDocument.Load("Application.config");
            var clients = doc.Descendants("clients").Descendants("client");
            var element = clients.SingleOrDefault(c => c.Attribute("id").Value == id.ToString());
            element.Remove();
            doc.Save("Application.config");
        }
    }

    public class UDPServer : UDPHost
    {
        /// <summary>
        /// 从配置文件读取Server
        /// </summary>
        /// <returns></returns>
        public static UDPServer GetServerFromConfig()
        {
            XDocument doc = XDocument.Load("Application.config");
            var element = doc.Descendants("server").Single();
            UDPServer server = new UDPServer();
            server.IP = element.Attribute("ip").Value;
            server.Port = int.Parse(element.Attribute("port").Value);
            return server;
        }

        public static void UpdateServerToConfig(string attr,string value)
        {
            XDocument doc = XDocument.Load("Application.config");
            var element = doc.Descendants("server").Single();
            element.SetAttributeValue(attr,value);
            doc.Save("Application.config");
        }
    }
}
