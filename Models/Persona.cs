using System;
using System.Collections.Generic;

namespace AgroLaboratorio.Models;

public partial class Persona
{
    public int CodPersona { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string? Documento { get; set; }

    public string TipoPersona { get; set; } = null!;

    public string CodUserAlta { get; set; } = null!;

    public DateTime FechaAlta { get; set; }

    public string? CodUserModif { get; set; }

    public DateTime? FechaModif { get; set; }

    public virtual ICollection<SolAnalisisCab> SolAnalisisCabs { get; set; } = new List<SolAnalisisCab>();
}
