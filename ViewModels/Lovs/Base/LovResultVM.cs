using System.Text.Json;

namespace AgroLaboratorio.ViewModels.Lovs.Base
{
    public class LovResultVM
    {
        //Indica que se ha encontrado una coincidencia del campo de búsqueda
        public bool IsDataAvailable { get; set; } = false;
        public int TotalRows { get; set; } = 0;
        public int TotalPages { get; set; } = 0;

        public bool HasNextPage { get; set; } = false;
        public bool HasPreviousPage { get; set; } = false;

        public JsonDocument Items { get; set; } = default!;
    }
}
