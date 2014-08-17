using System;
using System.Data.Entity;
using System.IdentityModel.Claims;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Appoints.Api.Resources;
using Appoints.Core.Data;

namespace Appoints.Api.Controllers
{
    [Authorize]
    public class UsersController : ApiController
    {
        private readonly AppointsDbContext _dbContext;

        public UsersController(AppointsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route("me")]
        public async Task<IHttpActionResult> GetMe()
        {
            var currentPrincipal = this.Request.GetOwinContext().Authentication.User;
            var userId = Int32.Parse((currentPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value));
            return await Get(userId);
        }

        [Route("users/{id}")]
        public async Task<IHttpActionResult> Get(int id)
        {
            var dbUser = await _dbContext.Users
                .Include(u => u.UserRoles.Select(ur => ur.Role))
                .SingleOrDefaultAsync(u => u.Id == id);
            if (dbUser == null)
            {
                return NotFound();
            }
            return Ok(new User
                      {
                          id = dbUser.Id,
                          userId = dbUser.ProviderUserId,
                          provider = dbUser.Provider,
                          email = dbUser.Email,
                          displayName = dbUser.DisplayName,
                          roles = dbUser.GetRolesAsString()
                      });
        }
    }
}