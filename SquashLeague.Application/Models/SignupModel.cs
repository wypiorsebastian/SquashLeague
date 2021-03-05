using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace SquashLeague.Application.Models
{
    public class SignupModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [JsonIgnore]
        public string ConfirmationLink { get; set; }
    }
}
