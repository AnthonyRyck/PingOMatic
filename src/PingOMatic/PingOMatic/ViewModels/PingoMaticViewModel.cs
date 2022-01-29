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
		/// Chemin d'accès au fichier de sauvegarde pour le fichier.
		/// </summary>
		private string PathSaveFile;

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

			LoadSaveFile();

			IsEnabled = true;

			TimerPing = new Timer(Convert.ToDouble(TimeSpan.FromMinutes(5).TotalMilliseconds));

			TimerPing.Elapsed += TimerPing_Elapsed;
			TimerPing.AutoReset = true;
			TimerPing.Enabled = true;

			optionPing = SEQUENTIEL;
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
			Clipboard.SetText(machineSelected.NomMachine);
			Notify.ShowNotification("Copié", machineSelected.NomMachine + " dans le presse-papiers !", System.Windows.Forms.ToolTipIcon.Info, 3);
		}

		#endregion

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

		#endregion

	}
}
