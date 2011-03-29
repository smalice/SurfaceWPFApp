using System;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using System.Collections;
using System.Windows.Media.Animation;
using Microsoft.Surface.Presentation.Manipulations;

namespace CapgeminiSurface
{
    public partial class CapgeminiSurfaceWindow : SurfaceWindow
    {
        private Random randomStartAngle = new Random();

        public ArrayList MenuCardHolder = new ArrayList();

        public CapgeminiSurfaceWindow()
        {
            Model.ModelManager.Instance.Load();
            InitializeComponent();
            AddActivationHandlers();

            foreach (var costumer in Model.ModelManager.Instance.AllCustomers)
            {
                MenuCard card = new MenuCard();
                card.DataContext = costumer;
                surfaceMainGrid.Children.Add(card);
                Grid.SetColumn(card, 0);
                Grid.SetRow(card, 0);
                Panel.SetZIndex(card, 0);
                card.cardRotateTransform.Angle = randomStartAngle.Next(0, 360);
                MenuCardHolder.Add(card);
                card.ContactTapGesture += new ContactEventHandler(card_ContactTapGesture);
                card.scatCard.ScatterManipulationCompleted += new ScatterManipulationCompletedEventHandler(card_scatterManipulationComp);
            }

            Logo.DeltaManipulationFinished += Rotate;
        }

        private void Rotate(object sender, Affine2DOperationDeltaEventArgs eventArgs)
        {
            foreach (MenuCard card in MenuCardHolder)
            {
                card.OnManipulationDelta(sender, eventArgs);
            }

            particleSystem.particlePoint3D = new Point3D(Math.Cos(eventArgs.RotationDelta), eventArgs.RotationDelta, 0.0);
        }

        void card_scatterManipulationComp(object sender, ScatterManipulationCompletedEventArgs e)
        {
            Storyboard HideFavouriteStack = (Storyboard)FindResource("HideFavouriteStack");

            HideFavouriteStack.Begin();

            particleSystem.particleSpeed = 30.0;
        }

        void card_ContactTapGesture(object sender, ContactEventArgs e)
        {
            var obj = sender as MenuCard;

            if (obj.CurrentState.Equals( MenuCard.States.stateRotation ) )
            {
                Storyboard RevealFavouriteStack = (Storyboard)FindResource("RevealFavouriteStack");

                RevealFavouriteStack.Begin();

                obj.CurrentState = MenuCard.States.stateUnlocked;

                particleSystem.particleSpeed = 5.0;
            }

        }

        protected override void OnClosed(EventArgs e)        {
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
    }
}