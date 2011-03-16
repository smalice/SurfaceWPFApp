using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapgeminiSurface.Model
{
    public class VisitCard
    {
        string name;
        string email;
        string tlf;
        string pictureFileName;

        public string Name
        {
            set { name = value; }
            get { return name; }
        }
        public string Email
        {
            set { email = value; }
            get { return email; }
        }
        public string Tlf
        {
            set { tlf = value; }
            get { return tlf; }
        }
        public string PictureFileName
        {
            set { pictureFileName = value; }
            get { return pictureFileName; }
        }
    }
}
