namespace github2trello;

public static class DateExtensions
{
    /// <summary>
    ///     Ensures the day part of a date in the format YYYY-MM-DD is valid
    /// </summary>
    /// <param name="date">Date to fix if invalid</param>
    /// <returns>The original date with the correct number of days if invalid for that month.</returns>
    public static string FixMonthDays(string? date)
    {
        if (date == null)
        {
            throw new ArgumentException("No date was given, expected one in format YYYY-MM-DD");
        }
        
        var split = date.Split("-");

        if (split.Length != 3)
        {
            throw new ArgumentException($"Invalid date: {date}. Format expected: YYYY-MM-DD");
        }

        if (!int.TryParse(split[0], out var year))
        {
            throw new ArgumentException($"{split[0]} is not a valid numeric year.");
        }
        
        if (!int.TryParse(split[1], out var month))
        {
            throw new ArgumentException($"{split[1]} is not a valid numeric month");
        }

        if (!int.TryParse(split[2], out var day))
        {
            throw new ArgumentException($"{split[2]} is not a valid numeric day");
        }

        var daysInMonth = DateTime.DaysInMonth(year, month);
        return daysInMonth < day ? $"{year}-{month}-{daysInMonth}" : date;
    }
}