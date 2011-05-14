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

        public CapgeminiSurfaceWindow()
        {
            ModelManager.Instance.Load();
            InitializeComponent();
            InitializeManipulationProcessor();
			AddFilterHandlers();

			newAngle = _randomStartAngle.Next(0, 360);
			
            foreach (Customer costumer in ModelManager.Instance.AllCustomers)
            {
                InitializeCard(costumer);
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

        #endregion

        private void scatterViewTarget_Activated(object sender, RoutedEventArgs e)
        {
            if (!isSendingAfterDrop) return;
            (e.OriginalSource as ScatterViewItem).Center = dropPoint;
            (e.OriginalSource as ScatterViewItem).Orientation = dropOrientation;
			(e.OriginalSource as ScatterViewItem).Height = dropScatHeight;
			(e.OriginalSource as ScatterViewItem).Width = dropScatWidth;
            isSendingAfterDrop = false;
        }
    }
}