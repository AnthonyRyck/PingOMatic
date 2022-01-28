using PingOMatic.Codes;
using PingOMatic.ViewModels;
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

		private void ClickToItem(object sender, SelectionChangedEventArgs e)
		{
			var machineSelected = e.AddedItems[0] as MachineToTestDisplay;
			ViewModel.CopyToClipBoard(machineSelected);
		}

		private async void Button_AddListClick(object sender, RoutedEventArgs e)
        {
			await ViewModel.AddList();
			DescriptionMultiTextBox.Clear();

		}
    }
}
