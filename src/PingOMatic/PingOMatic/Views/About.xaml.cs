using System.Windows;
using System.Windows.Documents;

namespace PingOMatic.Views
{
	/// <summary>
	/// Interaction logic for About.xaml
	/// </summary>
	public partial class About : Window
	{
		public About()
		{
			InitializeComponent();
		}

		private void OnClickOnMe(object sender, RoutedEventArgs e)
		{
			OpenBrowser((Hyperlink)sender);
		}

		private void OnClickGithub(object sender, RoutedEventArgs e)
		{
			OpenBrowser((Hyperlink)sender);
		}

		private void OpenBrowser(Hyperlink hyperlink)
		{
			string url = hyperlink.NavigateUri.ToString();
			System.Diagnostics.Process.Start(url);
		}
	}
}
