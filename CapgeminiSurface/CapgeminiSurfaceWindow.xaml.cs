using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using CapgeminiSurface.Model;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Manipulations;
using System.Collections.ObjectModel;
using SurfaceBluetooth;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace CapgeminiSurface
{
    public partial class CapgeminiSurfaceWindow
    {
        #region Initialization
        
        public enum States
        {
            AllCardRotation = 0,
            OneCardDocked = 1,
        } ;

        public States CurrentState = States.AllCardRotation;

        private readonly Random _randomStartAngle = new Random();

        public ArrayList MenuCardHolder = new ArrayList();

        private Affine2DManipulationProcessor _manipulationProcessor;

        readonly Point _centerPoint = new Point(512, 384);

        private ObservableCollection<ContentItem> _targetItems;

        public ObservableCollection<ContentItem> TargetItems
        {
            get { return _targetItems ?? (_targetItems = new ObservableCollection<ContentItem>()); }
        }

        private bool isSendingAfterDrop;
        private Point dropPoint;
        private double dropOrientation;
		private double dropScatHeight;
		private double dropScatWidth;
		private double newAngle;
        private BluetoothMonitor monitor;

        public CapgeminiSurfaceWindow()
        {
            ModelManager.Instance.Load();
            InitializeComponent();
            InitializeManipulationProcessor();
			AddFilterHandlers();
			
			newAngle = _randomStartAngle.Next(0, 360);
			
            foreach (Customer customer in ModelManager.Instance.AllCustomers)
            {
                InitializeCard(customer);
            }
            Logo.DeltaManipulationFinished += Rotate;
            scatterViewTarget.ItemsSource = TargetItems;
            ModelManager.Instance.PropertyChanged += InstancePropertyChanged;
        }

        private void AddFilterHandlers()
        {
            CustomerFilter.EnergyFilterChecked += HandleEneryFilterChecked;
            CustomerFilter.EnergyFilterUnchecked += HandleEneryFilterUnchecked;

            CustomerFilter.CapgeminiFilterChecked += HandleCapgeminiFilterChecked;
            CustomerFilter.CapgeminiFilterUnchecked += HandleCapgeminiFilterUnchecked;

            CustomerFilter.OtherFilterChecked += HandleOtherFilterChecked;
            CustomerFilter.OtherFilterUnchecked += HandleOtherFilterUnchecked;

			CustomerFilter.NdcFilterChecked += HandleNdcFilterChecked;
			CustomerFilter.NdcFilterUnchecked += HandleNdcFilterUnchecked;
        }

        private void InitializeCard(Customer costumer)
        {
            var card = new MenuCard { DataContext = costumer };
            surfaceMainGrid.Children.Add(card);
            Grid.SetColumn(card, 0);
            Grid.SetRow(card, 0);
			card.cardRotateTransform.Angle=newAngle;
			newAngle+=25;
            Panel.SetZIndex(card, 2);
            MenuCardHolder.Add(card);
            card.ContactTapGesture += CardContactTapGesture;
            card.ContactDown += CardContactDown;
            card.scatCard.ScatterManipulationCompleted += CardScatterManipulationComp;
            card.SetZorder += CardContactDown;
        }

        private void surfaceWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeBT();
        }

        private void InitializeBT()
        {
            if (monitor == null)
            {
                try
                {
                    monitor = new BluetoothMonitor();
                }
                catch
                {
                    // Can't function without a bluetooth radio present so alert the user
                    Microsoft.Surface.UserNotifications.RequestNotification("Surface Bluetooth", "No Bluetooth Hardware Detected");
                    return;
                }

                // Data bind the list control to the current list of available devices
                DeviceList.ItemsSource = monitor.Devices;

                // Defines how long a single search pass will last
                monitor.DiscoveryDuration = new TimeSpan(0, 0, 5);
                // Defines how long to wait between searches
                monitor.IdleDuration = new TimeSpan(0, 0, 5);
                monitor.DiscoveryStarted += new EventHandler(monitor_DiscoveryStarted);
                monitor.DiscoveryCompleted += new EventHandler(monitor_DiscoveryCompleted);
                // Show the Surface's Bluetooth radio name on screen
                RadioNameText.Text = monitor.RadioFriendlyName;
            }

            // Starts listening loop for detecting nearby devices
            monitor.StartDiscovery();
        }
        #endregion

        #region Rotation
        
        private void InitializeManipulationProcessor()
        {
            _manipulationProcessor = new Affine2DManipulationProcessor(Affine2DManipulations.Rotate, particleSystem, _centerPoint);
            _manipulationProcessor.Affine2DManipulationDelta += OnManipulationDelta;
        }

        public void OnManipulationDelta(object sender, Affine2DOperationDeltaEventArgs e)
        {
            particleSystem.particleGridTransform.Angle += e.RotationDelta;
        }

        private void Rotate(object sender, Affine2DOperationDeltaEventArgs eventArgs)
        {
            particleSystem.particleGridTransform.Angle += eventArgs.RotationDelta;

            foreach (MenuCard card in MenuCardHolder)
            {
                card.OnManipulationDelta(sender, eventArgs);
            }

            particleSystem.ToggleControlPanel();
        }

        #endregion

        #region SurfaceInteraction
        
        private void CardScatterManipulationComp(object sender, ScatterManipulationCompletedEventArgs e)
        {
            _targetItems.Clear();

            foreach (MenuCard card in MenuCardHolder)
            {
                card.FadeInCardAnimation();
            }
            CurrentState = States.AllCardRotation;

            var hideFavouriteStack = (Storyboard)FindResource("HideFavouriteStack");
            hideFavouriteStack.Begin();

            particleSystem.SetSpeedSlider(15.0);
        }

        private void CardContactDown(object sender, ContactEventArgs e)
        {
            var obj = sender as MenuCard;

            if (obj != null && CurrentState.Equals(States.AllCardRotation))
            {
                foreach (MenuCard card in MenuCardHolder)	
                {
                    Panel.SetZIndex(card, card.Equals(obj) ? 2 : 1);
                }
                obj.AfterContactdown(e);
            }
        }

        private void CardContactTapGesture(object sender, ContactEventArgs e)
        {
            var obj = sender as MenuCard;

            if (obj != null && CurrentState.Equals(States.AllCardRotation))
            {
                foreach (MenuCard card in MenuCardHolder)
                {
                    card.FadeOutCardAnimation();
                }

                if (obj.CurrentState.Equals(MenuCard.States.StateRotation))
                {
                    CurrentState = States.OneCardDocked;

                    var revealFavouriteStack = (Storyboard) FindResource("RevealFavouriteStack");

                    if (revealFavouriteStack != null)
                    {
                        revealFavouriteStack.Begin();
                    }

                    particleSystem.SetSpeedSlider(5.0);
                }
                obj.AfterOnTapGesture(e);
            }
        }

        private void ScatterViewDragEnter(object sender, SurfaceDragDropEventArgs e)
        {
            e.Cursor.Visual.Tag = "DragEnter";
        }

        private void ScatterViewDragLeave(object sender, SurfaceDragDropEventArgs e)
        {
            e.Cursor.Visual.Tag = null;
        }

        private void ScatterViewDrop(object sender, SurfaceDragDropEventArgs e)
        {
            TargetItems.Add(e.Cursor.Data as ContentItem);
            var item = scatterViewTarget.Items[scatterViewTarget.Items.Count - 1];
            dropPoint = e.Cursor.GetPosition(scatterViewTarget);
            dropOrientation = e.Cursor.GetOrientation(scatterViewTarget);
			dropScatHeight = e.Cursor.Visual.Height;
			dropScatWidth = e.Cursor.Visual.Width;
            isSendingAfterDrop = true;
            scatterViewTarget.Activate(item);
            favouriteStack.RemoveInstancePropertyObject(item);

        }

        private void SurfaceToggleButtonChecked(object sender, RoutedEventArgs e)
        {
            var animation = (Storyboard)FindResource("ShowLogoStack");
            animation.Begin();

            particleSystem.GenerateParticles = true;

            foreach (MenuCard card in MenuCardHolder)
            {
                card.Opacity = 1;
                card.IsEnabled = true;
                Panel.SetZIndex(card, 2);
            }
        }

        private void SurfaceToggleButtonUnchecked(object sender, RoutedEventArgs e)
        {
            var animation = (Storyboard)FindResource("HideLogoStack");
            animation.Begin();

            particleSystem.GenerateParticles = false;

            foreach (MenuCard card in MenuCardHolder)
            {
                card.Opacity = 0;
                card.IsEnabled = false;
                Panel.SetZIndex(card, -10);
            }
        }

        #endregion

        #region FilterHandling
        
        void InstancePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ModelManager.Instance.SelectedCustomer == null)
                _targetItems.Clear();
            
        }

        private static void HandleEneryFilterUnchecked(object sender, EventArgs e)
        {
            foreach (var energyCustomer in ModelManager.Instance.EnergyCustomers)
            {
                energyCustomer.IsVisible = false;
            }
        }

        private static void HandleEneryFilterChecked(object sender, EventArgs e)
        {
            foreach (var energyCustomer in ModelManager.Instance.EnergyCustomers)
            {
                energyCustomer.IsVisible = true;
            }
        }

        private static void HandleOtherFilterChecked(object sender, EventArgs e)
        {
            foreach (var otherCustomer in ModelManager.Instance.OtherCustomers)
            {
                otherCustomer.IsVisible = true;
            }
        }

        private static void HandleOtherFilterUnchecked(object sender, EventArgs e)
        {
            foreach (var otherCustomer in ModelManager.Instance.OtherCustomers)
            {
                otherCustomer.IsVisible = false;
            }
        }

        private static void HandleCapgeminiFilterChecked(object sender, EventArgs e)
        {
            foreach (var capgeminiCustomer in ModelManager.Instance.CapgeminiInfo)
            {
                capgeminiCustomer.IsVisible = true;
            }
        }

        private static void HandleCapgeminiFilterUnchecked(object sender, EventArgs e)
        {
            foreach (var capgeminiCustomer in ModelManager.Instance.CapgeminiInfo)
            {
                capgeminiCustomer.IsVisible = false;
            }
        }

		private static void HandleNdcFilterChecked(object sender, EventArgs e)
		{
			foreach (var ndcCustomer in ModelManager.Instance.NdcInfo)
			{
				ndcCustomer.IsVisible = true;
			}
		}

		private static void HandleNdcFilterUnchecked(object sender, EventArgs e)
		{
			foreach (var ndcCustomer in ModelManager.Instance.NdcInfo)
			{
				ndcCustomer.IsVisible = false;
			}
		}

        #endregion

        private void scatterViewTarget_Activated(object sender, RoutedEventArgs e)
        {
            if (!isSendingAfterDrop) return;
            (e.OriginalSource as ScatterViewItem).Center = dropPoint;
            (e.OriginalSource as ScatterViewItem).Orientation = dropOrientation;
			if (dropScatHeight < 100.0)
			{
				dropScatHeight=(dropScatHeight*1.5);
			}
			if (dropScatWidth < 150.0)
			{
				dropScatWidth=(dropScatWidth*1.5);
			}
			(e.OriginalSource as ScatterViewItem).Height = (dropScatHeight);
			(e.OriginalSource as ScatterViewItem).Width = (dropScatWidth);
            isSendingAfterDrop = false;
        }

        #region Bluetooth methods
        private void OnDropTargetDragEnterBT(object sender, SurfaceDragDropEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement)
            {
                FrameworkElement fe = e.OriginalSource as FrameworkElement;

                if (fe.DataContext is BluetoothDevice)
                {
                    // Drop target must be a device not the list control
                    e.Cursor.Visual.Tag = "DragEnter";
                }
            }
        }

        void monitor_DiscoveryCompleted(object sender, EventArgs e)
        {
            // Hides the discovery animation
            this.Dispatcher.Invoke(new UpdateVisibilityDelegate(UpdateVisibility), Visibility.Hidden);
        }

        void monitor_DiscoveryStarted(object sender, EventArgs e)
        {
            // Shows the discovery animation
            this.Dispatcher.Invoke(new UpdateVisibilityDelegate(UpdateVisibility), Visibility.Visible);
        }

        delegate void UpdateVisibilityDelegate(Visibility v);

        void UpdateVisibility(Visibility v)
        {
            // Additionally display a text description of what the surface is currently doing
            if (v == Visibility.Visible)
            {
                DiscoveryStatusText.Text = "listening for Bluetooth devices...";
            }
            else
            {
                DiscoveryStatusText.Text = "waiting...";
            }
        }

        private void OnDropTargetDragLeaveBT(object sender, SurfaceDragDropEventArgs e)
        {
            e.Cursor.Visual.Tag = "Dragging";
        }

        // Runs in a threadpool thread and performs the actual obex exchange
        private void BeamObject(object context)
        {
            ObexWebRequest owr = context as ObexWebRequest;

            try
            {
                InTheHand.Net.ObexWebResponse response = (InTheHand.Net.ObexWebResponse)owr.GetResponse();

                // Remove once-off pairing
                BluetoothSecurity.RemoveDevice(BluetoothAddress.Parse(owr.RequestUri.Host));
            }
            catch (System.Net.WebException we)
            {
                System.Diagnostics.Debug.WriteLine(we.ToString());
            }
            finally
            {
                // Restart discovery for new devices
                monitor.StartDiscovery();
            }
        }

        private void OnDropBT(object sender, SurfaceDragDropEventArgs e)
        {
            FrameworkElement element = e.OriginalSource as FrameworkElement;
            if (element != null)
            {
                if (element.DataContext is BluetoothDevice)
                {
                    // Target is a Bluetooth device
                    BluetoothDevice device = element.DataContext as BluetoothDevice;

                    if (e.Cursor.Data is ContentItem)
                    {
                        ContentItem ci = e.Cursor.Data as ContentItem;
                        ObexItem oi = null;
                        if (ci.IsPictureItem)
                        {
                            oi = new ObexImage() { ImageUri = new Uri("pack://application:,,,/" + ci.FileName) };                            
                        } 
                        else if (ci.IsVisitItem)
                        {
                            oi = new ObexContactItem() 
                            {
                                FirstName = ci.Name, 
                                LastName = ci.FileName, 
                                EmailAddress = ci.Email,
                                MobileTelephoneNumber = ci.Tlf
                            };
                        }
                        if (oi != null)
                        { 
                            // Pause discovery as it interferes with/slows down beam process
                            monitor.StopDiscovery();

                            // Create the new request and write the contact details
                            ObexWebRequest owr = new ObexWebRequest(new Uri("obex://" + device.DeviceAddress.ToString() + "/" + oi.FileName));
                            System.IO.Stream s = owr.GetRequestStream();
                            oi.WriteToStream(s);

                            owr.ContentType = oi.ContentType;
                            owr.ContentLength = s.Length;
                            s.Close();

                            // Beam the item on new thread
                            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(BeamObject), owr);
                        }
                        // Return item to the scatter view
                        TargetItems.Add(e.Cursor.Data as ContentItem);
                        var item = scatterViewTarget.Items[scatterViewTarget.Items.Count - 1];
                        favouriteStack.RemoveInstancePropertyObject(item);
                    }

                }

                // Otherwise not supported
            }
        }
        #endregion

        #region Closing window
        private void surfaceWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            if (monitor != null)
            {
                monitor.StopDiscovery();
            }
        }

        private void surfaceWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (monitor != null)
            {
                monitor.StopDiscovery();
            }
        }
        #endregion
    }
}