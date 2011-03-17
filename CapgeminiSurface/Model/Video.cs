using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapgeminiSurface.Model
{
    public class Video
    {
        string fileName;

        public string FileName
        {
            set { fileName = value; }
            get { return fileName; }
        }
    }
}
