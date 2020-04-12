using System;
using System.IO;
using ClamAV.Net.Commands;
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

            Assert.Throws<ArgumentException>("dataStream",() => new InStreamCommand(mock.Object));
        }


        [Fact]
        public void Ctor_DataStream_Null_Should_Throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new InStreamCommand(null));
        }
    }
}