using WebApi.Hal;

namespace Appoints.Api
{
    public static class LinkTemplates
    {
        public static class Root
        {
            public static Link Get
            {
                get { return new Link("", "~/"); }
            }
        }

        public static class Auth
        {
            public static Link Facebook
            {
                get { return new Link("auth_facebook", "~/auth/facebook"); }
            }

            public static Link Google
            {
                get { return new Link("auth_google", "~/auth/google"); }
            }

            public static Link LoggedIn
            {
                get { return new Link("auth_loggedin", "~/auth/loggedin"); }
            }
        }

        public static class Appointments
        {
            public static Link Get
            {
                get { return new Link("appointments", "~/appointments"); }
            }

            public static Link Appointment
            {
                get {  return new Link("appointment", "~/appointments/{id}"); }
            }
        }

        public static class Users
        {
            public static Link Me
            {
                get { return new Link("me", "~/me"); }
            }

            public static Link User
            {
                get { return new Link("user", "~/users/{id}"); }
            }
        }
    }
}