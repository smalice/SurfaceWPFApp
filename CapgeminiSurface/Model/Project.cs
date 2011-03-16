using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapgeminiSurface.Model
{
    public class Project
    {
        string name;
        string description;

        public string Name
        {
            set { name = value; }
            get { return name; }
        }

        public string Description
        {
            set { description = value; }
            get { return description; }
        }
    }
}
