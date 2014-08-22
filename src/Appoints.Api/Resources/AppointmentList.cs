using WebApi.Hal;

namespace Appoints.Api.Resources
{
    public class AppointmentList : SimpleListRepresentation<Appointment>
    {
        public override string Rel
        {
            get { return LinkTemplates.Appointments.Get.Rel; }
            set { }
        }

        public override string Href
        {
            get { return LinkTemplates.Appointments.Get.Href; }
            set { }
        }

        protected override void CreateHypermedia()
        {
            Links.Add(new Link { Href = Href, Rel = "self" });
        }
    }
}