using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;

namespace CapgeminiSurface.Model
{
    public class ModelManager : INotifyPropertyChanged
    {
        private Customer selectedCustomer;

        public Customer SelectedCustomer
        {
            get { return selectedCustomer; }
            set 
            { 
                selectedCustomer = value;
                OnPropertyChanged("SelectedCustomer");
            }
        }

        public List<Customer> AllCustomers;
        public List<Customer> EnergyCustomers = new List<Customer>();
        public List<Customer> CapgeminiInfo = new List<Customer>();
        public List<Customer> OtherCustomers = new List<Customer>();
		public List<Customer> NdcInfo = new List<Customer>();
		
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
                    case "Other":
                        OtherCustomers.Add(customer);
                        break;
					case "Capgemini":
                        CapgeminiInfo.Add(customer);
                        break;
					case "Energy":
                        EnergyCustomers.Add(customer);
                        break;
					case "Ndc":
						NdcInfo.Add(customer);
						break;
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
