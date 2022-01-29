using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace PingOMatic.Codes
{
	public static class Reseau
	{
		/// <summary>
		/// Permet de faire un ping sur la machine, pour savoir si elle est connecté.
		/// </summary>
		/// <param name="nameOrAddress">Nom ou adresse IP de la machine</param>
		/// <returns></returns>
		public static async Task<ReponsePing> PingHostAsync(string nameOrAddress)
		{
			return await Task.Factory.StartNew(() =>
			{
				Ping pinger = new Ping();

				ReponsePing reponse = new ReponsePing();
				reponse.IsPingable = false;

				try
				{
					PingReply reply = pinger.Send(nameOrAddress);
					reponse.IsPingable = reply.Status == IPStatus.Success;
					reponse.TimePing = reply.RoundtripTime;
				}
				catch (Exception)
				{
					reponse.IsPingable = false;
				}
				finally
				{
					if (pinger != null)
					{
						pinger.Dispose();
					}
				}

				return reponse;
			});
		}

	}
}