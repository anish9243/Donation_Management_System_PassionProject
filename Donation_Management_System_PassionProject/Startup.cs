using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Donation_Management_System_PassionProject.Startup))]
namespace Donation_Management_System_PassionProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
