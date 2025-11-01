using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AgroLaboratorio.Utils;
using AgroLaboratorio.Extensions;
using Microsoft.Extensions.Options;
using AgroLaboratorio.Data;

namespace AgroLaboratorio.ViewModels.SolsAnalisis
{
    public class SolAnalisisVM
    {
        public int? CodAnalisis { get; set; }

        [DisplayName("DOCUMENTO")]
        public string? Documento { get; set; }

        [DisplayName("FECHA")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [DisplayName("CLIENTE")]
        public int CodCliente { get; set; }

        public string? NombreCliente { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [DisplayName("DESCRIPCION")]
        public string Descripcion { get; set; }

        public string? Estado { get; set; }

        public List<SolAnalisisDetVM> Detalles { get; set; } = new List<SolAnalisisDetVM>();

        
    }
}
