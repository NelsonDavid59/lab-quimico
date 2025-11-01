using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AgroLaboratorio.Utils;

namespace AgroLaboratorio.ViewModels.Personas
{
    public class PersonaVM
    {
        public int? CodPersona { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [DisplayName("Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [DisplayName("Apellido")]
        public string Apellido { get; set; }

        [DisplayName("Documento")]
        public string? Documento { get; set; }
    }
}
