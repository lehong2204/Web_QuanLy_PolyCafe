using Microsoft.Extensions.Configuration;
using Web_QuanLy_PolyCafe.Services;

namespace Web_QuanLy_PolyCafe.Tests
{
    public class FakeEmailService : EmailService
    {
        public bool WasCalled { get; private set; }

        public FakeEmailService(IConfiguration config) : base(config) { }

        public override Task SendAsync(string to, string subject, string body) // ✅ override
        {
            WasCalled = true;
            return Task.CompletedTask;
        }
    }
}
