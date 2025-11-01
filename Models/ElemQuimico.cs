using System;
using System.Collections.Generic;

namespace AgroLaboratorio.Models;

public partial class ElemQuimico
{
    public string CodElemento { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public string CodUserAlta { get; set; } = null!;

    public DateTime FechaAlta { get; set; }

    public string? CodUserModif { get; set; }

    public DateTime? FechaModif { get; set; }

    public virtual ICollection<SolAnalisisDet> SolAnalisisDets { get; set; } = new List<SolAnalisisDet>();
}
