using System;
using System.Windows;

namespace CapgeminiSurface
{
    public partial class CustomerFilter
    {

    	public EventHandler EnergyFilterChecked;
		public EventHandler EnergyFilterUnchecked;
		public EventHandler CapgeminiFilterChecked;
		public EventHandler CapgeminiFilterUnchecked;
		public EventHandler OtherFilterChecked;
		public EventHandler OtherFilterUnchecked;
		
		public CustomerFilter()
        {
            InitializeComponent();
			VisualStateManager.GoToState(this, "Hide", true);
		}

		private void EnergyButtonChecked(object sender, RoutedEventArgs e)
		{
			EnergyFilterChecked(this, e);
		}

		private void EnergyButtonUnchecked(object sender, RoutedEventArgs e)
		{
			EnergyFilterUnchecked(this, e);
		}

		private void CapgeminiButtonChecked(object sender, RoutedEventArgs e)
		{
			CapgeminiFilterChecked(this, e);
		}

		private void CapgeminiButtonUnchecked(object sender, RoutedEventArgs e)
		{
			CapgeminiFilterUnchecked(this, e);
		}

		private void OtherButtonChecked(object sender, RoutedEventArgs e)
		{
			OtherFilterChecked(this, e);
		}

		private void OtherButtonUnchecked(object sender, RoutedEventArgs e)
		{
			OtherFilterUnchecked(this, e);
		}

    	private void ShowButtonChecked(object sender, RoutedEventArgs e)
    	{
			VisualStateManager.GoToState(this, "Show", true);
    	}

    	private void ShowButtonUnchecked(object sender, RoutedEventArgs e)
    	{
			VisualStateManager.GoToState(this, "Hide", true);
    	}
    }
}
