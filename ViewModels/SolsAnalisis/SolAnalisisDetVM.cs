using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AgroLaboratorio.Data;
using AgroLaboratorio.Utils;

namespace AgroLaboratorio.ViewModels.SolsAnalisis
{
    public class SolAnalisisDetVM
    {   
        public int? CodAnalisis {  get; set; }

        [DisplayName("NRO. LÍNEA")]
        public int? NroLinea { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [DisplayName("ELEMENTO QUÍMICO")]
        public string CodElemento { get; set; }

        public string? DescElemento { get; set; }

        [DisplayName("VALOR DE GARANTÍA")]
        [Range(0, 9999.999, ErrorMessage = "El valor debe estar entre 0 y 9999,999.")]
        public decimal? GarantiaVal { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [DisplayName("SOLUBILIDAD")]
        public int? CodSoluble { get; set; }

        public string?  DescSolubilidad { get; set; }
    }
}
