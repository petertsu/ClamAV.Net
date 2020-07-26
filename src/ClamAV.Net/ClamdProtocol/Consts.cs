namespace ClamAV.Net.ClamdProtocol
{
    internal static class Consts
    {
        public const char COMMAND_PREFIX_CHARACTER = 'z';
        public const byte TERMINATION_BYTE = 0;
        public const string VERSION_COMMAND = "VERSION";
        public const string PING_COMMAND = "PING";
        public const string INSTREAM_COMMAND = "INSTREAM";
        public const string IDSESSION_COMMAND = "IDSESSION";
        public const string END_COMMAND = "END";
        public const string SCAN = "SCAN";
    }
}