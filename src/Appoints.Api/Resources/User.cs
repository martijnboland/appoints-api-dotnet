using WebApi.Hal;

namespace Appoints.Api.Resources
{
    public class User : Representation
    {
        public int id { get; set; }
        public string userId { get; set; }
        public string provider { get; set; }
        public string email { get; set; }
        public string displayName { get; set; }
        public string[] roles { get; set; }

        public override string Href
        {
            get { return LinkTemplates.Users.User.Href; }
            set { }
        }

        public override string Rel
        {
            get { return LinkTemplates.Users.User.Rel; }
            set { }
        }

        protected override void CreateHypermedia()
        {
        }
    }
}