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
	/// Interaction logic for AjoutTest.xaml
	/// </summary>
	public partial class AjoutTest : Window
	{
		public AjoutTest(Func<string, string, Task> addMachine, Func<string, Task> addList, Func<string, Task> addClipboard)
		{
			InitializeComponent();
			AddClipboard = addClipboard;
			AddList = addList;
			AddMachine = addMachine;
		}


		private Func<string, string, Task> AddMachine;
		private Func<string, Task> AddList;
		private Func<string, Task> AddClipboard;

		private void UcName_TextChanged(object sender, TextChangedEventArgs e)
		{
			var textBox = sender as TextBox;
			textBox.Text = textBox.Text.Replace(" ", string.Empty);
		}

		private async void AddToList(object sender, RoutedEventArgs e)
		{
			await AddMachine(UcName.Text, DescriptionTextBox.Text);

			UcName.Text = string.Empty;
			DescriptionTextBox.Text = string.Empty;
		}

		private async void Button_AddListClick(object sender, RoutedEventArgs e)
		{
			await AddList(DescriptionMultiTextBox.Text);
			DescriptionMultiTextBox.Clear();

		}

		private async void Button_AddClipBoard_Click(object sender, RoutedEventArgs e)
		{
			await AddClipboard(DescriptionClipboardTextBox.Text);
			DescriptionClipboardTextBox.Clear();
		}

		private void CloseOnClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
