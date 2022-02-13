using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PingOMatic.Views
{
	/// <summary>
	/// Interaction logic for ConfigMenu.xaml
	/// </summary>
	public partial class ConfigMenu : Window
	{
		private Func<string, string, Task> Save;


		public ConfigMenu(Func<string, string, Task> save)
		{
			InitializeComponent();
			Save = save;
		}

		private async void SaveConfig(object sender, RoutedEventArgs e)
		{
			await Save(HeaderName.Text, AppPath.Text);
			HeaderName.Text = string.Empty;
			AppPath.Text = string.Empty;
		}
	}
}
