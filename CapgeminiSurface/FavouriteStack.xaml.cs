using System;
using System.Windows.Data;
using System.ComponentModel;
using CapgeminiSurface.Model;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace CapgeminiSurface
{
    public partial class FavouriteStack
    {
        #region Initialization
        
        public CollectionViewSource _collection = new CollectionViewSource();
        
        public FavouriteStack()
        {
            InitializeComponent();    
        }
        
        #endregion
        
        private void SurfaceStackInitialized(object sender, EventArgs e)
        {
            ModelManager.Instance.PropertyChanged += InstancePropertyChanged;
        }
        
        void InstancePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            List<ContentItem> copyList = null;
            if (ModelManager.Instance.SelectedCustomer != null)
                copyList = new List<ContentItem>(ModelManager.Instance.SelectedCustomer.ContentItems);
            _collection.Source = copyList;
            _collection.GroupDescriptions.Add(new PropertyGroupDescription("ContentType"));
            favouriteStackContent.ItemsSource = _collection.View;
        }

        public void RemoveCurrentInstanceProperty()
        {
            favouriteStackContent.ItemsSource = null;
        }

        public void RemoveInstancePropertyObject(object sender)
        {
            var item = sender as ContentItem;
            if (item != null)
            {
                var stackSource = _collection.Source as List<ContentItem>;
                if (stackSource != null)
                {
                    stackSource.Remove(item);
                    favouriteStackContent.ItemsSource = _collection.View;
                }
            }
        }

        public void AddInstancePropertyObject(object sender)
        {
            var item = sender as ContentItem;
            if (item != null)
            {

                var stackSource = _collection.Source as List<ContentItem>;
                if (stackSource != null)
                {
                    stackSource.Add(item);
                    _collection.Source = stackSource;
                    _collection.GroupDescriptions.Add(new PropertyGroupDescription("ContentType"));
                    favouriteStackContent.ItemsSource = null;
                    favouriteStackContent.ItemsSource = _collection.View;   
                }
            }
        }
    }
}
