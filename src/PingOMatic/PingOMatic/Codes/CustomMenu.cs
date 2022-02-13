using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace PingOMatic.Codes
{
	internal class CustomMenu
	{
		public string NameMenu { get; private set; }

		public string PathApp { get; private set; }

		public CustomMenu(string nameMenu, string pathApp)
		{
			NameMenu = nameMenu;
			PathApp = pathApp;
		}
	}
}
