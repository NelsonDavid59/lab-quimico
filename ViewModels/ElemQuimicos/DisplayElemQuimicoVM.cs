namespace AgroLaboratorio.ViewModels.ElemQuimicos
{
    public class DisplayElemQuimicoVM
    {
        public string CodElemento { get; set; }
        public string Descripcion { get; set; }
        public string CodUserAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public string? CodUserModif { get; set; }
        public DateTime? FechaModif { get; set; }
    }
}
