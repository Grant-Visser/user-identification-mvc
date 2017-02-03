using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UserIdentificationMvc.Startup))]
namespace UserIdentificationMvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
