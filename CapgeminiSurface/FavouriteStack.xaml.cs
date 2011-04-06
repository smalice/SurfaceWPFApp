using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;
using CapgeminiSurface.Model;

namespace CapgeminiSurface
{

    public partial class FavouriteStack : UserControl
    {
        CollectionViewSource collection = new CollectionViewSource();

        public FavouriteStack()
        {
            InitializeComponent();
            
        }

        private void SurfaceStack_Initialized(object sender, EventArgs e)
        {
            ModelManager.Instance.PropertyChanged += new PropertyChangedEventHandler(Instance_PropertyChanged);
        }

        void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ModelManager.Instance.SelectedCustomer == null)
            {
                collection.Source = null;
            }
            else
            {
                collection.Source = ModelManager.Instance.SelectedCustomer.ContentItems;
            }
            collection.GroupDescriptions.Add(new PropertyGroupDescription("ContentType"));
            favouriteStackContent.ItemsSource = collection.View;
        }
    }
}
