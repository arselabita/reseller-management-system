using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ResellerManagementSystem.Startup))]

namespace ResellerManagementSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}