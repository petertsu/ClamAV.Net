using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ClamAV.Net.ClamdProtocol;
using ClamAV.Net.Client;
using ClamAV.Net.Client.Results;
using ClamAV.Net.Commands;
using ClamAV.Net.Exceptions;
using FluentAssertions;
using Xunit;

namespace ClamAV.Net.Tests.Commands
{
    public class VersionCommandTests
    {
        [Fact]
        public void Ctor_Set_Valid_Name()
        {
            VersionCommand versionCommand = new VersionCommand();
            versionCommand.Name.Should().Be("VERSION");
        }

        [Fact]
        public async Task WriteCommandAsync_Should_Write_CommandName()
        {
            VersionCommand versionCommand = new VersionCommand();
            await using MemoryStream memoryStream = new MemoryStream();

            await versionCommand.WriteCommandAsync(memoryStream).ConfigureAwait(false);

            byte[] commandData = memoryStream.ToArray();

            string actual = Encoding.UTF8.GetString(commandData);

            actual.Should()
                .Be($"{Consts.COMMAND_PREFIX_CHARACTER}{versionCommand.Name}{(char)Consts.TERMINATION_BYTE}");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ERROR")]
        [InlineData("ClamAV 1.17.99/")]

        public async Task ProcessRawResponseAsync_Invalid_Raw_Data_Should_Throw_exception(string rawData)
        {
            VersionCommand versionCommand = new VersionCommand();

            byte[] rawBytes = rawData == null ? null : Encoding.UTF8.GetBytes(rawData);

            await Assert.ThrowsAsync<ClamAvException>(async () => await versionCommand.ProcessRawResponseAsync(rawBytes).ConfigureAwait(false));
        }

        [Fact]
        public async Task ProcessRawResponseAsync_Valid_Raw_Data_Should_Return_PONG()
        {
            VersionCommand versionCommand = new VersionCommand();

            string expectedProgramVersion = "ClamAv 1.17.219";
            string expectedVirusDbVersion = (DateTime.Now.Ticks % 11177).ToString();


            byte[] rawBytes = Encoding.UTF8.GetBytes($"{expectedProgramVersion}/{expectedVirusDbVersion}/{DateTime.Now}");

            VersionResult actual = await versionCommand.ProcessRawResponseAsync(rawBytes).ConfigureAwait(false);

            actual.ProgramVersion.Should().Be(expectedProgramVersion);
            actual.VirusDbVersion.Should().Be(expectedVirusDbVersion);

        }
    }
}