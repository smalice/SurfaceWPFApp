using System;
using System.Windows.Data;
using System.ComponentModel;
using CapgeminiSurface.Model;

namespace CapgeminiSurface
{

    public partial class FavouriteStack
    {
        #region Initialization
        
        readonly CollectionViewSource _collection = new CollectionViewSource();

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
            _collection.Source = ModelManager.Instance.SelectedCustomer == null ? null : ModelManager.Instance.SelectedCustomer.ContentItems;
            _collection.GroupDescriptions.Add(new PropertyGroupDescription("ContentType"));
            favouriteStackContent.ItemsSource = _collection.View;
        }
    }
}
