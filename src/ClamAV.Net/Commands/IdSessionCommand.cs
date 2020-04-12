//using System.Threading;
//using System.Threading.Tasks;
//using ClamAV.Net.Commands.Base;
//using ClamAV.Net.Socket;

//namespace ClamAV.Net.Commands
//{
//    internal class IdSessionCommand : BaseCommand, ICommand<string>
//    {
//        public IdSessionCommand(ISocketClient socketClient) : base("IDSESSION", socketClient)
//        {
//        }

//        public async Task<string> Execute(CancellationToken cancellationToken = default)
//        {
//            string response = await ExecuteAsync(cancellationToken).ConfigureAwait(false);

//            return response;
//        }

//        protected override bool WaitForResponse => false;
//    }
//}