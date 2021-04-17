using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PingOMatic.ViewModels
{
	public class BaseViewModel : INotifyPropertyChanged
	{
		/// <summary>
		/// Chemin pour le fichier de log erreur.
		/// </summary>
		private string PathLogFile;


		public BaseViewModel()
		{
			PathLogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logErro.txt");
		}

		protected void LogErreur(Exception exception)
		{
			string messageError = DateTime.Now.ToString("dd/MM/yyyy-HH-mm")
									+ " - "
									+ exception.Message;

			File.AppendAllText(PathLogFile, messageError);
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
}
