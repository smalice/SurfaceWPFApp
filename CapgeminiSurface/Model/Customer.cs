using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapgeminiSurface.Model
{
    public class Customer
    {
        string name;
        string link;
        string logo;
        string category;
        bool isVisible;

        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }

        public string Name
        {
            set { name = value; }
            get { return name; }
        }

        public string Link
        {
            set { link = value; }
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

    }
}
