namespace ClamAV.Net.CommandResponses
{
    internal class VersionCommandResponse
    {
        public string ProgramVersion { get; }
        public string VirusDbVersion { get; }

        public VersionCommandResponse(string rawResponse)
        {
           // string[] data = rawResponse.Split(Path.AltDirectorySeparatorChar);
            ProgramVersion = rawResponse;
          //  VirusDbVersion = rawResponse;
        }
    }
}