namespace AgroLaboratorio.Extensions
{
    public static class DateTimeExtensions
    {
        //Para retornar fechas formateadas en string
        public static string ToShortPY(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy");
        }
    }
}
