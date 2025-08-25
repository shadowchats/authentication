using Shadowchats.Authentication.Core.Application.Interfaces;

namespace Shadowchats.Authentication.Infrastructure.Tests.Bus
{
    namespace Shadowchats.Authentication.Infrastructure.Tests.Bus.Fakes
    {
        internal class TestCommand : ICommand<TestResult>
        {
            public string Data { get; set; } = string.Empty;
        }

        internal class TestResult
        {
            public string Value { get; set; } = string.Empty;
        }
    }
}