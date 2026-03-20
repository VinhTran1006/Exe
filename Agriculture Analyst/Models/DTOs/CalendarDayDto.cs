
// Models/DTO/CalendarDayDto.cs
public class CalendarDayDto
{
    public int Day { get; set; }
    public string? Activity { get; set; }
    public string? Description { get; set; }
    public string? Weather { get; set; }
}

// Models/DTO/DiarySummaryDto.cs
public class CalenDiarySummaryDto
{
    public int TotalEntries { get; set; }
    public int ActiveDays { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public string? MostCommonActivity { get; set; }
    public string? MostCommonWeather { get; set; }
    public int TaskCompletedCount { get; set; }
    public int TaskTotalCount { get; set; }
    public List<WeeklyActivityDto> WeeklyActivity { get; set; } = new();
    public List<ActivityTypeDto> ActivityBreakdown { get; set; } = new();
    public List<WeatherStatDto> WeatherStats { get; set; } = new();
}

// Models/DTO/WeeklyActivityDto.cs
public class WeeklyActivityDto
{
    public int Week { get; set; }       // 1–5 (tuần trong tháng)
    public string Label { get; set; } = "";
    public int Count { get; set; }
}

// Models/DTO/ActivityTypeDto.cs
public class ActivityTypeDto
{
    public string Activity { get; set; } = "";
    public int Count { get; set; }
    public double Percent { get; set; }
}

// Models/DTO/WeatherStatDto.cs
public class WeatherStatDto
{
    public string Weather { get; set; } = "";
    public int Count { get; set; }
}