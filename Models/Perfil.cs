using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AgroLaboratorio.Models;

public partial class Perfil : IdentityRole<string>
{
    [NotMapped]
    public string CodPerfil { get => this.Id; set => this.Id = value; }

    public string Descripcion { get; set; } = null!;

    public bool? Estado { get; set; }

    public string CodUserAlta { get; set; } = null!;

    public DateTime FechaAlta { get; set; }

    public string? CodUserModif { get; set; }

    public DateTime? FechaModif { get; set; }
}
