using System;
using Microsoft.Surface.Presentation;
using CapgeminiSurface.Model;
using System.Windows;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;
using CapgeminiSurface.Client;
using System.Collections.Generic;

namespace CapgeminiSurface
{
    public partial class ProjectItem
    {
        #region Initialization

        bool _isPlaying;
        ConferenceDataClient client = new ConferenceDataClient();
        bool sessionLoaded;

        public ProjectItem()
        {
            InitializeComponent();
            myBrowser.Loaded += new RoutedEventHandler(myBrowser_Loaded);
        }

        void myBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            var content = DataContext as ContentItem;
            if (content == null || !content.IsLinkItem)
                return;
            myBrowser.Navigate(content.Name);
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

        private void closeButton_ContactDown(object sender, ContactEventArgs e)
        {
            var content = DataContext as ContentItem;
            var image = sender as Image;

            //#Felo: get parent that's a scatterView
            var scatterView = GuiHelpers.GetParentObject<ScatterView>(image);
            if (scatterView != null)
            {
                var scatterSource = scatterView.ItemsSource as ObservableCollection<ContentItem>;

                //#Felo: get parent
                DependencyObject parent = LogicalTreeHelper.GetParent(scatterView);
                while (parent != null)
                {
                    Grid MainGrid = parent as Grid;

                    if (scatterSource != null && MainGrid != null)
                    {
                        //#Felo: get child of parent
                        var favItem = GuiHelpers.GetChildObject<FavouriteStack>(MainGrid);
                        if (favItem != null)
                        {
                            FavouriteStack favStack = favItem as FavouriteStack;
                            if (favStack != null)
                            {
                                favStack.AddInstancePropertyObject(content);
                            }
                        }
                    }
                    parent = LogicalTreeHelper.GetParent(parent);
                }
                scatterSource.Remove(content);
                return;
            }
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

        private void SurfaceButton_Click(object sender, RoutedEventArgs e)
        {
            var content = DataContext as ContentItem;
            if (content == null || !content.IsLinkItem)
                return;
            myBrowser.Navigate(content.Name);
        }

        private void agendaGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var content = DataContext as ContentItem;
            if (content != null)
                if (!content.IsAgendaItem || sessionLoaded)
                    return;
            sessionLoaded = true;
            switch (content.Name)
            {
                case "Day1": client.Day = 1; break;
                case "Day2": client.Day = 2; break;
                case "Day3": client.Day = 3; break;
                default: return;
            }
            for (int i = 0; i < 7; i++)
            {
                StackPanel sp = new StackPanel();
                sp.Children.Add(new Label() { Content = string.Format("Track {0}",i) });
                foreach (var ses in client.GetSessions(i+1))
                {
                    sp.Children.Add(new TextBlock() { Text = ses.Title, TextWrapping=TextWrapping.Wrap });
                }
                agendaGrid.Children.Add(sp);
                Grid.SetColumn(sp, i);
            }
        }
    }
}