using System.Windows;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Manipulations;

namespace CapgeminiSurface
{

    public partial class MenuCard : SurfaceUserControl
    {
        Point centerPoint = new Point(512, 384);

        private Affine2DManipulationProcessor manipulationProcessor;

        public enum States : int 
            {   stateRotation = 0, 
                stateUnlocked = 1, 
            };

        public States CurrentState = States.stateRotation;

        public static bool cardOut;

        public MenuCard()
        {
            InitializeComponent();
            InitializeManipulationProcessor();
            scatCard.Orientation = 0;
            CurrentState = States.stateRotation;
            cardOut = false;
        }

        private void InitializeManipulationProcessor()
        {
            manipulationProcessor = new Affine2DManipulationProcessor(Affine2DManipulations.Rotate, rotationGrid, centerPoint);
            manipulationProcessor.Affine2DManipulationDelta += OnManipulationDelta;
        }

        public void OnManipulationDelta(object sender, Affine2DOperationDeltaEventArgs e)
        {
            cardRotateTransform.Angle += e.RotationDelta;  
        }

        protected override void OnContactDown(ContactEventArgs e)
        {
            if (CurrentState.Equals(States.stateRotation))
            {
                base.OnContactDown(e);

                e.Contact.Capture(this);

                manipulationProcessor.BeginTrack(e.Contact);

                e.Handled = true;
            }
            else 
            {
                e.Handled = true;
            }
        }

        protected override void OnContactTapGesture(ContactEventArgs e)
        {
            if (CurrentState.Equals(States.stateRotation) && !cardOut)
            {
                cardOut = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void scatCard_ScatterManipulationCompleted(object sender, ScatterManipulationCompletedEventArgs e)
        {
            if (CurrentState.Equals(States.stateUnlocked) && cardOut)
            {
                cardOut = false;

                CurrentState = States.stateRotation;
            }
            else 
            {
                e.Handled = true;
            }
        }
    }
}
