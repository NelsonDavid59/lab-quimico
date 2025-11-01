using System.ComponentModel;
using AgroLaboratorio.ViewModels.Lovs.Base;
using AgroLaboratorio.ViewModels.Attributes;

namespace AgroLaboratorio.ViewModels.Lovs
{
    public class LovPersonaVM : LovBaseVM
    {
        [DisplayName("Nombre y Apellido")]
        [Searchable] //Seleccionable para filtrar
        public string Nombre { get; set; }

        [Searchable]
        public string? Documento { get; set; }
    }
}
