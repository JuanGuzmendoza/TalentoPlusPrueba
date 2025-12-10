namespace TalentoPlus.Application.Interfaces
{
    public interface IDateParserService
    {
        DateTime ParseDate(string dateString, DateTime defaultValue);
    }
}