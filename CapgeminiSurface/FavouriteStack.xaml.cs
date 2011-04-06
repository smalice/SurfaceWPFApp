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
            collection.Source = ModelManager.Instance.SelectedCustomer.ContentItems;
            collection.GroupDescriptions.Add(new PropertyGroupDescription("ContentType"));
            favouriteStackContent.ItemsSource = collection.View;
            //favouriteStackContent.DragLeave += new System.Windows.DragEventHandler(favouriteStackContent_DragLeave);
        }

        //void favouriteStackContent_DragLeave(object sender, System.Windows.DragEventArgs e)
        //{
        //    var b = true;
        //}

        //private void ProjectItem_Drop(object sender, System.Windows.DragEventArgs e)
        //{
        //    var b = true;
        //}

        //private void ProjectItem_DragOver(object sender, System.Windows.DragEventArgs e)
        //{
        //    var b = true;
        //}

        //private void ProjectItem_ContactDown(object sender, Microsoft.Surface.Presentation.ContactEventArgs e)
        //{
        //    var b = true;
       // }
    }
}
