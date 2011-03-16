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

namespace CapgeminiSurface
{
	/// <summary>
	/// Interaction logic for ProjectItem.xaml
	/// </summary>
	public partial class ProjectItem : StackItem
	{
		public ProjectItem()
		{
			InitializeComponent();
		}

		private void onInitialized(object sender, EventArgs e)
		{
			projectName.Text = sender.GetType().ToString();
			projectName.Foreground = Brushes.Black;
			projectDescription.Text = stringOptimizer("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus iaculis luctus dui quis tincidunt. Praesent nunc enim, pulvinar sit amet suscipit et, euismod sit amet nulla. Morbi id commodo lectus. Nulla vel dignissim felis. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Quisque eu mi quis ligula vulputate dignissim sit amet non diam. Etiam imperdiet ligula at enim dapibus ultricies. Pellentesque vitae purus nulla. Ut pulvinar urna nec nisi dignissim ut tempus dui tincidunt. Maecenas sit amet diam lobortis sapien pharetra molestie. Duis dolor metus, egestas ac scelerisque a, porttitor sit amet mi. Sed metus augue, porta eget pulvinar vel, fermentum in mauris. Sed laoreet iaculis orci, quis mattis diam congue molestie. Cras in massa nisi. Donec sodales tincidunt adipiscing. Aenean in purus eget mi volutpat consequat. Fusce felis sapien, feugiat a vulputate in, luctus et massa. Pellentesque nibh ipsum, molestie quis tempor et, aliquam vulputate leo. Nulla sodales consectetur tortor ut commodo. Nullam venenatis felis eget felis aliquet a molestie sem posuere.");
			projectDescription.Foreground = Brushes.Black;
			projectGrid.Background = Brushes.Aqua;
		}

		private string stringOptimizer(string text)
		{
			string tmp = text;
			int size = 50;
			if (text.Length > size)
			{
				if (!text.Contains("\n"))
				{
					tmp = text.Substring(0, size) + "\n";
					for (int position = 51; position < text.Length; position += (size + 1))
					{
						if (text.Substring(position).Length <= size)
						{
							tmp += text.Substring(position);
						}
						else
						{
							tmp += text.Substring(position, size) + "\n";
						}
					}
				}
			}
			return tmp;
		}
	}
}
