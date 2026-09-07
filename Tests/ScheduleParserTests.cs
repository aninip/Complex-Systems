using Complex_Systems.Model;
using Complex_Systems.Parsing;

namespace Complex_Systems.Tests;

public sealed class ScheduleParserTests
{
    private readonly ScheduleParser _parser = new();

    [Fact]
    public void Parse_FullFormatWithMilliseconds_ReturnsCorrectDefinition()
    {
        ScheduleDefinition definition =
            _parser.Parse("2026.09.15 1-5 10:20:30.500");

        Assert.True(definition.Years.Contains(2026));
        Assert.True(definition.Months.Contains(9));
        Assert.True(definition.Days.Contains(15));

        Assert.True(definition.Weekdays.Contains(1));
        Assert.True(definition.Weekdays.Contains(2));
        Assert.True(definition.Weekdays.Contains(3));
        Assert.True(definition.Weekdays.Contains(4));
        Assert.True(definition.Weekdays.Contains(5));
        Assert.False(definition.Weekdays.Contains(0));
        Assert.False(definition.Weekdays.Contains(6));

        Assert.True(definition.Hours.Contains(10));
        Assert.True(definition.Minutes.Contains(20));
        Assert.True(definition.Seconds.Contains(30));
        Assert.True(definition.Milliseconds.Contains(500));
    }

    [Fact]
    public void Parse_DateAndTimeWithMilliseconds_UsesWildcardWeekday()
    {
        ScheduleDefinition definition =
            _parser.Parse("2026.09.15 10:20:30.500");

        Assert.True(definition.Years.Contains(2026));
        Assert.True(definition.Months.Contains(9));
        Assert.True(definition.Days.Contains(15));

        for (int weekday = 0; weekday <= 6; weekday++)
        {
            Assert.True(definition.Weekdays.Contains(weekday));
        }

        Assert.True(definition.Hours.Contains(10));
        Assert.True(definition.Minutes.Contains(20));
        Assert.True(definition.Seconds.Contains(30));
        Assert.True(definition.Milliseconds.Contains(500));
    }

    [Fact]
    public void Parse_TimeWithMilliseconds_UsesWildcardDateAndWeekday()
    {
        ScheduleDefinition definition =
            _parser.Parse("10:20:30.500");

        for (int year = 2000; year <= 2100; year++)
        {
            Assert.True(definition.Years.Contains(year));
        }

        for (int month = 1; month <= 12; month++)
        {
            Assert.True(definition.Months.Contains(month));
        }

        for (int day = 1; day <= 32; day++)
        {
            Assert.True(definition.Days.Contains(day));
        }

        for (int weekday = 0; weekday <= 6; weekday++)
        {
            Assert.True(definition.Weekdays.Contains(weekday));
        }

        Assert.True(definition.Hours.Contains(10));
        Assert.True(definition.Minutes.Contains(20));
        Assert.True(definition.Seconds.Contains(30));
        Assert.True(definition.Milliseconds.Contains(500));
    }

    [Fact]
    public void Parse_FullFormatWithoutMilliseconds_SetsMillisecondsToZero()
    {
        ScheduleDefinition definition =
            _parser.Parse("2026.09.15 1-5 10:20:30");

        Assert.True(definition.Years.Contains(2026));
        Assert.True(definition.Months.Contains(9));
        Assert.True(definition.Days.Contains(15));

        for (int weekday = 1; weekday <= 5; weekday++)
        {
            Assert.True(definition.Weekdays.Contains(weekday));
        }

        Assert.True(definition.Hours.Contains(10));
        Assert.True(definition.Minutes.Contains(20));
        Assert.True(definition.Seconds.Contains(30));
        Assert.True(definition.Milliseconds.Contains(0));

        Assert.False(definition.Milliseconds.Contains(1));
        Assert.False(definition.Milliseconds.Contains(999));
    }

    [Fact]
    public void Parse_DateAndTimeWithoutMilliseconds_UsesWildcardWeekdayAndZeroMilliseconds()
    {
        ScheduleDefinition definition =
            _parser.Parse("2026.09.15 10:20:30");

        Assert.True(definition.Years.Contains(2026));
        Assert.True(definition.Months.Contains(9));
        Assert.True(definition.Days.Contains(15));

        for (int weekday = 0; weekday <= 6; weekday++)
        {
            Assert.True(definition.Weekdays.Contains(weekday));
        }

        Assert.True(definition.Hours.Contains(10));
        Assert.True(definition.Minutes.Contains(20));
        Assert.True(definition.Seconds.Contains(30));
        Assert.True(definition.Milliseconds.Contains(0));
    }

    [Fact]
    public void Parse_TimeWithoutMilliseconds_UsesWildcardDateAndWeekday()
    {
        ScheduleDefinition definition =
            _parser.Parse("10:20:30");

        Assert.True(definition.Years.Contains(2026));
        Assert.True(definition.Months.Contains(9));
        Assert.True(definition.Days.Contains(15));

        for (int weekday = 0; weekday <= 6; weekday++)
        {
            Assert.True(definition.Weekdays.Contains(weekday));
        }

        Assert.True(definition.Hours.Contains(10));
        Assert.True(definition.Minutes.Contains(20));
        Assert.True(definition.Seconds.Contains(30));
        Assert.True(definition.Milliseconds.Contains(0));
    }

    [Fact]
    public void Parse_TimeOnly_PreservesWildcardDate()
    {
        ScheduleDefinition definition =
            _parser.Parse("10:20:30");

        Assert.False(definition.Years.Contains(1999));
        Assert.False(definition.Years.Contains(2101));

        Assert.False(definition.Months.Contains(0));
        Assert.False(definition.Months.Contains(13));

        Assert.False(definition.Days.Contains(0));
        Assert.False(definition.Days.Contains(33));
    }

    [Fact]
    public void Parse_DateExpressionRequiresExactlyThreeComponents()
    {
        Assert.Throws<ArgumentException>(
            () => _parser.Parse("2026.09 10:20:30"));

        Assert.Throws<ArgumentException>(
            () => _parser.Parse("2026.09.15.1 10:20:30"));
    }

    [Fact]
    public void Parse_DateExpressionWithEmptyComponent_IsRejected()
    {
        Assert.Throws<ArgumentException>(
            () => _parser.Parse("2026..15 10:20:30"));

        Assert.Throws<ArgumentException>(
            () => _parser.Parse(".09.15 10:20:30"));

        Assert.Throws<ArgumentException>(
            () => _parser.Parse("2026.09. 10:20:30"));
    }

    [Fact]
    public void Parse_TimeExpressionRequiresThreeComponents()
    {
        Assert.Throws<ArgumentException>(
            () => _parser.Parse("10:20 10:20:30"));

        Assert.Throws<ArgumentException>(
            () => _parser.Parse("10:20:30:40"));
    }

    [Fact]
    public void Parse_TimeExpressionWithMoreThanOneDot_IsRejected()
    {
        Assert.Throws<ArgumentException>(
            () => _parser.Parse("10:20:30.100.200"));
    }

    [Fact]
    public void Parse_TimeExpressionWithEmptyComponent_IsRejectedByFieldParser()
    {
        Assert.Throws<ArgumentException>(
            () => _parser.Parse("10::30"));

        Assert.Throws<ArgumentException>(
            () => _parser.Parse("10:20:"));

        Assert.Throws<ArgumentException>(
            () => _parser.Parse(":20:30"));
    }

    [Fact]
    public void Parse_EmptyString_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => _parser.Parse(""));
    }

    [Fact]
    public void Parse_WhitespaceString_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => _parser.Parse("   "));
    }

    [Fact]
    public void Parse_TooManyTokens_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => _parser.Parse(
                "2026.09.15 1-5 10:20:30 500"));
    }

    [Fact]
    public void Parse_MultipleSpaces_AreIgnored()
    {
        ScheduleDefinition definition =
            _parser.Parse(
                "   2026.09.15   1-5   10:20:30.500   ");

        Assert.True(definition.Years.Contains(2026));
        Assert.True(definition.Months.Contains(9));
        Assert.True(definition.Days.Contains(15));
        Assert.True(definition.Weekdays.Contains(1));
        Assert.True(definition.Weekdays.Contains(5));
        Assert.True(definition.Hours.Contains(10));
        Assert.True(definition.Minutes.Contains(20));
        Assert.True(definition.Seconds.Contains(30));
        Assert.True(definition.Milliseconds.Contains(500));
    }

    [Fact]
    public void Parse_InvalidHour_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => _parser.Parse("24:00:00"));
    }

    [Fact]
    public void Parse_InvalidMinute_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => _parser.Parse("10:60:00"));
    }

    [Fact]
    public void Parse_InvalidSecond_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => _parser.Parse("10:00:60"));
    }

    [Fact]
    public void Parse_InvalidMillisecond_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => _parser.Parse("10:00:00.1000"));
    }

    [Fact]
    public void Parse_InvalidWeekday_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => _parser.Parse("2026.09.15 7 10:00:00"));
    }

    [Fact]
    public void Parse_InvalidYear_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => _parser.Parse("1999.09.15 10:00:00"));

        Assert.Throws<ArgumentException>(
            () => _parser.Parse("2101.09.15 10:00:00"));
    }

    [Fact]
    public void Parse_InvalidMonth_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => _parser.Parse("2026.13.15 10:00:00"));

        Assert.Throws<ArgumentException>(
            () => _parser.Parse("2026.00.15 10:00:00"));
    }

    [Fact]
    public void Parse_InvalidDay_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => _parser.Parse("2026.09.33 10:00:00"));
    }
}