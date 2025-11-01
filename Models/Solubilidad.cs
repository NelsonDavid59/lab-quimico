using System;
using System.Collections.Generic;

namespace AgroLaboratorio.Models;

public partial class Solubilidad
{
    public int CodSoluble { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<SolAnalisisDet> SolAnalisisDets { get; set; } = new List<SolAnalisisDet>();
}
