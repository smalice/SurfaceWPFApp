using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;


namespace SurfaceApp
{
 
    public enum states{ stateOne,stateTwo,stateThree,stateFour };

    public partial class Card : UserControl
    {
        private double rotationSpeed = 1.5;
        private double radiusMod = 100;
        private double hypotenuseMod = 57.5;
        private Point currentCenterPoint = new Point(280, 180);

        public Card()
        {
            InitializeComponent();
        }

        private void card_ScatterManipulationStarted(object sender, Microsoft.Surface.Presentation.Controls.ScatterManipulationStartedEventArgs e)
        {

        }

        private void Card1_ScatterManipulationDelta(object sender, ScatterManipulationDeltaEventArgs e)
        {
            var scatterViewItem = sender as ScatterViewItem;

            if (scatterViewItem != null)
            {
                bool directionMod=true;

                // #FELO: Decide how draging will move object
                if (scatterViewItem.Orientation > 360 || scatterViewItem.Orientation < 90+1)
                {
                    if (e.HorizontalChange + e.VerticalChange > 0)
                    {
                        directionMod=true;
                    }
                    else
                    {
                        directionMod=false;
                    }
                }
                else if (scatterViewItem.Orientation > 90 && scatterViewItem.Orientation < 180+1)
                {
                    if (e.HorizontalChange + -e.VerticalChange < 0)
                    {
                        directionMod = true;
                    }
                    else
                    {
                        directionMod = false;
                    }
                }
                else if (scatterViewItem.Orientation > 180 && scatterViewItem.Orientation < 270+1)
                {
                    if (e.HorizontalChange + e.VerticalChange < 0)
                    {
                        directionMod = true;
                    }
                    else
                    {
                        directionMod = false;
                    }
                }
                else if (scatterViewItem.Orientation > 270 && scatterViewItem.Orientation < 360+1)
                {
                    if (e.HorizontalChange + -e.VerticalChange > 0)
                    {
                        directionMod = true;
                    }
                    else
                    {
                        directionMod = false;
                    }
                }

                // #FELO: Reset Original translation
                moveObjectReset(sender, new Point(-e.HorizontalChange, -e.VerticalChange));                
                
                if (directionMod)
                {
                    scatterViewItem.Orientation += rotationSpeed;
                }
                else
                {
                    scatterViewItem.Orientation -= rotationSpeed;
                }
                
                double orient = scatterViewItem.Orientation;
                moveObject(sender, new Point(radiusMod * Math.Sin(orient / hypotenuseMod), radiusMod * Math.Cos(orient / hypotenuseMod)));
                
                // #FELO: Reset Orientation - for circle calibration
                if (scatterViewItem.Orientation > 360)
                {
                    scatterViewItem.Orientation = 0;
                }
                else if (scatterViewItem.Orientation < 0)
                {
                    scatterViewItem.Orientation = 360;
                }

            }

        }


        private void moveObjectReset(object sender, Point movement)
        {
            var scatterViewItem = sender as ScatterViewItem;
            if (scatterViewItem != null)
            {
                double posX = scatterViewItem.Center.X;
                double posY = scatterViewItem.Center.Y;
                scatterViewItem.Center = new Point(posX + movement.X, posY - movement.Y);
            }            
        }

        private void moveObject(object sender, Point movement)
        {
            var scatterViewItem = sender as ScatterViewItem;
            if (scatterViewItem != null)
            {
                double posX = currentCenterPoint.X;
                double posY = currentCenterPoint.Y;
                scatterViewItem.Center = new Point(posX + movement.X, posY - movement.Y);
            }
        }

        private void card_ScatterManipulationCompleted(object sender, Microsoft.Surface.Presentation.Controls.ScatterManipulationCompletedEventArgs e)
        {

        }

        private void card_TextInput(object sender, TextCompositionEventArgs e)
        {
            
        }
    }
}
