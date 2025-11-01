namespace AgroLaboratorio.ViewModels.Personas
{
    public class DisplayPersonaVM
    {
        public int CodPersona { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string? Documento { get; set; }
        public string Tipo { get; set; }
        public string CodUserAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public string? CodUseModif {  get; set; }
        public DateTime? FechaModif { get; set; }
    }
}
