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
    }
}
