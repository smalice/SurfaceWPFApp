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

        // TODO: remove const; construct
        private const int menuCardAmount=5;

        public CapgeminiSurfaceWindow()
        {
            InitializeComponent();
<<<<<<< HEAD
            foreach (var card in ModelManager.Instance.Customers)
            {
                MenuCard menuCard = new MenuCard();
                menuCard.Name = card.Name;
                ContentGrid.Children.Add(menuCard);
                Grid.SetColumn(menuCard, 1);
                Grid.SetRow(menuCard, 1);
                Panel.SetZIndex(menuCard, 0);
            }
            // Add handlers for Application activation events
=======
>>>>>>> 5e67bfe948c0386c8cce0a426de719c22da7faf4
            AddActivationHandlers();
            
            // TODO: remove
            //menuLogo = new MenuLogo(this);

            // TODO: remove
            MenuCardHolder.Add(CardTwo);
            MenuCardHolder.Add(CardTwo);
            MenuCardHolder.Add(CardThree);
            MenuCardHolder.Add(CardFour);
            MenuCardHolder.Add(CardFive);

            for (int i = 1; i < menuCardAmount; i++)
            {
                MenuCardHolder.Add(new MenuCard());
            }

            foreach (MenuCard card in MenuCardHolder)
            {
                card.cardRotateTransform.Angle = randomStartAngle.Next(0, 360);
            }

            Logo.ContactDown += new ContactEventHandler(menuLogo_ContactDown);
        }

        void menuLogo_ContactDown(object sender, ContactEventArgs e)
        {
            rotateAllCards(1);
            //throw new NotImplementedException();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            RemoveActivationHandlers();
        }

        public void rotateAllCards(int rotation)
        {
            foreach (MenuCard card in MenuCardHolder)
            {
                card.cardRotateTransform.Angle += rotation;
            }
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