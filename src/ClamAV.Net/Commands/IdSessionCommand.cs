using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.ClamdProtocol;
using ClamAV.Net.Commands.Base;
using ClamAV.Net.Exceptions;
using ClamAV.Net.Socket;

namespace ClamAV.Net.Commands
{
    internal class IdSessionCommand : BaseCommand
    {

        public IdSessionCommand() : base(Consts.IDSESSION_COMMAND)
        {
        }
    }
}