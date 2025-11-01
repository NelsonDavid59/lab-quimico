using System;
using System.Collections.Generic;

namespace AgroLaboratorio.Models;

public partial class SolAnalisisDet
{
    public int CodAnalisis { get; set; }

    public int NroLinea { get; set; }

    public string CodElemento { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public decimal GarantiaVal { get; set; }

    public int? CodSoluble { get; set; }

    public virtual SolAnalisisCab CodAnalisisNavigation { get; set; } = null!;

    public virtual ElemQuimico CodElementoNavigation { get; set; } = null!;

    public virtual Solubilidad? CodSolubleNavigation { get; set; }
}
