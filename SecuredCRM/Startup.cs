using Owin;

namespace SecuredCRM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            ConfigureAuth(app);
        }
    }
}
