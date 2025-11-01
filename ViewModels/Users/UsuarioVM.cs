namespace AgroLaboratorio.ViewModels.UsersVMS
{
    public class UsuarioVM
    {
        public string CodUsuario { get; set; }
        public string Username { get; set; }

        public string CodUserAlta { get; set; }

        public DateTime FechaAlta { get; set; }

        public string? CodUserModif { get; set; }

        public DateTime? FechaModif { get; set; }

        public bool Estado { get; set; }
    }
}
