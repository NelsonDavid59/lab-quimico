using System.ComponentModel.DataAnnotations;
using AgroLaboratorio.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using AgroLaboratorio.ViewModels.Attributes;

namespace AgroLaboratorio.ViewModels.UsersVMS
{
    public class RegistrationVM
    {
        [Display(Name = "Cod. Usuario")]
        public string? CodUsuario { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [Display(Name = "Nombre de Usuario")]
        public string Username { get; set; }

        [RequiredIfCreate(nameof(CodUsuario), ErrorMessage = ErrorMessages.RequiredField)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string? Password { get; set; }

        [RequiredIfCreate(nameof(CodUsuario), ErrorMessage = ErrorMessages.RequiredField)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmación de Contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [Display(Name = "Perfil de Usuario")]
        public string CodPerfil { get; set; }

        [ValidateNever]
        public List<SelectListItem> Perfiles { get; set; }
    }
}
