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
using Microsoft.Surface.Presentation.Controls;

namespace CapgeminiSurface
{
    
    public partial class MenuCard : UserControl
    {
        private double rotationSpeed = 1.0;
        private double radiusMod = 125;
        private double hypotenuseMod = 57.5;
        private double maximumRotationDegree = 360;
        private double minimalRotationDegree = 0;
        private Point currentCenterPoint = new Point(250, 250);

        public enum States : int { stateOne = 0, stateTwo = 1, stateThree = 2, stateFour = 3 };

        public States CurrentState=States.stateOne;
        
        public MenuCard()
        {
            InitializeComponent();
        }

        private void card_ScatterManipulationStarted(object sender, Microsoft.Surface.Presentation.Controls.ScatterManipulationStartedEventArgs e)
        {

        }

        private void card_ScatterManipulationDelta(object sender, ScatterManipulationDeltaEventArgs e)
        {
            var scatterViewItem = sender as ScatterViewItem;

            if (scatterViewItem != null)
            {
                evaluateState();

                

                switch ( CurrentState )
                {
                    case States.stateOne:
                    rotateMoveObject(sender, e);
                    break;

                    case States.stateTwo:
                    
                    break;
                    
                    case States.stateThree:

                    break;
                    
                    case States.stateFour:

                    break;
                    
                }
                
            }
        }

        private void evaluateState()
        {
           
        }

        private void rotateMoveObject(object sender, ScatterManipulationDeltaEventArgs e)
        {
            var scatterViewItem = sender as ScatterViewItem;
            bool directionMod = true;

            if (scatterViewItem != null)
            {
                // #FELO: Decide how draging will move object
                // #TODO: potential for improvement
                if (scatterViewItem.Orientation > maximumRotationDegree || scatterViewItem.Orientation < 90 + 1)
                {
                    if (e.HorizontalChange + e.VerticalChange > minimalRotationDegree)
                    {
                        directionMod = true;
                    }
                    else
                    {
                        directionMod = false;
                    }
                }
                else if (scatterViewItem.Orientation > 90 && scatterViewItem.Orientation < 180 + 1)
                {
                    if (e.HorizontalChange + -e.VerticalChange < minimalRotationDegree)
                    {
                        directionMod = true;
                    }
                    else
                    {
                        directionMod = false;
                    }
                }
                else if (scatterViewItem.Orientation > 180 && scatterViewItem.Orientation < 270 + 1)
                {
                    if (e.HorizontalChange + e.VerticalChange < minimalRotationDegree)
                    {
                        directionMod = true;
                    }
                    else
                    {
                        directionMod = false;
                    }
                }
                else if (scatterViewItem.Orientation > 270 && scatterViewItem.Orientation < maximumRotationDegree + 1)
                {
                    if (e.HorizontalChange + -e.VerticalChange > minimalRotationDegree)
                    {
                        directionMod = true;
                    }
                    else
                    {
                        directionMod = false;
                    }
                }

                // #FELO: Reset Original translation
                moveObjectReset(sender, new Point(-e.HorizontalChange, e.VerticalChange));

                if (directionMod)
                {
                    scatterViewItem.Orientation += rotationSpeed;
                }
                else
                {
                    scatterViewItem.Orientation -= rotationSpeed;
                }

                double orient = scatterViewItem.Orientation;
                double moveX = radiusMod * Math.Sin(orient / hypotenuseMod);
                double moveY = radiusMod * Math.Cos(orient / hypotenuseMod);
                Point movePointMod = new Point(moveX, moveY);
                moveObject(sender, movePointMod);

                // #FELO: Reset Orientation - for circle calibration
                if (scatterViewItem.Orientation > maximumRotationDegree)
                {
                    scatterViewItem.Orientation = minimalRotationDegree;
                }
                else if (scatterViewItem.Orientation < minimalRotationDegree)
                {
                    scatterViewItem.Orientation = maximumRotationDegree;
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
    }
}
