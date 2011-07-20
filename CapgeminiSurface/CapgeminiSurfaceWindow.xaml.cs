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

        private bool _isSendingAfterDrop;
        private Point _dropPoint;
        private double _dropOrientation;
        private double _dropScatHeight;
        private double _dropScatWidth;
        private double _newAngle;
        private double _maxHeight;
        private double _maxWidth;
        private double _minHeight;
        private double _minWidth;

        private BluetoothMonitor _monitor;

        public CapgeminiSurfaceWindow()
        {
            ModelManager.Instance.Load();
            InitializeComponent();
            InitializeManipulationProcessor();
            AddFilterHandlers();

            _newAngle = _randomStartAngle.Next(0, 360);

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
            card.cardRotateTransform.Angle = _newAngle;
            _newAngle += 25;
            Panel.SetZIndex(card, 2);
            MenuCardHolder.Add(card);
            card.ContactTapGesture += CardContactTapGesture;
            card.ContactDown += CardContactDown;
            card.SetZorder += CardContactDown;
        }

        private void SurfaceWindowLoaded(object sender, RoutedEventArgs e)
        {
            InitializeBt();
        }

        private void InitializeBt()
        {
            if (_monitor == null)
            {
                try
                {
                    _monitor = new BluetoothMonitor();
                }
                catch
                {
                    // Can't function without a bluetooth radio present so alert the user
                    Microsoft.Surface.UserNotifications.RequestNotification("Surface Bluetooth", "No Bluetooth Hardware Detected");
                    return;
                }

                // Data bind the list control to the current list of available devices
                DeviceList.ItemsSource = _monitor.Devices;

                // Defines how long a single search pass will last
                _monitor.DiscoveryDuration = new TimeSpan(0, 0, 5);
                // Defines how long to wait between searches
                _monitor.IdleDuration = new TimeSpan(0, 0, 5);
                _monitor.DiscoveryStarted += MonitorDiscoveryStarted;
                _monitor.DiscoveryCompleted += MonitorDiscoveryCompleted;
                // Show the Surface's Bluetooth radio name on screen
                RadioNameText.Text = _monitor.RadioFriendlyName;
            }

            // Starts listening loop for detecting nearby devices
            _monitor.StartDiscovery();
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

            bool check = false;
            if (obj != null && (!States.AllCardRotation.Equals(CurrentState)))
            {

                _targetItems.Clear();

                foreach (MenuCard card in MenuCardHolder)
                {
                    card.FadeInCardAnimation();

                }
                CurrentState = States.AllCardRotation;

                var hideFavouriteStack = (Storyboard)FindResource("HideFavouriteStack");
                hideFavouriteStack.Remove();
                hideFavouriteStack.Begin();

                particleSystem.SetSpeedSlider(15.0);

                obj.AfterTapReset();

                e.Handled = true;
                check = true;

            }

            if (obj != null && CurrentState.Equals(States.AllCardRotation) && !check)
            {
                foreach (MenuCard card in MenuCardHolder)
                {
                    card.FadeOutCardAnimation();
                }

                if (obj.CurrentState.Equals(MenuCard.States.StateRotation))
                {
                    CurrentState = States.OneCardDocked;

                    var revealFavouriteStack = (Storyboard)FindResource("RevealFavouriteStack");

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
            _dropPoint = e.Cursor.GetPosition(scatterViewTarget);

            _maxHeight = e.Cursor.Visual.MaxHeight;
            _maxWidth = e.Cursor.Visual.MaxWidth;
            _minHeight = e.Cursor.Visual.MinHeight;
            _minWidth = e.Cursor.Visual.MinWidth;

            _dropOrientation = e.Cursor.GetOrientation(scatterViewTarget);

            _dropScatHeight = e.Cursor.Visual.ActualHeight;
            _dropScatWidth = e.Cursor.Visual.ActualWidth;

            _isSendingAfterDrop = true;
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


        private void ScatterViewTargetActivated(object sender, RoutedEventArgs e)
        {
            if (!_isSendingAfterDrop) return;
            ((ScatterViewItem) e.OriginalSource).Center = _dropPoint;
            var newScatterItem = (e.OriginalSource as ScatterViewItem);
            newScatterItem.Orientation = _dropOrientation;
            newScatterItem.MaxHeight = _maxHeight;
            newScatterItem.MaxWidth = _maxWidth;
            newScatterItem.MinHeight = _minHeight;
            newScatterItem.MinWidth = _minWidth;
            newScatterItem.IsTopmostOnActivation = true;
            //newScatterItem.ZIndex = 100;
            if (_dropScatHeight < 100.0)
            {
                _dropScatHeight=(_dropScatHeight*1.5);
            }
            if (_dropScatWidth < 150.0)
            {
                _dropScatWidth=(_dropScatWidth*1.5);
            }

            (e.OriginalSource as ScatterViewItem).Height = (_dropScatHeight);
            (e.OriginalSource as ScatterViewItem).Width = (_dropScatWidth);
            _isSendingAfterDrop = false;
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

        #region Bluetooth methods
        private void OnDropTargetDragEnterBt(object sender, SurfaceDragDropEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement)
            {
                var fe = e.OriginalSource as FrameworkElement;

                if (fe.DataContext is BluetoothDevice)
                {
                    // Drop target must be a device not the list control
                    e.Cursor.Visual.Tag = "DragEnter";
                }
            }
        }

        void MonitorDiscoveryCompleted(object sender, EventArgs e)
        {
            // Hides the discovery animation
            Dispatcher.Invoke(new UpdateVisibilityDelegate(UpdateVisibility), Visibility.Hidden);
        }

        void MonitorDiscoveryStarted(object sender, EventArgs e)
        {
            // Shows the discovery animation
            Dispatcher.Invoke(new UpdateVisibilityDelegate(UpdateVisibility), Visibility.Visible);
        }

        delegate void UpdateVisibilityDelegate(Visibility v);

        void UpdateVisibility(Visibility v)
        {
            // Additionally display a text description of what the surface is currently doing
            DiscoveryStatusText.Text = v == Visibility.Visible ? "listening for Bluetooth devices..." : "waiting...";
        }

        private void OnDropTargetDragLeaveBt(object sender, SurfaceDragDropEventArgs e)
        {
            e.Cursor.Visual.Tag = "Dragging";
        }

        // Runs in a threadpool thread and performs the actual obex exchange
        private void BeamObject(object context)
        {
            var owr = context as ObexWebRequest;

            try
            {
                var response = (InTheHand.Net.ObexWebResponse)owr.GetResponse();

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
                _monitor.StartDiscovery();
            }
        }

        private void OnDropBT(object sender, SurfaceDragDropEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            if (element != null)
            {
                if (element.DataContext is BluetoothDevice)
                {
                    // Target is a Bluetooth device
                    var device = element.DataContext as BluetoothDevice;

                    if (e.Cursor.Data is ContentItem)
                    {
                        var ci = e.Cursor.Data as ContentItem;
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
                            _monitor.StopDiscovery();

                            // Create the new request and write the contact details
                            var owr = new ObexWebRequest(new Uri("obex://" + device.DeviceAddress.ToString() + "/" + oi.FileName));
                            System.IO.Stream s = owr.GetRequestStream();
                            oi.WriteToStream(s);

                            owr.ContentType = oi.ContentType;
                            owr.ContentLength = s.Length;
                            s.Close();

                            // Beam the item on new thread
                            System.Threading.ThreadPool.QueueUserWorkItem(BeamObject, owr);
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
        private void SurfaceWindowUnloaded(object sender, RoutedEventArgs e)
        {
            if (_monitor != null)
            {
                _monitor.StopDiscovery();
            }
        }

        private void SurfaceWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_monitor != null)
            {
                _monitor.StopDiscovery();
            }
        }
        #endregion

        #region BlueToothButton
        private void BTbuttonChecked(object sender, RoutedEventArgs e)
        {
            DeviceList.Visibility = Visibility.Visible;
            //RadioNameText.Visibility = Visibility.Visible;
            //DiscoveryStatusText.Visibility = Visibility.Visible;
        }

        private void BTbuttonUnchecked(object sender, RoutedEventArgs e)
        {
            DeviceList.Visibility = Visibility.Collapsed;
            RadioNameText.Visibility = Visibility.Collapsed;
            DiscoveryStatusText.Visibility = Visibility.Collapsed;
        }
        #endregion
    }
}