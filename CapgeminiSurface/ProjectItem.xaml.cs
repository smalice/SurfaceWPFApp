using System.Collections.Generic;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Manipulations;

namespace CapgeminiSurface
{
    /// <summary>
    /// Interaction logic for ProjectItem.xaml
    /// </summary>
    public partial class ProjectItem : SurfaceUserControl
    {
        public IList<string> parameters { get; set; }
        public ProjectItem()
        {
            InitializeComponent();
        }

        private string stringOptimizer(string text)
        {
            string tmp = text;
            int size = 50;
            if (text.Length > size)
            {
                if (!text.Contains("\n"))
                {
                    tmp = text.Substring(0, size) + "\n";
                    for (int position = 51; position < text.Length; position += (size + 1))
                    {
                        if (text.Substring(position).Length <= size)
                        {
                            tmp += text.Substring(position);
                        }
                        else
                        {
                            tmp += text.Substring(position, size) + "\n";
                        }
                    }
                }
            }
            return tmp;
        }

        //private void Grid_ContactHoldGesture(object sender, Microsoft.Surface.Presentation.ContactEventArgs e)
        //{
        //    bool b = true;
        //}

        //private void Grid_ContactDown(object sender, ContactEventArgs e)
        //{
        //    bool b = true;
        //}
    }
}