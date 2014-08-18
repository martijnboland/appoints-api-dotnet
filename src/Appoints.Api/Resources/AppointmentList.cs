using WebApi.Hal;

namespace Appoints.Api.Resources
{
    public class AppointmentList : SimpleListRepresentation<Appointment>
    {
        protected override void CreateHypermedia()
        {
            Href = LinkTemplates.Appointments.Get.Href;

            Links.Add(new Link { Href = Href, Rel = "self" });
        }
    }
}