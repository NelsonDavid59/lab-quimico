namespace AgroLaboratorio.Utils
{
    public static class ErrorMessages
    {
        public const string RequiredField = "El campo {0} es requerido.";
        public const string Save = "Ha ocurrido un error al intentar GUARDAR el recurso.";
        public const string Update = "Ha ocurrido un error al intentar ACTUALIZAR el recurso.";
        public const string Delete = "Ha ocurrido un error al intentar ELIMINAR el recurso.";
        public const string ResourceNotFound = "No se ha encontrado el recurso requerido.";
        public const string InternalDbError = "Error interno de servicio de base de datos.";
        public const string InvalidId = "No existe {0} con el código especificado.";
    }
}
