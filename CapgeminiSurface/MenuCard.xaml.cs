using System;
using System.Windows;
using System.Windows.Media.Animation;
using CapgeminiSurface.Model;
using CapgeminiSurface.Util;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Manipulations;

namespace CapgeminiSurface
{

    public partial class MenuCard : SurfaceUserControl
    {
        readonly Point _centerPoint = new Point(512, 384);

        private Affine2DManipulationProcessor _manipulationProcessor;

        public EventHandler<ContactEventArgs> SetZorder;

        public enum States : int 
            {   StateRotation = 0, 
                StateUnlocked = 1, 
            };

        public States CurrentState = States.StateRotation;

        public static bool CardOut;

        public MenuCard()
        {
            InitializeComponent();
            InitializeManipulationProcessor();
            scatCard.Orientation = 0;
            CurrentState = States.StateRotation;
            CardOut = false;
        }

        private void InitializeManipulationProcessor()
        {
            _manipulationProcessor = new Affine2DManipulationProcessor(Affine2DManipulations.Rotate, rotationGrid, _centerPoint);
            _manipulationProcessor.Affine2DManipulationDelta += OnManipulationDelta;
        }

        public void OnManipulationDelta(object sender, Affine2DOperationDeltaEventArgs e)
        {
            cardRotateTransform.Angle += e.RotationDelta;  
        }

        protected override void OnContactDown(ContactEventArgs e)
        {
            if (!CurrentState.Equals(States.StateRotation))
            {
                e.Handled = true;
            }

            SetZorder(this, e);
        }

        public void AfterContactdown(ContactEventArgs e)
        {
            if (CurrentState.Equals(States.StateRotation))
            {
                Storyboard tapTheCard = (Storyboard)FindResource("TapTheCard");
                tapTheCard.Remove();
                tapTheCard.Begin();

                base.OnContactDown(e);

                e.Contact.Capture(this);

                _manipulationProcessor.BeginTrack(e.Contact);

                e.Handled = true;
            }
        }

        protected override void OnContactTapGesture(ContactEventArgs e)
        {
            if (!CurrentState.Equals(States.StateRotation) && CardOut)
            {
                e.Handled = true;
            }

            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();

            ModelManager.Instance.SelectedCustomer = this.DataContext as Customer;
        }

        public void AfterOnTapGesture(ContactEventArgs e)
        {
            Storyboard tapTheCard = (Storyboard)FindResource("TapTheCard");
            tapTheCard.Remove();

            Storyboard dragOutCard = (Storyboard)FindResource("dragOut");
            dragOutCard.Remove();
            dragOutCard.Begin();

            Storyboard cardIsOut = (Storyboard)FindResource("CardIsOut");
            cardIsOut.Remove();
            cardIsOut.Begin();
            
            CardOut = true;

            CurrentState = MenuCard.States.StateUnlocked;
        }

        private void ScatCardScatterManipulationCompleted(object sender, ScatterManipulationCompletedEventArgs e)
        {
            if (CurrentState.Equals(States.StateUnlocked) && CardOut)
            {
                Storyboard cardIsOut = (Storyboard)FindResource("CardIsOut");
                cardIsOut.Remove();

                CardOut = false;

                CurrentState = States.StateRotation;
            }
            else
                e.Handled = true;
        }

        private void ScatCardActivated(object sender, RoutedEventArgs e)
        {

        }

        public void FadeInCardAnimation()
        {
            Storyboard fadeOutCard = (Storyboard)FindResource("FadeOutCard");
            fadeOutCard.Remove();
            Storyboard fadeInCard = (Storyboard)FindResource("FadeInCard");
            fadeInCard.Begin();
        }

        public void FadeOutCardAnimation()
        {
            Storyboard fadeInCard = (Storyboard)FindResource("FadeInCard");
            fadeInCard.Remove();
            Storyboard fadeOutCard = (Storyboard)FindResource("FadeOutCard");
            fadeOutCard.Begin();
        }
    }
}
