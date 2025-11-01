using System.ComponentModel.DataAnnotations;

namespace AgroLaboratorio.ViewModels.UsersVMS
{
    public class LoginVM
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
