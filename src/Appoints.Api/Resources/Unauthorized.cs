using WebApi.Hal;

namespace Appoints.Api.Resources
{
    public class Unauthorized : Representation
    {
        public string Message { get; set; }

        public string Details { get; set; }

        protected override void CreateHypermedia()
        {
            Links.Add(LinkTemplates.Auth.Facebook);
            Links.Add(LinkTemplates.Auth.Google);
        }
    }
}