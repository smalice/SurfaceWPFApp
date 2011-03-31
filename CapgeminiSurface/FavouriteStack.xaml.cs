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
        public FavouriteStack()
        {
            InitializeComponent();
            
        }

        private void SurfaceStack_Initialized(object sender, EventArgs e)
        {
            CollectionViewSource collection = new CollectionViewSource();
            collection.Source = ModelManager.Instance.AllCustomers;
            collection.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            favouriteStackContent.ItemsSource = collection.View;
        }
    }
}
