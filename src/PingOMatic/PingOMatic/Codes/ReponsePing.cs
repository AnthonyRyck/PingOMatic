
namespace PingOMatic.Codes
{
    public class ReponsePing
    {
        /// <summary>
        /// Indique si la machine est pingable.
        /// </summary>
        public bool IsPingable { get; set; }

        /// <summary>
        /// Temps en millisecondes
        /// </summary>
        public long TimePing { get; set; }
    }
}
