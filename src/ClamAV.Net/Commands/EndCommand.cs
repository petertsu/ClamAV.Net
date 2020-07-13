using ClamAV.Net.ClamdProtocol;
using ClamAV.Net.Commands.Base;

namespace ClamAV.Net.Commands
{
    internal class EndCommand : BaseCommand
    {
        public EndCommand() : base(Consts.IDSESSION_COMMAND)
        {
        }
    }
}