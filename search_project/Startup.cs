using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(search_project.Startup))]
namespace search_project
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
