using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CapgeminiSurface.Model
{
    public class Customer : INotifyPropertyChanged
    {
        string name;
        string link;
        string logo;
        string category;
        bool isVisible;

        public bool IsVisible
        {
            get { return isVisible; }
            set 
            { 
                isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }

        public string Name
        {
            set { name = value; }
            get { return name; }
        }

        public string Link
        {
            set
            {
                link = value;
                if (link != null && ContentItems != null)
                    ContentItems.Add(new ContentItem() { Name = link, ContentType = ContentItem.Type.LinkItem });
            }
            get { return link; }
        }

        public string Logo
        {
            set { logo = value; }
            get { return "Resources/" + logo; }
        }

        public string Category
        {
            set { category = value; }
            get { return category; }
        }

        public List<ContentItem> ContentItems;

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
