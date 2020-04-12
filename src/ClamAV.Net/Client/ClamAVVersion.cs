using ClamAV.Net.CommandResponses;

namespace ClamAV.Net.Client
{
    public class VersionResult
    {
        public string ProgramVersion { get; }
        public string VirusDBVersion { get; }

        internal VersionResult(VersionCommandResponse commandResponse)
        {
            ProgramVersion = commandResponse.ProgramVersion;
            VirusDBVersion = commandResponse.VirusDbVersion;
        }
    }
}