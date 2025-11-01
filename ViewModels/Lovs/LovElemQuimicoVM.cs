using System.ComponentModel;
using AgroLaboratorio.ViewModels.Lovs.Base;

namespace AgroLaboratorio.ViewModels.Lovs
{
    public class LovElemQuimicoVM : LovBaseVM
    {
        [DisplayName("DESCRIPCIÓN")]
        public string Descripcion { get; set; }
    }
}
