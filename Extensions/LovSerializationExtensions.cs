using AgroLaboratorio.ViewModels.Attributes;
using AgroLaboratorio.ViewModels.Lovs.Base;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace AgroLaboratorio.Extensions
{
    public static class LovSerializationExtensions
    {
        /// <summary>
        /// Método de extensión que se puede llamar desde cualquier IEnumerable<T> donde T hereda de LovBaseVM
        /// Por ejemplo: listaPersonas.SerializeLovItems()
        /// </summary>
        /// <param name="items">La colección de objetos LOV que queremos serializar</param>
        /// <returns>Un JsonDocument con la estructura enriquecida</returns>
        public static JsonDocument SerializeLovItems<T>(this IEnumerable<T> items) where T : LovBaseVM
        {
            // Select() toma cada item y lo transforma usando la función EnrichObject
            // ToList() materializa el resultado en una List<object>
            var enrichedItems = items.Select(item => LovItemObjec(item)).ToList();

            // JsonSerializer.Serialize() convierte el objeto C# a texto JSON
            var json = JsonSerializer.Serialize(enrichedItems);

            // JsonDocument.Parse() parsea el string JSON a un objeto JsonDocument
            return JsonDocument.Parse(json);
        }

        /**
         * Retorna un diccionario con todas las propiedades del ViewModel
        *   Clave: Nombre cada propiedad del objeto
        *   Valor: Cada titulo asociado al DisplayName de la propidad
        *       y el valor del campo.
        *       {titulo, valor}
        */
        private static object LovItemObjec<T>(T lovVM)
        {
            var result = new Dictionary<string, object>();

            var type = typeof(T);

            //Obtenemos TODAS las propiedades públicas del tipo
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            //Recorremos las propiedades
            foreach (var prop in properties) 
            {
                // GetCustomAttribute<DisplayNameAttribute>(): busca el atributo [DisplayName]
                // ?.DisplayName: si existe el atributo, toma su DisplayName
                // ?? prop.Name: si no existe el atributo, usa el nombre de la propiedad
                var displayName = prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? prop.Name;

                // Atributo para saber si se puede seleccionar el campo
                var isSearchable = prop.GetCustomAttribute<SearchableAttribute>()?.IsSearchable ?? false;

                var value = prop.GetValue(lovVM);

                //Creamos la estructura title/ value y la agregamos al diccionario
                result[prop.Name] = new
                {
                    title = displayName,
                    value = value,
                    searchable = isSearchable
                };
            }

            return result;
        }
    }
}
