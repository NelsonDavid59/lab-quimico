using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AgroLaboratorio.Utils;

namespace AgroLaboratorio.ViewModels.ElemQuimicos;

public class ElemQuimicoVM
{
    private string _codElemento;

    [Required(ErrorMessage = ErrorMessages.RequiredField)]
    [DisplayName("SIGLA")]
    public string CodElemento { get => _codElemento; set => _codElemento = value?.ToUpper(); }

    [Required(ErrorMessage = ErrorMessages.RequiredField)]
    [DisplayName("DESCRIPCIÓN")]
    public string Descripcion { get; set; }

}
