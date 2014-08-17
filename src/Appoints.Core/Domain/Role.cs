using System.ComponentModel.DataAnnotations;

namespace Appoints.Core.Domain
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}