using System;
using System.Windows;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;

namespace CapgeminiSurface
{

    public partial class CustomerFilter : SurfaceUserControl
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

		private void EnergyButton_Checked(object sender, RoutedEventArgs e)
		{
			EnergyFilterChecked(this, e);
		}

		private void EnergyButton_Unchecked(object sender, RoutedEventArgs e)
		{
			EnergyFilterUnchecked(this, e);
		}

		private void CapgeminiButton_Checked(object sender, RoutedEventArgs e)
		{
			CapgeminiFilterChecked(this, e);
		}

		private void CapgeminiButton_Unchecked(object sender, RoutedEventArgs e)
		{
			CapgeminiFilterUnchecked(this, e);
		}

		private void OtherButton_Checked(object sender, RoutedEventArgs e)
		{
			OtherFilterChecked(this, e);
		}

		private void OtherButton_Unchecked(object sender, RoutedEventArgs e)
		{
			OtherFilterUnchecked(this, e);
		}

    	private void ShowButton_Checked(object sender, RoutedEventArgs e)
    	{
			VisualStateManager.GoToState(this, "Show", true);
    	}

    	private void ShowButton_Unchecked(object sender, RoutedEventArgs e)
    	{
			VisualStateManager.GoToState(this, "Hide", true);
    	}
    }
}
