using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using CapgeminiSurface.Model;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Manipulations;
using System.Collections.ObjectModel;

namespace CapgeminiSurface
{
    public partial class CapgeminiSurfaceWindow : SurfaceWindow
    {
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

        private ObservableCollection<ContentItem> targetItems;

        public ObservableCollection<ContentItem> TargetItems
        {
            get
            {
                if (targetItems == null)
                {
                    targetItems = new ObservableCollection<ContentItem>();
                }

                return targetItems;
            }
        }

        public CapgeminiSurfaceWindow()
        {
            ModelManager.Instance.Load();
            InitializeComponent();
            AddActivationHandlers();
            InitializeManipulationProcessor();
			AddFilterHandlers();

            foreach (Customer costumer in ModelManager.Instance.AllCustomers)
            {
                var card = new MenuCard {DataContext = costumer};
                surfaceMainGrid.Children.Add(card);
                Grid.SetColumn(card, 0);
                Grid.SetRow(card, 0);
                Panel.SetZIndex(card, 2);
                card.cardRotateTransform.Angle = _randomStartAngle.Next(0, 360);
                MenuCardHolder.Add(card);
                card.ContactTapGesture += CardContactTapGesture;
                card.ContactDown += CardContactDown;
                card.scatCard.Activated += ScatCardActivated;
                card.scatCard.ScatterManipulationCompleted += CardScatterManipulationComp;
                card.SetZorder += CardContactDown;
            }
            Logo.DeltaManipulationFinished += Rotate;
            scatterViewTarget.ItemsSource = TargetItems;
            ModelManager.Instance.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Instance_PropertyChanged);
        }

        void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ModelManager.Instance.SelectedCustomer == null)
                targetItems.Clear();
        }
		
		private void AddFilterHandlers()
    	{
    		this.CustomerFilter.EnergyFilterChecked += HandleEneryFilterChecked;
    		this.CustomerFilter.EnergyFilterUnchecked += HandleEneryFilterUnchecked;

			this.CustomerFilter.CapgeminiFilterChecked += HandleCapgeminiFilterChecked;
			this.CustomerFilter.CapgeminiFilterUnchecked += HandleCapgeminiFilterUnchecked;

			this.CustomerFilter.OtherFilterChecked += HandleOtherFilterChecked;
			this.CustomerFilter.OtherFilterUnchecked += HandleOtherFilterUnchecked;
    	}

        private void InitializeManipulationProcessor()
        {
            _manipulationProcessor = new Affine2DManipulationProcessor(Affine2DManipulations.Rotate, particleSystem , _centerPoint );
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

            particleSystem.toggleControlPanel();
        }

        private void CardScatterManipulationComp(object sender, ScatterManipulationCompletedEventArgs e)
        {
            foreach (MenuCard card in MenuCardHolder)
            {
                card.FadeInCardAnimation();
            }
            CurrentState = States.AllCardRotation;

            var hideFavouriteStack = (Storyboard)FindResource("HideFavouriteStack");

            hideFavouriteStack.Begin();

            particleSystem.setSpeedSlider(15.0);
        }

        private void ScatCardActivated(object sender, RoutedEventArgs e)
        {
            
        }

        private void CardContactDown(object sender, ContactEventArgs e)
        {
            var obj = sender as MenuCard;

            if (obj != null && CurrentState.Equals(States.AllCardRotation))
            {          
                foreach (MenuCard card in MenuCardHolder)	
                {
                    if (card.Equals(obj))
                    {
                        Panel.SetZIndex(card, 2);		
                    }   
                    else
                    {
                        Panel.SetZIndex(card, 1);               
                    }
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

                    particleSystem.setSpeedSlider(5.0);
                }
                obj.AfterOnTapGesture(e);
            }

        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            RemoveActivationHandlers();
        }

        private void AddActivationHandlers()
        {
            ApplicationLauncher.ApplicationActivated += OnApplicationActivated;
            ApplicationLauncher.ApplicationPreviewed += OnApplicationPreviewed;
            ApplicationLauncher.ApplicationDeactivated += OnApplicationDeactivated;
        }

        private void RemoveActivationHandlers()
        {
            ApplicationLauncher.ApplicationActivated -= OnApplicationActivated;
            ApplicationLauncher.ApplicationPreviewed -= OnApplicationPreviewed;
            ApplicationLauncher.ApplicationDeactivated -= OnApplicationDeactivated;
        }

        private void OnApplicationActivated(object sender, EventArgs e)
        {
        }

        private void OnApplicationPreviewed(object sender, EventArgs e)
        {
        }

        private void OnApplicationDeactivated(object sender, EventArgs e)
        {
        }

        private void HandleEneryFilterUnchecked(object sender, EventArgs e)
        {
            foreach (var energyCustomer in ModelManager.Instance.EnergyCustomers)
            {
                energyCustomer.IsVisible = false;
            }
        }

        private void HandleEneryFilterChecked(object sender, EventArgs e)
        {
            foreach (var energyCustomer in ModelManager.Instance.EnergyCustomers)
            {
                energyCustomer.IsVisible = true;
            }
        }

        private void HandleOtherFilterChecked(object sender, EventArgs e)
        {
            foreach (var otherCustomer in ModelManager.Instance.OtherCustomers)
            {
                otherCustomer.IsVisible = true;
            }
        }

        private void HandleOtherFilterUnchecked(object sender, EventArgs e)
        {
            foreach (var otherCustomer in ModelManager.Instance.OtherCustomers)
            {
                otherCustomer.IsVisible = false;
            }
        }

        private void HandleCapgeminiFilterChecked(object sender, EventArgs e)
        {
            foreach (var capgeminiCustomer in ModelManager.Instance.CapgeminiInfo)
            {
                capgeminiCustomer.IsVisible = true;
            }
        }

        private void HandleCapgeminiFilterUnchecked(object sender, EventArgs e)
        {
            foreach (var capgeminiCustomer in ModelManager.Instance.CapgeminiInfo)
            {
                capgeminiCustomer.IsVisible = false;
            }
        }

        private void ScatterView_DragEnter(object sender, SurfaceDragDropEventArgs e)
        {
            e.Cursor.Visual.Tag = "DragEnter";
        }

        private void ScatterView_DragLeave(object sender, SurfaceDragDropEventArgs e)
        {
            e.Cursor.Visual.Tag = null;
        }

        private void ScatterView_Drop(object sender, SurfaceDragDropEventArgs e)
        {
            TargetItems.Add(e.Cursor.Data as ContentItem);
        }
    }
}