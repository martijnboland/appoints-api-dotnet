using System;
using System.Web.Mvc;

namespace Appoints.Api
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
        }
    }
}