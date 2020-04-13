using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ClamAV.Net.Client;
using ClamAV.Net.Client.Results;
using ClamAV.Net.Commands;
using ClamAV.Net.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace ClamAV.Net.Tests.Commands
{
    public class InStreamCommandTests
    {
        [Fact]
        public void Ctor_Set_Valid_Name()
        {
            Mock<Stream> mock = new Mock<Stream>();
            mock.Setup(stream => stream.CanRead).Returns(true);

            InStreamCommand inStreamCommand = new InStreamCommand(mock.Object);
            inStreamCommand.Name.Should().Be("INSTREAM");
        }

        [Fact]
        public void Ctor_DataStream_Null_Should_Throw_ArgumentNullException1()
        {
            Mock<Stream> mock = new Mock<Stream>();
            mock.Setup(stream => stream.CanRead).Returns(false);

            Assert.Throws<ArgumentException>("dataStream", () => new InStreamCommand(mock.Object));
        }

        [Fact]
        public void Ctor_DataStream_Null_Should_Throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new InStreamCommand(null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("stream: Win.Test.EICAR_HDB-1 FOUND\0")]
        [InlineData("stream: OK\0")]
        [InlineData("ERROR")]
        [InlineData("FOUND")]
        public async Task ProcessRawResponseAsync_Invalid_Raw_Data_Should_Throw_exception(string rawData)
        {
            Mock<Stream> mock = new Mock<Stream>();
            mock.Setup(stream => stream.CanRead).Returns(true);

            InStreamCommand inStreamCommand = new InStreamCommand(mock.Object);

            byte[] rawBytes = rawData == null ? null : Encoding.UTF8.GetBytes(rawData);

            await Assert.ThrowsAsync<ClamAvException>(async () =>
                await inStreamCommand.ProcessRawResponseAsync(rawBytes).ConfigureAwait(false));
        }

        [Theory]
        [InlineData("stream: Win.Test.EICAR_HDB-1 FOUND", true , "Win.Test.EICAR_HDB-1")]
        [InlineData("stream: OK", false, null)]

        public async Task ProcessRawResponseAsync_Valid_Raw_Data_Should_Return_ScanResult(string rawData,
            bool expectedInfected , string expectedVirusName)
        {
            Mock<Stream> mock = new Mock<Stream>();
            mock.Setup(stream => stream.CanRead).Returns(true);

            InStreamCommand inStreamCommand = new InStreamCommand(mock.Object);

            ScanResult actualResult = await inStreamCommand.ProcessRawResponseAsync(Encoding.UTF8.GetBytes(rawData))
                .ConfigureAwait(false);

            actualResult.Should().NotBeNull();
            actualResult.Infected.Should().Be(expectedInfected);
            actualResult.VirusName.Should().Be(expectedVirusName);


        }
    }
}