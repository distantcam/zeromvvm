using ApiApprover;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Xunit;

namespace ZeroMVVM.Tests
{
    [UseReporter(typeof(DiffReporter))]
    [UseApprovalSubdirectory("results")]
    public class APITests
    {
        [Fact]
        public void PublicAPI()
        {
            PublicApiApprover.ApprovePublicApi("ZeroMVVM.dll");
        }
    }
}