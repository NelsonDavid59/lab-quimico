using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;

namespace AgroLaboratorio.Models;

/*
  OBS: Clase que hereda de IdentityUser con clave de tipo 'string'
    - Este es el usuario personalizado del sistema
 */
public partial class Usuario : IdentityUser<string>
{
    [NotMapped]
    public string CodUsuario
    {
        get => this.Id;
        set => this.Id = value;
    }

    //public string Clave { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    //public int CodPerfil { get; set; }

    public bool Estado { get; set; }

    public string CodUserAlta { get; set; } = null!;

    public DateTime FechaAlta { get; set; }

    public string? CodUserModif { get; set; }

    public DateTime? FechaModif { get; set; }
}
