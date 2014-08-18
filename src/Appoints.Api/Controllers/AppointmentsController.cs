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
    public class AppointmentsController : ApiController
    {
        private readonly AppointsDbContext _dbContext;

        public AppointmentsController(AppointsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route("appointments")]
        public async Task<IHttpActionResult> GetByUser()
        {
            var currentPrincipal = this.Request.GetOwinContext().Authentication.User;
            var userId = Int32.Parse((currentPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value));
            var dbAppointments = await _dbContext.Appointments.Where(a => a.User.Id == userId)
                    .OrderByDescending(a => a.StartDateAndTime).ToArrayAsync();
            var appointments = dbAppointments
                .Select(a => new Appointment
                             {
                                 id = a.Id,
                                 title = a.Title,
                                 dateAndTime = a.StartDateAndTime,
                                 endDateAndTime = a.EndDateAndTime,
                                 duration = a.Duration,
                                 remarks = a.Remarks,
                                 userId = a.UserId
                             }).ToList();
            return Ok(new AppointmentList {ResourceList = appointments});
        }
    }
}