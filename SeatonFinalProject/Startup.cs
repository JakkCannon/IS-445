using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SeatonFinalProject.Startup))]
namespace SeatonFinalProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
