using Microsoft.Win32;
using Newtonsoft.Json;
using PingOMatic.Codes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace PingOMatic.ViewModels
{
	public class PingoMaticViewModel : BaseViewModel
	{
		/// <summary>
		/// Liste des machines à Ping
		/// </summary>
		public ObservableCollection<MachineToTestDisplay> ListeDesMachines
		{
			get { return _listeDesMachines; }
			set
			{
				_listeDesMachines = value;
				OnNotifyPropertyChanged();
			}
		}
		private ObservableCollection<MachineToTestDisplay> _listeDesMachines;

		/// <summary>
		/// Pour verrouiller les controles quand Ping
		/// </summary>
		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				_isEnabled = value;
				OnNotifyPropertyChanged();
			}
		}
		private bool _isEnabled;

		/// <summary>
		/// Chemin d'accès au fichier de sauvegarde pour les machines/URL.
		/// </summary>
		private string PathSaveFile;

		/// <summary>
		/// Chemin d'accès au fichier de sauvegarde pour les menus.
		/// </summary>
		private string PathFileMenu;

		/// <summary>
		/// Timer pour faire les Ping
		/// </summary>
		private Timer TimerPing;


		private const string SEQUENTIEL = "sequentiel";
		private const string PARALELLE = "parallele";
		private string optionPing;

		private NotifyWindow Notify = new NotifyWindow();

		public PingoMaticViewModel()
		{
			ListeDesMachines = new ObservableCollection<MachineToTestDisplay>();
			PathSaveFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "saveMachines.dat");
			PathFileMenu = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "menus.dat");

			LoadSaveFile();

			IsEnabled = true;

			TimerPing = new Timer(Convert.ToDouble(TimeSpan.FromMinutes(5).TotalMilliseconds));

			TimerPing.Elapsed += TimerPing_Elapsed;
			TimerPing.AutoReset = true;
			TimerPing.Enabled = true;

			optionPing = SEQUENTIEL;

			InitContextMenus();
		}

		#region Internal methods

		internal async Task AddMachine(string machine, string description)
		{
			try
			{
				MachineToTestDisplay nouvelleMachine = new MachineToTestDisplay(machine, description);
				ListeDesMachines.Add(nouvelleMachine);

				await SaveToFile();
			}
			catch (Exception ex)
			{
				LogErreur(ex);
				Notify.ShowNotification("Erreur", "Erreur ajouté au fichier log", System.Windows.Forms.ToolTipIcon.Error);
			}
		}

		internal async Task DeleteMachine(MachineToTestDisplay selected)
		{
			try
			{
				ListeDesMachines.Remove(selected);
				await SaveToFile();
			}
			catch (Exception ex)
			{
				LogErreur(ex);
				Notify.ShowNotification("Erreur", "Erreur ajouté au fichier log", System.Windows.Forms.ToolTipIcon.Error);
			}
		}

        internal async Task AddClipboard(string description)
        {
			if (string.IsNullOrEmpty(description))
				description = "Ajout Presse Papier";

			string text = Clipboard.GetText();
			foreach (string url in Split(text))
			{
				await AddMachine(url, description);
			}
		}

        internal void SetConfigPing(string tag)
        {
			if(!string.IsNullOrEmpty(tag))
				optionPing = tag;
		}

        internal async Task AddList(string description)
        {
            try
            {
				Stream[] files = await Task.Factory.StartNew(() =>
				{
					OpenFileDialog fileDialog = new OpenFileDialog();
					fileDialog.Filter = "fichiers csv (*.csv)|*.csv";
					fileDialog.Multiselect = true;
					fileDialog.Title = "Liste de machine/URL";
					fileDialog.ShowDialog();

					return fileDialog.OpenFiles();
				});

				if (string.IsNullOrEmpty(description))
					description = "Multi ajout";

				foreach (var file in files)
                {
					foreach(string url in GetUrl(file))
                    {
						await AddMachine(url, description);
                    }
				}
            }
            catch (Exception ex)
            {
				LogErreur(ex);
				Notify.ShowNotification("Erreur", "Erreur ajouté au fichier log", System.Windows.Forms.ToolTipIcon.Error);
			}
        }

        internal async Task Ping()
		{
			try
			{
                TimerPing.Stop();
                
				switch (optionPing)
                {
					case SEQUENTIEL:
						await PingAllSequentiel();
						break;

					case PARALELLE:
						await PingAllParallel();
						break;
                    
					default:
						await PingAllSequentiel();
						break;
                }

				TimerPing.Start();
			}
			catch (Exception ex)
			{
				LogErreur(ex);
				Notify.ShowNotification("Erreur", "Erreur ajouté au fichier log", System.Windows.Forms.ToolTipIcon.Error);
			}
		}

		internal async Task Ping(MachineToTestDisplay machine)
		{
			try
			{
				await PingMachine(machine);
			}
			catch (Exception ex)
			{
				LogErreur(ex);
				Notify.ShowNotification("Erreur", "Erreur ajouté au fichier log", System.Windows.Forms.ToolTipIcon.Error);
			}
		}

		internal void CopyToClipBoard(MachineToTestDisplay machineSelected)
		{
			if (machineSelected == null)
				return;

			Clipboard.SetText(machineSelected.NomMachine);
			Notify.ShowNotification("Copié", machineSelected.NomMachine + " dans le presse-papiers !", System.Windows.Forms.ToolTipIcon.Info, 3);
		}




		#endregion

		#region Private methods

		private void LoadSaveFile()
		{
			if (!File.Exists(PathSaveFile))
				return;

			string saveFile = File.ReadAllText(PathSaveFile);
			var tempList = JsonConvert.DeserializeObject<List<MachineToTest>>(saveFile);
			var listMachines = tempList.Select(x => new MachineToTestDisplay(x.NomMachine, x.Description)).ToList();

			ListeDesMachines = new ObservableCollection<MachineToTestDisplay>(listMachines);
		}

		private async Task SaveToFile()
		{
			List<MachineToTest> toSaveList = ListeDesMachines.ToList<MachineToTest>();

			string output = JsonConvert.SerializeObject(toSaveList);
			File.WriteAllText(PathSaveFile, output);
		}

		private async void TimerPing_Elapsed(object sender, ElapsedEventArgs e)
		{
			TimerPing.Stop();
			await Ping();
			TimerPing.Start();
		}

		private async Task PingAllSequentiel()
		{
			IsEnabled = false;

            foreach (MachineToTestDisplay machine in ListeDesMachines)
            {
                await PingMachine(machine);
            }

            IsEnabled = true;
		}

		private async Task PingAllParallel()
        {
			IsEnabled = false;
			Parallel.ForEach(ListeDesMachines, async machine => await PingMachine(machine));
			IsEnabled = true;
		}

		/// <summary>
		/// Méthode qui va ping la machine.
		/// </summary>
		/// <param name="machine"></param>
		/// <returns></returns>
		private async Task PingMachine(MachineToTestDisplay machine)
		{
			var oldValue = machine.StatusMachine;

			machine.StatusMachine = Status.InTesting;

			ReponsePing resultPing = await Reseau.PingHostAsync(machine.NomMachine);

			if (resultPing.IsPingable)
			{
				machine.StatusMachine = Status.Connected;
				
				if (oldValue == Status.NotConnected)
				{
					Notify.ShowNotification("PING", machine.NomMachine + " en ligne", System.Windows.Forms.ToolTipIcon.Info);
				}

				// Résoudre le DNS.
				var adresses = await Reseau.DnsTestAsync(machine.NomMachine);
				int nbreIp = adresses.Count();

				switch (nbreIp)
				{
					// Si 0, pas de DNS configuré.
					case 0:
						machine.IpAdresse = "Aucun adresse IP trouvé";
						machine.DnsStatus = StatusDns.NoDns;
						break;

					case 1:
						machine.IpAdresse = adresses.First();
						machine.DnsStatus = StatusDns.GoodDns;
						break;

					// DNS qui contient plusieurs IP pour la même adresse.
					default:
						machine.IpAdresse = string.Join(" - ", adresses);
						machine.DnsStatus = StatusDns.ErrorDns;
						break;
				}
			}
			else
			{
				if (oldValue == Status.Connected)
				{
					Notify.ShowNotification("PING", machine.NomMachine + " déconnecté", System.Windows.Forms.ToolTipIcon.Error);
				}

				machine.StatusMachine = Status.NotConnected;
			}

			machine.Temps = resultPing.TimePing;
		}

		private IEnumerable<string> GetUrl(Stream streamFile)
		{
			StreamReader reader = new StreamReader(streamFile);
			string text = reader.ReadToEnd();
			return Split(text);
		}

		private IEnumerable<string> Split(string text)
		{
			return text.Split(new string[] { ";", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
		}

		#endregion


		#region Add custom Menu

		private void InitContextMenus()
		{
			// Ajout par défaut de ce menu.
			CustomMenuItems = new List<MenuItem>();
			MenuItem menuPing = new MenuItem()
			{
				Header = "Ping"
			};
			menuPing.Click += OnPingThisMachine;
			CustomMenuItems.Add(menuPing);

			LoadMenuFile();
			foreach (var item in _menuCustom)
			{
				MenuItem menu = CreateMenuItem(item);
				CustomMenuItems.Add(menu);
			}
		}

		private List<CustomMenu> _menuCustom = new List<CustomMenu>();
		public List<MenuItem> CustomMenuItems { get; set; }

		private async void OnPingThisMachine(object sender, RoutedEventArgs e)
		{
			var item = sender as MenuItem;
			var machine = item.DataContext as MachineToTestDisplay;

			await Ping(machine);
		}

		private async void OnExecuteCommand(object sender, RoutedEventArgs e)
		{
			var item = sender as MenuItem;
			var machine = item.DataContext as MachineToTestDisplay;
			var custom = item.Tag as CustomMenu;

			string param = GetParam(machine);
			string newPath = custom.PathApp.Replace("*****", param);

			if (!string.IsNullOrEmpty(custom.PathApp))
				System.Diagnostics.Process.Start(newPath);
		}

		private string GetParam(MachineToTestDisplay machine)
		{
			var parametre = machine.Description.Split('[', ']');
			if (parametre.Length > 1)
				return parametre[1];
			else return string.Empty;
		}

		internal async Task AddMenu(string header, string path)
		{
			try
			{
				CustomMenu nouveauMenu = new CustomMenu(header, path);
				_menuCustom.Add(nouveauMenu);

				MenuItem menuItem = CreateMenuItem(nouveauMenu);
				CustomMenuItems.Add(menuItem);
				
				await SaveMenuToFile();
			}
			catch (Exception ex)
			{
				LogErreur(ex);
				Notify.ShowNotification("Erreur", "Erreur ajouté au fichier log", System.Windows.Forms.ToolTipIcon.Error);
			}
		}

		private MenuItem CreateMenuItem(CustomMenu nouveauMenu)
		{
			MenuItem menuItem = new MenuItem()
			{
				Header = nouveauMenu.NameMenu
			};
			menuItem.Click += OnExecuteCommand;
			menuItem.Tag = nouveauMenu;

			return menuItem;
		}


		private async Task SaveMenuToFile()
		{
			string output = JsonConvert.SerializeObject(_menuCustom);
			File.WriteAllText(PathFileMenu, output);
		}

		private void LoadMenuFile()
		{
			if (!File.Exists(PathFileMenu))
				return;

			string menus = File.ReadAllText(PathFileMenu);
			_menuCustom = JsonConvert.DeserializeObject<List<CustomMenu>>(menus);
		}


		#endregion

	}
}
