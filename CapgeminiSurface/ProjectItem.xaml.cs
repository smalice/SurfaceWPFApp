using System.Drawing;
using System.Linq;
using Microsoft.Surface.Presentation;
using CapgeminiSurface.Model;
using System.Windows;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using CapgeminiSurface.Client;
using Image = System.Windows.Controls.Image;

namespace CapgeminiSurface
{
    public partial class ProjectItem
    {
        #region Initialization

        bool _isPlaying;
        public static ConferenceDataClient client;
        bool sessionLoaded;

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

        private void agendaGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var content = DataContext as ContentItem;
            if (content != null)
                if (!content.IsAgendaItem || sessionLoaded || client == null)
                    return;
            sessionLoaded = true;
            if (content != null)
                switch (content.Name)
                {    
                    case "Day1": client.Day = 1; break;
                    case "Day2": client.Day = 2; break;
                    case "Day3": client.Day = 3; break;
                    default: return;
                }
            for (var i = 0; i < 8; i++)
            {
				for (var j = 0; j < 8; j++)
            	{
            	    if (i > 0 && j < 1)
            	    {
            	        var nLabel = new Label {Content = string.Format("Track {0}", i)};
            	        agendaGrid.Children.Add(nLabel);
            	        Grid.SetColumn(nLabel, i);
            	        Grid.SetRow(nLabel, j);
            	    }
            	}
				if (i < 1)
					{
                        var sp = new Label { Content = string.Format("Time:") };
                        agendaGrid.Children.Add(sp);
                        Grid.SetColumn(sp, i);
                        Grid.SetRow(sp,0);

                        var sp1 = new TextBlock { Text = " 09:00 - 10:00 ", TextWrapping = TextWrapping.Wrap };
                        agendaGrid.Children.Add(sp1);
                        Grid.SetColumn(sp1, i);
                        Grid.SetRow(sp1,1);

                        var sp2 = new TextBlock { Text = " 10:20 - 11:20 ", TextWrapping = TextWrapping.Wrap };
                        agendaGrid.Children.Add(sp2);
                        Grid.SetColumn(sp2, i);
                        Grid.SetRow(sp2, 2);

                        var sp3 = new TextBlock { Text = " 11:40 - 12:40 ", TextWrapping = TextWrapping.Wrap };
                        agendaGrid.Children.Add(sp3);
                        Grid.SetColumn(sp3, i);
                        Grid.SetRow(sp3, 3);

                        var sp4 = new TextBlock { Text = " 13:40 - 14:40 ", TextWrapping = TextWrapping.Wrap };
                        agendaGrid.Children.Add(sp4);
                        Grid.SetColumn(sp4, i);
                        Grid.SetRow(sp4, 4);

                        var sp5 = new TextBlock { Text = " 15:00 - 16:00 ", TextWrapping = TextWrapping.Wrap };
                        agendaGrid.Children.Add(sp5);
                        Grid.SetColumn(sp5, i);
                        Grid.SetRow(sp5, 5);

                        var sp6 = new TextBlock { Text = " 16:20 - 17:20 ", TextWrapping = TextWrapping.Wrap };
                        agendaGrid.Children.Add(sp6);
                        Grid.SetColumn(sp6, i);
                        Grid.SetRow(sp6, 6);

                        var sp7 = new TextBlock { Text = " 17:40 - 18:40 ", TextWrapping = TextWrapping.Wrap };
                        agendaGrid.Children.Add(sp7);
                        Grid.SetColumn(sp7, i);
                        Grid.SetRow(sp7, 7);
					}
					else
				{
				    var counter = client.Day==1 ? 2 : 1;

				    foreach (var ntBlock in
				        client.GetSessions(i).Select(ses => new TextBlock {Text = ses.Title, TextWrapping = TextWrapping.Wrap}))
				    {
				        agendaGrid.Children.Add(ntBlock);
				        Grid.SetColumn(ntBlock, i);
				        Grid.SetRow(ntBlock, counter);
				        counter = counter+1;
				    }
				}
            }
        }

        private void visitGrid_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}