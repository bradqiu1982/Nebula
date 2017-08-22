using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Nebula.Startup))]
namespace Nebula
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
        }
    }
}
