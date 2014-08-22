using System;
using Newtonsoft.Json;
using WebApi.Hal;

namespace Appoints.Api.Resources
{
    public class Appointment : Representation
    {
        public int id { get; set; }
        public string title { get; set; }
        public DateTime dateAndTime { get; set; }
        public DateTime endDateAndTime { get; set; }
        public int duration { get; set; }
        public string remarks { get; set; }
        [JsonIgnore]
        public int userId { get; set; }

        public override string Href
        {
            get { return LinkTemplates.Appointments.Appointment.CreateLink(new { id }).Href; } // don't do a templated link because angular-hal chokes on it.
            set { }
        }

        public override string Rel
        {
            get { return LinkTemplates.Appointments.Appointment.Rel; }
            set { }
        }


        protected override void CreateHypermedia()
        {
            Links.Add(LinkTemplates.Users.User.CreateLink(new { id = userId }));
        }
    }
}