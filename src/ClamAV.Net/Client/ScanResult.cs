namespace ClamAV.Net.Client
{
    public class ScanResult
    {
        public bool Infected { get; }
        public string VirusName { get; }

        internal ScanResult(bool infected, string virusName = null)
        {
            Infected = infected;
            VirusName = virusName;
        }
    }
}