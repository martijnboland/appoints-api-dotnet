using WebApi.Hal;

namespace Appoints.Api.Resources
{
    public class Root : Representation
    {
        public string Message
        {
            get { return "Appoints service API"; }
        }

        public string Details
        {
            get { return "This is a REST api where you can schedule appointments for <insert business here>"; }
        }

        public override string Rel
        {
            get { return LinkTemplates.Root.Get.Rel; }
            set { }
        }

        public override string Href
        {
            get { return LinkTemplates.Root.Get.Href; }
            set { }
        }

        protected override void CreateHypermedia()
        {
            Links.Add(LinkTemplates.Users.Me);
            Links.Add(LinkTemplates.Appointments.Get);
        }
    }
}