using Microsoft.Surface.Presentation;
using CapgeminiSurface.Model;

namespace CapgeminiSurface
{
    public partial class ProjectItem
    {
        #region Initialization

        bool _isPlaying;

        public ProjectItem()
        {
            InitializeComponent();
        }

        #endregion

        private void MyMediaContactDown(object sender, ContactEventArgs e)
        {
            var content = DataContext as ContentItem;
            if (content != null)
                if (!content.IsVideoItem)
                    return;
            if (_isPlaying)
            {
                if (myMedia.Position >= myMedia.NaturalDuration.TimeSpan)
                {
                    myMedia.Position = new System.TimeSpan();
                    myMedia.Play();
                    _isPlaying = false;
                }
                else
                    myMedia.Pause();
            }
            else
            {
                myMedia.Play();
            }
            _isPlaying = !_isPlaying;
        }

        private void VideoGridLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var content = DataContext as ContentItem;
            if (content != null)
                if (!content.IsVideoItem)
                    return;
            myMedia.Play();
            myMedia.Pause();
        }
    }
}