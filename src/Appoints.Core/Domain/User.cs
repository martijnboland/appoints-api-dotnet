using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Appoints.Core.Domain
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Provider { get; set; }

        [Required]
        public string ProviderUserId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public string ProviderAccessToken { get; set; }
        public string ProviderRefreshToken { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastAuthenticated { get; set; }
        public virtual ISet<UserRole> UserRoles { get; set; }

        public User()
        {
            this.UserRoles = new HashSet<UserRole>();
        }

        public string[] GetRolesAsString()
        {
            return UserRoles.Select(ur => ur.Role.Name).ToArray();
        }
    }
}