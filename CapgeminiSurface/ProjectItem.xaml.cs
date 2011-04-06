using System.Collections.Generic;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Manipulations;
using CapgeminiSurface.Model;

namespace CapgeminiSurface
{
    /// <summary>
    /// Interaction logic for ProjectItem.xaml
    /// </summary>
    public partial class ProjectItem : SurfaceUserControl
    {
        bool isPlaying;

        public ProjectItem()
        {
            InitializeComponent();
        }

        private void myMedia_ContactDown(object sender, ContactEventArgs e)
        {
            var content = this.DataContext as ContentItem;
            if (!content.IsVideoItem)
                return;
            if (isPlaying)
                myMedia.Pause();
            else
                myMedia.Play();
            isPlaying = !isPlaying;
        }

        private void videoGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var content = this.DataContext as ContentItem;
            if (!content.IsVideoItem)
                return;
            myMedia.Play();
            myMedia.Pause();
        }
    }
}