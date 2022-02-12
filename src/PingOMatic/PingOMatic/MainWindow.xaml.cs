using PingOMatic.Codes;
using PingOMatic.ViewModels;
using PingOMatic.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PingOMatic
{
	/// <summary>
	/// Logique d'interaction pour MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private PingoMaticViewModel ViewModel;


		public MainWindow()
		{
			InitializeComponent();

			ViewModel = GetViewModel();
		}




		private async void AddToList(object sender, RoutedEventArgs e)
		{
			//var viewModel = GetViewModel();
			// Ajouter dans le fichier de sauvegarde
			await ViewModel.AddMachine(UcName.Text, DescriptionTextBox.Text);

			UcName.Text = string.Empty;
			DescriptionTextBox.Text = string.Empty;
		}


		private PingoMaticViewModel GetViewModel()
		{
			return this.DataContext as PingoMaticViewModel;
		}

		private async void ForcerPingOnClick(object sender, RoutedEventArgs e)
		{
			await ViewModel.Ping();
		}

		private async void DeleteOnClick(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;

			var selected = btn.Tag as MachineToTestDisplay;
			await ViewModel.DeleteMachine(selected);
		}

		private void UcName_TextChanged(object sender, TextChangedEventArgs e)
		{
			var textBox = sender as TextBox;
			textBox.Text = textBox.Text.Replace(" ", string.Empty);
		}

		private async void OnPingThisMachine(object sender, RoutedEventArgs e)
		{
			var item = sender as MenuItem;
			var machine = item.DataContext as MachineToTestDisplay;

			await ViewModel.Ping(machine);
		}

		private void OnClickToMachine(object sender, MouseButtonEventArgs e)
		{
			var machineSelected = ((ListView) sender).SelectedItem as MachineToTestDisplay;
			ViewModel.CopyToClipBoard(machineSelected);
		}

		private async void Button_AddListClick(object sender, RoutedEventArgs e)
        {
			await ViewModel.AddList(DescriptionMultiTextBox.Text);
			DescriptionMultiTextBox.Clear();

		}

        private async void Button_AddClipBoard_Click(object sender, RoutedEventArgs e)
        {
			await ViewModel.AddClipboard(DescriptionClipboardTextBox.Text);
			DescriptionClipboardTextBox.Clear();
		}

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			if (sender == null)
				return;

			var comboBox = sender as ComboBox;
			var item = comboBox.SelectedItem as ComboBoxItem;

			if (item != null)
            {
				string tag = item.Tag?.ToString();
				ViewModel?.SetConfigPing(tag);
            }
			
		}

		#region Events sur ContextMenu


		private void OnThisMachineForcerPingOnClick(object sender, RoutedEventArgs e)
		{
			
		}


		#endregion

		private void AboutOnClick(object sender, RoutedEventArgs e)
		{
			// Display About.
			About aboutWindows = new About();
			aboutWindows.Show();
		}

	}
}
