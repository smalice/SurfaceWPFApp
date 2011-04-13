using System;
using System.Windows;
using CapgeminiSurface.Util;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Manipulations;
using System.Windows.Media.Animation;

namespace CapgeminiSurface
{
    public partial class MenuLogo

    {
        #region Initialization
        
        readonly Point _centerPoint = new Point(150, 150);
         
        private Affine2DManipulationProcessor _manipulationProcessor;

        public EventHandler<Affine2DOperationDeltaEventArgs> DeltaManipulationFinished;

        public MenuLogo()
        {
            InitializeComponent();
            InitializeManipulationProcessor();
        }

        #endregion

        #region Rotation
        
        private void InitializeManipulationProcessor()
        {
            _manipulationProcessor = new Affine2DManipulationProcessor(Affine2DManipulations.Rotate, logo, _centerPoint);
            _manipulationProcessor.Affine2DManipulationDelta += OnManipulationDelta;
        }

        private void OnManipulationDelta(object sender, Affine2DOperationDeltaEventArgs e)
        {
            logoRotateTransform.Angle += e.RotationDelta;
            DeltaManipulationFinished(this, e);            
        }

        #endregion

        #region SurfaceInteraction
        
        protected override void OnContactDown(ContactEventArgs e)
        {
            base.OnContactDown(e);

            e.Contact.Capture(this);

            _manipulationProcessor.BeginTrack(e.Contact);

            e.Handled = true;
        }

        protected override void OnContactTapGesture(ContactEventArgs e)
        {
            var textFadeOut = (Storyboard)FindResource("TextFadeOut");

            textFadeOut.Begin();

            new ThreadedSoundPlayer(Properties.Resources.Tap).PlaySound();
        }

        #endregion
    }
}
