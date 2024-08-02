namespace PrestamosCreciendo.Models
{
    public class DateOffset
    {
        public static DateTime DateNow(DateTime date, int offset)
        {
            DateTime dt = date.AddMinutes(offset);
            return dt;
        }
    }
}
