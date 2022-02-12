using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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

		/// <summary>
		/// Retourne la liste d'IP pour cette adresse.
		/// Résout un nom d'hôte ou une adresse IP
		/// </summary>
		/// <param name="nameOrAddress"></param>
		/// <returns></returns>
		public static async Task<IEnumerable<string>> DnsTestAsync(string nameOrAddress)
		{
			List<string> adressesIpV4 = new List<string>();

			IPHostEntry result = await Dns.GetHostEntryAsync(nameOrAddress);
			if (result == null)
				return new string[] { "Aucune IP" };

			foreach (var item in result.AddressList)
			{
				if (item.AddressFamily.ToString() == ProtocolFamily.InterNetwork.ToString())
					adressesIpV4.Add(item.ToString());
			}

			return adressesIpV4;
		}

	}
}