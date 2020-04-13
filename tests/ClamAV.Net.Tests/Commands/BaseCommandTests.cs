using System;
using ClamAV.Net.Commands.Base;
using Xunit;

namespace ClamAV.Net.Tests.Commands
{
    public class BaseCommandTests
    {
        private class TestCommand : BaseCommand
        {
            public TestCommand(string name) : base(name)
            {
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Base_Ctor_Validate_Name_Parameter(string name)
        {
            Assert.Throws<ArgumentNullException>(nameof(name), () => new TestCommand(name));
        }
    }
}