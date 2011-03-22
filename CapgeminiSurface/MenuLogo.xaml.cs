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
    public partial class MenuLogo : SurfaceUserControl

    {
        Point centerPoint = new Point(150, 150);
         
        private Affine2DManipulationProcessor manipulationProcessor;

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
        }

        protected override void OnContactDown(ContactEventArgs e)
        {
            base.OnContactDown(e);

            e.Contact.Capture(this);

            manipulationProcessor.BeginTrack(e.Contact);

            //e.Handled = true;
        }

        public void rotateCards(double rotationValue)
        {
        
        }

    }
}
