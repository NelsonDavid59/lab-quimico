namespace AgroLaboratorio.Utils
{
    /**
     * ESTA CLASE TIENE EL FIN DE GESTIONAR FECHAS.
     */
    public static class AppTime
    {
        private static readonly TimeZoneInfo _localTZ =
            TimeZoneInfo.FindSystemTimeZoneById("America/Asuncion");
        public static DateTime NowLocal()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _localTZ);
        }
    }
}
