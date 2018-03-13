using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HumanMobility.Startup))]
namespace HumanMobility
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
