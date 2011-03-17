using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapgeminiSurface.Model
{
    public class Customer
    {
        string name;

        public string Name
        {
            set { name = value; }
            get { return name; }
        }

        public List<Video> Videos;
        public List<Picture> Pictures;
        public List<Project> Projects;
        public List<VisitCard> Contacts;

    }
}
