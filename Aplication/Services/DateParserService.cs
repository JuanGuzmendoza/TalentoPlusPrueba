using TalentoPlus.Application.Interfaces;

namespace TalentoPlus.Application.Services
{
    public class DateParserService : IDateParserService
    {
        public DateTime ParseDate(string dateString, DateTime defaultValue)
        {
            if (string.IsNullOrWhiteSpace(dateString)) 
                return DateTime.SpecifyKind(defaultValue, DateTimeKind.Utc);

            var cleanString = dateString.Trim().Replace("'", "");

            if (DateTime.TryParse(cleanString, out DateTime parsedDate))
            {
                return DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
            }

            return DateTime.SpecifyKind(defaultValue, DateTimeKind.Utc);
        }
    }
}