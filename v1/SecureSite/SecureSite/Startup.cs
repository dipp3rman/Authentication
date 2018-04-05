using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SecureSite.Startup))]
namespace SecureSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
