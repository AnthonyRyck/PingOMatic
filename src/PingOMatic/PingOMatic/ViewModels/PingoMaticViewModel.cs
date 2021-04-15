using Newtonsoft.Json;
using PingOMatic.Codes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

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


		public PingoMaticViewModel()
		{
			ListeDesMachines = new ObservableCollection<MachineToTestDisplay>();
			PathSaveFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "saveMachines.dat");

			LoadSaveFile();

			IsEnabled = true;

			TimerPing = new Timer(Convert.ToDouble(TimeSpan.FromMinutes(5).TotalMilliseconds));
			//TimerPing = new Timer(Convert.ToDouble(TimeSpan.FromSeconds(15).TotalMilliseconds));

			TimerPing.Elapsed += TimerPing_Elapsed;
			TimerPing.AutoReset = true;
			TimerPing.Enabled = true;
		}

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
				throw;
			}
		}

		internal async Task DeleteMachine(MachineToTestDisplay selected)
		{
			ListeDesMachines.Remove(selected);
			await SaveToFile();
		}

		internal async Task Ping()
		{
			TimerPing.Stop();
			await PingAll();

			TimerPing.Start();
		}

		private void LoadSaveFile()
		{
			try
			{
				if (!File.Exists(PathSaveFile))
					return;

				string saveFile = File.ReadAllText(PathSaveFile);
				var tempList = JsonConvert.DeserializeObject<List<MachineToTest>>(saveFile);
				var listMachines = tempList.Select(x => new MachineToTestDisplay(x.NomMachine, x.Description)).ToList();

				ListeDesMachines = new ObservableCollection<MachineToTestDisplay>(listMachines);
			}
			catch (Exception ex)
			{

			}
		}

		private async Task SaveToFile()
		{
			try
			{
				List<MachineToTest> toSaveList = ListeDesMachines.ToList<MachineToTest>();

				string output = JsonConvert.SerializeObject(toSaveList);
				File.WriteAllText(PathSaveFile, output);
			}
			catch (Exception ex)
			{

			}
		}

		private async void TimerPing_Elapsed(object sender, ElapsedEventArgs e)
		{
			TimerPing.Stop();
			await PingAll();
			TimerPing.Start();
		}

		private async Task PingAll()
		{
			IsEnabled = false;

			foreach (MachineToTestDisplay machine in ListeDesMachines)
			{
				machine.StatusMachine = Status.InTesting;

				if(await Reseau.PingHostAsync(machine.NomMachine))
				{
					machine.StatusMachine = Status.Connected;
				}
				else
				{
					machine.StatusMachine = Status.NotConnected;
				}
			}

			IsEnabled = true;
		}

	}
}
