using System;
using System.Collections.Generic;

namespace AgroLaboratorio.Models;

public partial class SolAnalisisCab
{
    public int CodAnalisis { get; set; }

    public DateTime Fecha { get; set; }

    public int CodCliente { get; set; }

    public string? NombCliente { get; set; }

    public string Descripcion { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string CodUserAlta { get; set; } = null!;

    public DateTime FechaAlta { get; set; }

    public string? CodUserModif { get; set; }

    public DateTime? FechaModif { get; set; }

    public virtual Persona CodClienteNavigation { get; set; } = null!;

    public virtual ICollection<SolAnalisisDet> SolAnalisisDets { get; set; } = new List<SolAnalisisDet>();
}
