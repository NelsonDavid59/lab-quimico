using System.ComponentModel;
using AgroLaboratorio.ViewModels.Attributes;

namespace AgroLaboratorio.ViewModels.Lovs.Base
{
    public abstract class LovBaseVM
    {
        [DisplayName("CÓDIGO")]
        [Searchable]
        public object Codigo { get; set; }
    }
}
