using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Animation;
using CapgeminiSurface.Model;
using CapgeminiSurface.Util;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Manipulations;

namespace CapgeminiSurface
{
    public partial class MenuCard
    {
        #region Initialization

        readonly Point _centerPoint = new Point(512, 384);

        public EventHandler<ContactEventArgs> SetZorder;

        public enum States
        {
            StateRotation = 0,
            StateUnlocked = 1,
        };

        public States CurrentState = States.StateRotation;

        public static bool OneCardOut = false;

        public static bool CardOut;

        public MenuCard()
        {
            InitializeComponent();
            InitializeManipulationProcessor();
            scatCard.Orientation = 0;
            CurrentState = States.StateRotation;
            CardOut = false;
        }

        #endregion

        #region Rotation

        private Affine2DManipulationProcessor _manipulationProcessor;

        /// <summary>
        /// Initializer for ManipulationProcessor of MenuCard
        /// </summary>
        /// <returns></returns>
        private void InitializeManipulationProcessor()
        {
            _manipulationProcessor = new Affine2DManipulationProcessor(Affine2DManipulations.Rotate, rotationGrid, _centerPoint);
            _manipulationProcessor.Affine2DManipulationDelta += OnManipulationDelta;
        }

        /// <summary>
        /// Rotation of MenuCard using ManipulationProcessor
        /// </summary>
        /// <returns></returns>
        public void OnManipulationDelta(object sender, Affine2DOperationDeltaEventArgs e)
        {
            cardRotateTransform.Angle += e.RotationDelta;
        }

        #endregion

        #region SurfaceInteraction

        /// <summary>
        /// Override of ContactDown on ManuCard.
        /// If Card is in rotation state the SetZorder method is called.
        /// </summary>
        /// <returns></returns>
        protected override void OnContactDown(ContactEventArgs e)
        {
            if (!CurrentState.Equals(States.StateRotation))
            {
                e.Handled = true;
            }
            
            SetZorder(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void AfterContactdown(ContactEventArgs e)
        {
            if (CurrentState.Equals(States.StateRotation))
            {
                AfterContactDownAnimation();

                base.OnContactDown(e);

                e.Contact.Capture(this);

                _manipulationProcessor.BeginTrack(e.Contact);

                e.Handled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void AfterTapReset()
        {
            OneCardOut = false;
        }

        /// <summary>
        /// Override of ContactTapGesture for MenuCard
        /// </summary>
        /// <returns></returns>
        protected override void OnContactTapGesture(ContactEventArgs e)
        {
            PlaySound(Properties.Resources.Tap);

            if (!CurrentState.Equals(States.StateRotation) && CardOut)
            {
                if (CurrentState.Equals(States.StateUnlocked) && CardOut)
                {
                    RemoveCurrentAnimation();

                    CardOut = false;

                    CurrentState = States.StateRotation;

                    ModelManager.Instance.SelectedCustomer = null;

                    OneCardOut = false;

                    OnTapAnimationIn();
                }
                else
                    e.Handled = true;
            }

            if (!CardOut && !OneCardOut)
            {
                ModelManager.Instance.SelectedCustomer = DataContext as Customer;
                OneCardOut = true;
            }
            else
            {
                e.Handled = true;
                return;

            }

        }

        public void AfterOnTapGesture(ContactEventArgs e)
        {
            OnTapAnimation();

            CardOut = true;

            CurrentState = States.StateUnlocked;
        }

        #endregion

        #region Animation

        private void RemoveCurrentAnimation()
        {
            PlayAnimation("CardIsOut", false, true);
        }

        private void OnTapAnimation()
        {
            PlayAnimation("TapTheCard", false, true);
            PlayAnimation("dragOut", true, true);
            PlayAnimation("CardIsOut", true, true);
        }

        private void OnTapAnimationIn()
        {

            PlayAnimation("dragOut", false, false);
            PlayAnimation("CardIsOut", false, false);
            PlayAnimation("dragIn", true, true);
        }

        public void FadeInCardAnimation()
        {
            PlayAnimation("FadeOutCard", false, true);
            PlayAnimation("FadeInCard", true, true);
        }

        public void FadeOutCardAnimation()
        {
            PlayAnimation("FadeInCard", false, true);
            PlayAnimation("FadeOutCard", true, true);
        }

        private void AfterContactDownAnimation()
        {
            PlayAnimation("TapTheCard", true, true);
        }

        private void PlayAnimation(String name, Boolean play, Boolean remove)
        {
            var animation = (Storyboard)FindResource(name);
            if (remove)
                animation.Remove();
            if (play)
                animation.Begin();
        }

        #endregion

        #region Sound

        private static void PlaySound(UnmanagedMemoryStream unmanagedMemoryStream)
        {
            new ThreadedSoundPlayer(unmanagedMemoryStream).PlaySound();
        }

        #endregion
    }
}