namespace AgroLaboratorio.ViewModels.Attributes
{
    /**
     * Uso: Marca el atributo de un ViewModel como Searchable.
     * Es un campo que permite identificar si el atributo luego puede ser 
     * seleccionado para filtrar.
     */

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SearchableAttribute : Attribute
    {
        public bool IsSearchable { get; }

        public SearchableAttribute(bool value = true)
        {
            IsSearchable = value;
        }
    }
}
