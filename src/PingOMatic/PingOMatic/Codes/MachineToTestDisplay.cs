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


		public MachineToTestDisplay(string nom, string description)
		{
			base.NomMachine = nom;
			base.Description = description;
			StatusMachine = Status.NotTested;
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
}
