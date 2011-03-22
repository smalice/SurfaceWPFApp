using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Manipulations;

namespace CapgeminiSurface
{

    public partial class MenuCard : SurfaceUserControl
    {
        Point centerPoint = new Point(512, 384);

        double currentRotationDelta;

        private Affine2DManipulationProcessor manipulationProcessor;

        public enum States : int { stateRotation = 0, stateUnlocked = 1, stateThree = 2, stateFour = 3 };

        public States CurrentState = States.stateRotation;

        public MenuCard()
        {
            InitializeComponent();
            InitializeManipulationProcessor();
            scatCard.Orientation = 0;
            CurrentState = States.stateRotation;
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
            base.OnContactDown(e);

            e.Contact.Capture(this);

            manipulationProcessor.BeginTrack(e.Contact);

            //e.Handled = true;
        }

        protected override void OnContactTapGesture(ContactEventArgs e)
        {
          if (cardRotateTransform.Angle < 90 && cardRotateTransform.Angle > 0)
           {
               // cardRotateTransform.Angle = 45;
           }
            
        //    //e.Handled = true;
        }

        private void scatCard_ContactTapGesture(object sender, ContactEventArgs e)
        {            
            //e.Handled = true;
        }

    }
}
