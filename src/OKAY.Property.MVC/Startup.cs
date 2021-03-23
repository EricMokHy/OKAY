using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OKAY.Property.MVC.Startup))]
namespace OKAY.Property.MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
