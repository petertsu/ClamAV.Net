using ClamAV.Net.Commands.Base;

namespace ClamAV.Net.Commands
{
    internal class PingCommand : BaseCommand
    { 
        public PingCommand() : base("PING")
        {
        }

    }
}