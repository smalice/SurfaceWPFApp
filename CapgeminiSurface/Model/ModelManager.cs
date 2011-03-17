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
        public List<Customer> Customers;

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
            serializer.Serialize(textWriter, Customers);
            textWriter.Close();
        }

        public void Load()
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<Customer>));
            TextReader textReader = new StreamReader("Resources\\Customers.xml");
            Customers = (List<Customer>)deserializer.Deserialize(textReader);
            textReader.Close();
        }
    }
}
