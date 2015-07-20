using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CSS360.Web.Startup))]
namespace CSS360.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
