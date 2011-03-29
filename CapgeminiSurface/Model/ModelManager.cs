using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace CapgeminiSurface.Model
{
    public class ModelManager
    {
        public List<Customer> AllCustomers;
        public List<Customer> EnergyCustomers = new List<Customer>();
        public List<Customer> OtherCustomers = new List<Customer>();
        public List<Customer> CapgeminiInfo = new List<Customer>();

        static ModelManager instance;

        static public ModelManager Instance
        {
            get 
            {
                if (instance == null)
                    instance = new ModelManager();
                return instance;
            }
        }

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Customer>));
            TextWriter textWriter = new StreamWriter("Resources\\Customers.xml");
            serializer.Serialize(textWriter, AllCustomers);
            textWriter.Close();
        }

        public void Load()
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<Customer>));
            TextReader textReader = new StreamReader("Resources\\Customers.xml");
            AllCustomers = (List<Customer>)deserializer.Deserialize(textReader);
            foreach (Customer customer in AllCustomers)
            {
                switch (customer.Category)
                {
                    case "Energy":
                        customer.IsVisible = true;
                        EnergyCustomers.Add(customer);
                        break;
                    case "Other":
                        OtherCustomers.Add(customer);
                        break;
                    case "Capgemini":
                        CapgeminiInfo.Add(customer);
                        break;
                }
            }
        }
    }
}
