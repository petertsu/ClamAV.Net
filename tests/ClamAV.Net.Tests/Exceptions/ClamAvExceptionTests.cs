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
    }
}