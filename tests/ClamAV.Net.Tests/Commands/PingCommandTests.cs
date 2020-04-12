using System.IO;
using System.Text;
using System.Threading.Tasks;
using ClamAV.Net.ClamdProtocol;
using ClamAV.Net.Commands;
using FluentAssertions;
using Xunit;

namespace ClamAV.Net.Tests.Commands
{
    public class PingCommandTests
    {
        [Fact]
        public void Ctor_Set_Valid_Name()
        {
            PingCommand pingCommand = new PingCommand();
            pingCommand.Name.Should().Be("PING");
        }

        [Fact]
        public async Task WriteCommandAsync_Should_Write_CommandName()
        {
            PingCommand pingCommand = new PingCommand();
            await using MemoryStream memoryStream = new MemoryStream();

            await pingCommand.WriteCommandAsync(memoryStream).ConfigureAwait(false);

            byte[] commandData = memoryStream.ToArray();

            string actual = Encoding.UTF8.GetString(commandData);

            actual.Should()
                .Be($"{Consts.COMMAND_PREFIX_CHARACTER}{pingCommand.Name}{(char)Consts.TERMINATION_BYTE}");
        }
    }
}