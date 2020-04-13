namespace ClamAV.Net.Client.Results
{
    /// <summary>
    /// Result of VERSION command execution 
    /// </summary>
    public class VersionResult
    {
        /// <summary>
        /// ClamAV server version
        /// </summary>
        public string ProgramVersion { get; }

        /// <summary>
        /// ClamAV server virus database version
        /// </summary>
        public string VirusDbVersion { get; }

        internal VersionResult(string programVersion, string virusDbVersion)
        {
            ProgramVersion = programVersion;
            VirusDbVersion = virusDbVersion;
        }
    }
}