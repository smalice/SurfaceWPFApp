using Microsoft.Surface.Presentation;
using CapgeminiSurface.Model;
using System.Windows;

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
                    playButton.Visibility = Visibility.Hidden;
                    _isPlaying = false;
                }
                else
                {
                    myMedia.Pause();
                    playButton.Visibility = Visibility.Visible;
                }
            }
            else
            {
                myMedia.Play();
                playButton.Visibility = Visibility.Hidden;
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

        private void myMedia_MediaEnded(object sender, RoutedEventArgs e)
        {
            playButton.Visibility = Visibility.Visible;
        }

        private void playButton_ContactDown(object sender, ContactEventArgs e)
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
                    playButton.Visibility = Visibility.Hidden;
                    _isPlaying = false;
                }
                else
                {
                    myMedia.Pause();
                    playButton.Visibility = Visibility.Visible;
                }
            }
            else
            {
                myMedia.Play();
                playButton.Visibility = Visibility.Hidden;
            }
            _isPlaying = !_isPlaying;
        }
		
		
    }
}