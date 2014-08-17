using System.Web.Http;
using Appoints.Api.Resources;

namespace Appoints.Api.Controllers
{
    public class IndexController : ApiController
    {
        // GET /
        [OverrideAuthentication]
        [AllowAnonymous]
        public Root Get()
        {
            return new Root();
        }
    }
}