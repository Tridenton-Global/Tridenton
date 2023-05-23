namespace Tridenton.Extensions;

public static class DateTimeExtensions
{
    public static DateTime GetStartDate(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, 1);
    }

    public static DateTime GetEndDate(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month), 23, 59, 59);
    }

    //public static ReadOnlySpan<char> Beautify(this DateTime utcDateTime)
    //{
    //    var localDateTime = utcDateTime.ToLocalTime();

    //    var dateString = localDateTime.ToString(Constants.DateTimeFormat).AsSpan();

    //    var timeSpan = localDateTime - utcDateTime;

    //    var differenceInHours = timeSpan.Hours;

    //    var difference = (differenceInHours >= 0 ? $"(UTC+{HoursToString(differenceInHours)}:00)" : $"(UTC-{HoursToString(differenceInHours)}:00)").AsSpan();

    //    return $"{dateString} {difference}";
    //}

    //public static ReadOnlySpan<char> GetTimeSpan(this TimeSpan timeSpan)
    //{
    //    var answer = string.Empty;

    //    if (timeSpan.Hours > 0)
    //    {
    //        answer += $"{timeSpan.Hours}h ";
    //    }

    //    if (timeSpan.Minutes > 0)
    //    {
    //        answer += $"{timeSpan.Minutes}m ";
    //    }

    //    if (timeSpan.Seconds > 0)
    //    {
    //        answer += $"{timeSpan.Seconds}s";
    //    }

    //    return answer.AsSpan();
    //}

    //private static ReadOnlySpan<char> HoursToString(int hours)
    //{
    //    var hoursToString = hours.ToString().Replace("-", string.Empty).AsSpan();

    //    return hours < 10 ? $"0{hoursToString}" : hoursToString;
    //}
}