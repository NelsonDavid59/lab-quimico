using System.ComponentModel;
using AgroLaboratorio.ViewModels.Lovs.Base;

namespace AgroLaboratorio.ViewModels.Lovs
{
    public class LovSolublVM : LovBaseVM
    {
        [DisplayName("DESCRIPCION")]
        public string Descripcion { get; set; }
    }
}
