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
                .Select(MapAppointment).ToList();
            return Ok(new AppointmentList {ResourceList = appointments});
        }

        [Route("appointments")]
        public async Task<IHttpActionResult> Post(Appointment appointment)
        {
            var currentPrincipal = this.Request.GetOwinContext().Authentication.User;
            var userId = Int32.Parse((currentPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value));
            var dbAppointment = new Appoints.Core.Domain.Appointment
                                {
                                    Title = appointment.title,
                                    StartDateAndTime = appointment.dateAndTime,
                                    EndDateAndTime = appointment.endDateAndTime,
                                    Remarks = appointment.remarks,
                                    UserId = userId
                                };
            try
            {
                _dbContext.Appointments.Add(dbAppointment);
                await _dbContext.SaveChangesAsync();
                var url = LinkTemplates.Appointments.Appointment.CreateLink(dbAppointment.Id).Href;
                return Created(url, MapAppointment(dbAppointment));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("appointments/{id}")]
        public async Task<IHttpActionResult> Patch(int id, Appointment appointment)
        {
            try
            {
                var dbAppointment = await _dbContext.Appointments.FindAsync(id);
                if (dbAppointment == null)
                {
                    return NotFound();
                }
                if (appointment.dateAndTime != DateTime.MinValue)
                {
                    dbAppointment.StartDateAndTime = appointment.dateAndTime;
                }
                if (appointment.endDateAndTime != DateTime.MinValue)
                {
                    dbAppointment.EndDateAndTime = appointment.endDateAndTime;
                }
                if (appointment.title != null)
                {
                    dbAppointment.Title = appointment.title;
                }
                if (appointment.remarks != null)
                {
                    dbAppointment.Remarks = appointment.remarks;
                }
                await _dbContext.SaveChangesAsync();
                return Ok(MapAppointment(dbAppointment));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("appointments/{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            try
            {
                var dbAppointment = await _dbContext.Appointments.FindAsync(id);
                if (dbAppointment == null)
                {
                    return NotFound();
                }
                _dbContext.Appointments.Remove(dbAppointment);
                await _dbContext.SaveChangesAsync();
                return Ok(new {message = "Appointment deleted"});
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private Appointment MapAppointment(Appoints.Core.Domain.Appointment dbAppointment)
        {
            return new Appointment
                   {
                       id = dbAppointment.Id,
                       title = dbAppointment.Title,
                       dateAndTime = dbAppointment.StartDateAndTime,
                       endDateAndTime = dbAppointment.EndDateAndTime,
                       duration = dbAppointment.Duration,
                       remarks = dbAppointment.Remarks,
                       userId = dbAppointment.UserId
                   };
        }
    }
}