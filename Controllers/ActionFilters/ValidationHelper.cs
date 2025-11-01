using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AgroLaboratorio.Controllers.ActionFilters
{
    public static class ValidationHelper
    {
        public static void AddModelErrorWithDisplayName<T>(
            ModelStateDictionary modelState,
            string prefix,
            string propertyName,
            string errorTemplate)
        {
            var displayName = GetDisplayName<T>(propertyName);
            var message = string.Format(errorTemplate, displayName);
            modelState.AddModelError($"{prefix}.{propertyName}", message);
        }

        private static string GetDisplayName<T>(string propName)
        {
            var prop = typeof(T).GetProperty(propName);

            if (prop == null) return propName;

            var displayAttr = prop.GetCustomAttribute<DisplayNameAttribute>();
            return displayAttr?.DisplayName ?? propName;
        }
    }
}
