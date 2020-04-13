using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ClamAV.Net.Exceptions;
using FluentAssertions;
using Xunit;

namespace ClamAV.Net.Tests.Exceptions
{
    public class ClamAvExceptionTests
    {
        [Fact]
        public void Ctor_Validation()
        {
            ClamAvException clamAvException = new ClamAvException();
            clamAvException.Message.Should().NotBeNullOrEmpty();

            const string expectedMessage = "Some error";
            clamAvException = new ClamAvException(expectedMessage);
            clamAvException.Message.Should().Be(expectedMessage);

            clamAvException = new ClamAvException(expectedMessage, new Exception("Error"));
            clamAvException.Message.Should().Be(expectedMessage);
            clamAvException.InnerException.Should().NotBeNull();
        }

        [Fact]
        public void Serialization_Test()
        {
            const string expectedMessage = "Some error";
            ClamAvException clamAvException = new ClamAvException(expectedMessage, new Exception("Error"));

            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using MemoryStream memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, clamAvException);
            memoryStream.Seek(0, SeekOrigin.Begin);

            ClamAvException actual = binaryFormatter.Deserialize(memoryStream) as ClamAvException;

            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(clamAvException);
        }
    }
}