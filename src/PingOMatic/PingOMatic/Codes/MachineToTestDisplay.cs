using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PingOMatic.Codes
{
	public class MachineToTestDisplay : MachineToTest, INotifyPropertyChanged
	{
		/// <summary>
		/// Status de la machine.
		/// </summary>
		public Status StatusMachine
		{
			get { return _statusMachine; }
			set
			{
				_statusMachine = value;
				OnNotifyPropertyChanged();
			}
		}
		private Status _statusMachine;

		/// <summary>
		/// Temps de réponse du ping.
		/// </summary>
        public long Temps
        {
            get { return _temps; }
            set
            {
                _temps = value;
                OnNotifyPropertyChanged();
            }
        }
        private long _temps;

		/// <summary>
		/// IP de la machine
		/// </summary>
		public string IpAdresse
		{
			get { return _ipAdresse; }
			set
			{
				_ipAdresse = value;
				OnNotifyPropertyChanged();
			}
		}
		private string _ipAdresse;

		public StatusDns DnsStatus
		{
			get { return _dnsStatus; }
			set
			{
				_dnsStatus = value;
				OnNotifyPropertyChanged();
			}
		}
		private StatusDns _dnsStatus;





		public MachineToTestDisplay(string nom, string description)
		{
			base.NomMachine = nom;
			base.Description = description;
			StatusMachine = Status.NotTested;
			DnsStatus = StatusDns.NotTested;
			IpAdresse = "Pas encore testé";
		}

		#region Implement INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;


		protected void OnNotifyPropertyChanged([CallerMemberName] string memberName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(memberName));
			}
		}

		#endregion
	}

	public enum Status
	{
		NotTested,
		InTesting,
		Connected,
		NotConnected
	}

	public enum StatusDns
	{
		NotTested,
		NoDns,
		GoodDns,
		ErrorDns
	}
}
