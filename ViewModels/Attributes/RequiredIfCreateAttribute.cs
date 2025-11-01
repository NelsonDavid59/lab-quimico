using System.ComponentModel.DataAnnotations;

namespace AgroLaboratorio.ViewModels.Attributes
{
    /**
     *  ESTA CLASE ES UN ATRIBUTO DE VALIDACIÓN PERSONALIZADO, PARA USARSE
     *  EN CIERTOS CAMPOS QUE NO SON OBLIGATORIOS AL EDITAR PERO SI AL MOMENTO DE CREAR.
     */
    public class RequiredIfCreateAttribute : ValidationAttribute
    {
        private readonly string _codPropName;

        public RequiredIfCreateAttribute(string propName) : base()
        {
            _codPropName = propName;
        }

        public RequiredIfCreateAttribute(string propName, string errorMessage) : base(errorMessage) 
        {
            _codPropName = propName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var codProp = validationContext.ObjectType.GetProperty(_codPropName);

            if (codProp == null)
            {
                //Se lanza una excepción en caso de que no exista la propiedad
                throw new InvalidOperationException($"La propiedad {_codPropName} no existe en el ViewModel especificado.");
            }

            var codVal = codProp.GetValue(validationContext.ObjectInstance);

            //si no existe un codigo para el objeto se asume que se esta intentando crear
            if(codVal == null || string.IsNullOrWhiteSpace(codVal.ToString()))
            {
                //Se verifica el valor de que se quiere validar
                if(value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            //Éxito
            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name);
        }
    }
}
