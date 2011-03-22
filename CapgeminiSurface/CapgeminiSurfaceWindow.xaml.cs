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
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using System.Collections;

namespace CapgeminiSurface
{
    public partial class CapgeminiSurfaceWindow : SurfaceWindow
    {
        private Random randomStartAngle = new Random();

        public ArrayList MenuCardHolder = new ArrayList();

        public MenuLogo menuLogo;

        public CapgeminiSurfaceWindow()
        {
            Model.ModelManager.Instance.Load();
            InitializeComponent();
            AddActivationHandlers();

            foreach (var costumer in Model.ModelManager.Instance.Customers)
            {
                MenuCard card = new MenuCard();
                card.DataContext = costumer;
                surfaceMainGrid.Children.Add(card);
                Grid.SetColumn(card, 0);
                Grid.SetRow(card, 0);
                Panel.SetZIndex(card, 0);
                card.cardRotateTransform.Angle = randomStartAngle.Next(0, 360);
                MenuCardHolder.Add(card);
            }

            //Logo.rotateCards += new (menuLogo_ContactDown);
             
        }

        void menuLogo_ContactDown(object sender, ContactEventArgs e)
        {
            foreach (MenuCard card in MenuCardHolder)
            {

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