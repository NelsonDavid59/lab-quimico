namespace AgroLaboratorio.Common.Results
{
    public record Error(string Code, ErrorType Type, string Description);

    public static class Errors
    {
        public static Error RscNotFound { get; } = 
            new("RscNotFound", ErrorType.NotFound, "Recurso no encontrado.");
        
        public static Error UnauthorizedUser { get; } = 
            new("UnauthorizedUser", ErrorType.Unauthorized, "Usuario no autorizado.");

        public static Error InternalServerError { get; } = 
            new("InternalServerError", ErrorType.Internal, "Error interno en el servidor.");
    }
}
