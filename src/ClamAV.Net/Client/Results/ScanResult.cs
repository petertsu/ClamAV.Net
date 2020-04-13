namespace ClamAV.Net.Client.Results
{
    /// <summary>
    /// ClamAV scan result
    /// </summary>
    public class ScanResult
    {
        /// <summary>
        /// Indicates the infection
        /// </summary>
        public bool Infected { get; }

        /// <summary>
        /// Found virus name
        /// Null once not infected
        /// </summary>
        public string VirusName { get; }

        internal ScanResult(bool infected, string virusName = null)
        {
            Infected = infected;
            VirusName = virusName;
        }
    }
}