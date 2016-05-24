using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AAAService.Startup))]
namespace AAAService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
