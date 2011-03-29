using System;
using System.Windows;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Manipulations;

namespace CapgeminiSurface
{
    public partial class MenuLogo : SurfaceUserControl

    {
        readonly Point centerPoint = new Point(150, 150);
         
        private Affine2DManipulationProcessor manipulationProcessor;

        public EventHandler<Affine2DOperationDeltaEventArgs> DeltaManipulationFinished;

        public MenuLogo()
        {
            InitializeComponent();
            InitializeManipulationProcessor(); 
        }

        private void InitializeManipulationProcessor()
        {
            manipulationProcessor = new Affine2DManipulationProcessor(Affine2DManipulations.Rotate, logo, centerPoint);
            manipulationProcessor.Affine2DManipulationDelta += OnManipulationDelta;
        }

        private void OnManipulationDelta(object sender, Affine2DOperationDeltaEventArgs e)
        {
            logoRotateTransform.Angle += e.RotationDelta;
            rotateCards(e.RotationDelta);
            DeltaManipulationFinished(this, e);
        }

        protected override void OnContactDown(ContactEventArgs e)
        {
            base.OnContactDown(e);

            e.Contact.Capture(this);

            manipulationProcessor.BeginTrack(e.Contact);

            e.Handled = true;
        }

        public void rotateCards(double rotationValue)
        {
            
        }

    }
}
