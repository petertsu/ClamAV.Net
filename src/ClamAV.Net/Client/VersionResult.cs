namespace ClamAV.Net.Client
{
    public class VersionResult
    {
        public string ProgramVersion { get; }
        public string VirusDbVersion { get; }

        internal VersionResult(string programVersion, string virusDbVersion)
        {
            ProgramVersion = programVersion;
            VirusDbVersion = virusDbVersion;
        }
    }
}