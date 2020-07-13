using System.IO;
using System.Text;
using System.Threading.Tasks;
using ClamAV.Net.ClamdProtocol;
using ClamAV.Net.Commands;
using ClamAV.Net.Exceptions;
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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("DATA")]
        public async Task ProcessRawResponseAsync_Invalid_Raw_Data_Should_Throw_exception(string rawData)
        {
            PingCommand pingCommand = new PingCommand();

            byte[] rawBytes = rawData == null ? null : Encoding.UTF8.GetBytes(rawData);

            await Assert
                .ThrowsAsync<ClamAvException>(async () =>
                    await pingCommand.ProcessRawResponseAsync(rawBytes).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task ProcessRawResponseAsync_Valid_Raw_Data_Should_Return_PONG()
        {
            PingCommand pingCommand = new PingCommand();

            byte[] rawBytes = Encoding.UTF8.GetBytes("PONG");

            string actual = await pingCommand.ProcessRawResponseAsync(rawBytes).ConfigureAwait(false);

            actual.Should().Be("PONG");
        }
    }
}