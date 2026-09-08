using Complex_Systems.Model;

namespace Complex_Systems.Tests;

public class ScheduleSearcherTests
{
    [Fact]
    public void FindNextOrSame_ReturnsSameTime_WhenTimeIsAllowed()
    {
        var definition = CreateDefinition(
        years: [2026],
        months: [9],
        days: [8],
        weekdays: [2],
        hours: [15],
        minutes: [42],
        seconds: [31],
        milliseconds: [500]);

        DateTime value = new(2026, 9, 8, 15, 42, 31, 500);

        DateTime result = Searching.ScheduleSearcher.FindNextOrSame(value, definition);

        Assert.Equal(value, result);
    }

    [Fact]
    public void FindNextOrSame_ReturnsNextAllowedMillisecond()
    {
        var definition = CreateDefinition(
            hours: [0],
            minutes: [0],
            seconds: [0],
            milliseconds: [100, 500, 900]);

        DateTime value = new(2026, 9, 8, 0, 0, 0, 500);

        DateTime result = Searching.ScheduleSearcher.FindNextOrSame(
            value.AddTicks(1),
            definition);

        Assert.Equal(new DateTime(2026, 9, 8, 0, 0, 0, 900), result);
    }

    [Fact]
    public void FindNext_ReturnsStrictlyLaterEvent()
    {
        var definition = CreateDefinition(
            hours: [0],
            minutes: [0],
            seconds: [0],
            milliseconds: [100, 500, 900]);

        DateTime value = new(2026, 9, 8, 0, 0, 0, 500);

        DateTime result = Searching.ScheduleSearcher.FindNext(value, definition);

        Assert.Equal(new DateTime(2026, 9, 8, 0, 0, 0, 900), result);
    }

    [Fact]
    public void FindPreviousOrSame_ReturnsSameTime_WhenTimeIsAllowed()
    {
        var definition = CreateDefinition(
            years: [2026],
            months: [9],
            days: [8],
            weekdays: [2],
            hours: [15],
            minutes: [42],
            seconds: [31],
            milliseconds: [500]);

        DateTime value = new(2026, 9, 8, 15, 42, 31, 500);

        DateTime result = Searching.ScheduleSearcher.FindPreviousOrSame(value, definition);

        Assert.Equal(value, result);
    }

    [Fact]
    public void FindPrevious_ReturnsStrictlyEarlierEvent()
    {
        var definition = CreateDefinition(
            hours: [0],
            minutes: [0],
            seconds: [0],
            milliseconds: [100, 500, 900]);

        DateTime value = new(2026, 9, 8, 0, 0, 0, 500);

        DateTime result = Searching.ScheduleSearcher.FindPrevious(value, definition);

        Assert.Equal(new DateTime(2026, 9, 8, 0, 0, 0, 100), result);
    }

    [Fact]
    public void FindNextOrSame_MovesToNextAllowedHour()
    {
        var definition = CreateDefinition(
            hours: [8, 12, 18],
            minutes: [0],
            seconds: [0],
            milliseconds: [0]);

        DateTime value = new(2026, 9, 8, 13, 30, 0, 0);

        DateTime result = Searching.ScheduleSearcher.FindNextOrSame(value, definition);

        Assert.Equal(new DateTime(2026, 9, 8, 18, 0, 0, 0), result);
    }

    [Fact]
    public void FindNextOrSame_MovesToNextAllowedMinute()
    {
        var definition = CreateDefinition(
            hours: [10],
            minutes: [0, 20, 40],
            seconds: [0],
            milliseconds: [0]);

        DateTime value = new(2026, 9, 8, 10, 25, 0, 0);

        DateTime result = Searching.ScheduleSearcher.FindNextOrSame(value, definition);

        Assert.Equal(new DateTime(2026, 9, 8, 10, 40, 0, 0), result);
    }

    [Fact]
    public void FindNextOrSame_MovesToNextAllowedSecond()
    {
        var definition = CreateDefinition(
            hours: [10],
            minutes: [20],
            seconds: [0, 15, 30, 45],
            milliseconds: [0]);

        DateTime value = new(2026, 9, 8, 10, 20, 16, 0);

        DateTime result = Searching.ScheduleSearcher.FindNextOrSame(value, definition);

        Assert.Equal(new DateTime(2026, 9, 8, 10, 20, 30, 0), result);
    }

    [Fact]
    public void FindNextOrSame_MovesToNextAllowedDay()
    {
        var definition = CreateDefinition(
            days: [8, 15],
            weekdays: [2],
            hours: [10],
            minutes: [0],
            seconds: [0],
            milliseconds: [0]);

        DateTime value = new(2026, 9, 8, 11, 0, 0, 0);

        DateTime result = Searching.ScheduleSearcher.FindNextOrSame(value, definition);

        Assert.Equal(new DateTime(2026, 9, 15, 10, 0, 0, 0), result);
    }

    [Fact]
    public void FindNextOrSame_MovesToNextAllowedMonth()
    {
        var definition = CreateDefinition(
            months: [9, 12],
            days: [1],
            weekdays: [2],
            hours: [10],
            minutes: [0],
            seconds: [0],
            milliseconds: [0]);

        DateTime value = new(2026, 9, 2, 10, 0, 0, 0);

        DateTime result = Searching.ScheduleSearcher.FindNextOrSame(value, definition);

        Assert.Equal(new DateTime(2026, 12, 1, 10, 0, 0, 0), result);
    }

    [Fact]
    public void FindNextOrSame_MovesToNextAllowedYear()
    {
        var definition = CreateDefinition(
            years: [2026, 2028],
            months: [2],
            days: [1],
            weekdays: [2],
            hours: [10],
            minutes: [0],
            seconds: [0],
            milliseconds: [0]);

        DateTime value = new(2026, 9, 1, 10, 0, 0, 0);

        DateTime result = Searching.ScheduleSearcher.FindNextOrSame(value, definition);

        Assert.Equal(new DateTime(2028, 2, 1, 10, 0, 0, 0), result);
    }

    [Fact]
    public void FindNextOrSame_HandlesLastDayOfMonth()
    {
        var definition = CreateDefinition(
            months: [2],
            days: [32],
            weekdays: [6],
            hours: [23],
            minutes: [59],
            seconds: [59],
            milliseconds: [999]);

        DateTime value = new(2026, 2, 1, 0, 0, 0, 0);

        DateTime result = Searching.ScheduleSearcher.FindNextOrSame(value, definition);

        Assert.Equal(new DateTime(2026, 2, 28, 23, 59, 59, 999), result);
    }

    [Fact]
    public void FindNextOrSame_HandlesLeapYearLastDay()
    {
        var definition = CreateDefinition(
            years: [2028],
            months: [2],
            days: [32],
            weekdays: [2],
            hours: [10],
            minutes: [0],
            seconds: [0],
            milliseconds: [0]);

        DateTime value = new(2028, 2, 1, 0, 0, 0, 0);

        DateTime result = Searching.ScheduleSearcher.FindNextOrSame(value, definition);

        Assert.Equal(new DateTime(2028, 2, 29, 10, 0, 0, 0), result);
    }

    [Fact]
    public void FindNextOrSame_RequiresBothDayAndWeekday()
    {
        var definition = CreateDefinition(
            days: [8, 9],
            weekdays: [3],
            hours: [10],
            minutes: [0],
            seconds: [0],
            milliseconds: [0]);

        DateTime value = new(2026, 9, 8, 0, 0, 0, 0);

        DateTime result = Searching.ScheduleSearcher.FindNextOrSame(value, definition);

        Assert.Equal(new DateTime(2026, 9, 9, 10, 0, 0, 0), result);
    }

    [Fact]
    public void FindNextOrSame_HandlesFractionalMilliseconds()
    {
        var definition = CreateDefinition(
            hours: [10],
            minutes: [0, 1],
            seconds: [0],
            milliseconds: [500]);

        DateTime value = new DateTime(2026, 9, 8, 10, 0, 0, 500).AddTicks(1);

        DateTime result = Searching.ScheduleSearcher.FindNextOrSame(value, definition);

        Assert.Equal(new DateTime(2026, 9, 8, 10, 1, 0, 500), result);
    }

    [Fact]
    public void FindNextOrSame_Throws_WhenThereIsNoNextYear()
    {
        var definition = CreateDefinition(
            years: [2026],
            months: [1],
            days: [1],
            weekdays: [4],
            hours: [0],
            minutes: [0],
            seconds: [0],
            milliseconds: [0]);

        DateTime value = new(2027, 1, 1);

        Assert.Throws<InvalidOperationException>(
            () => Searching.ScheduleSearcher.FindNextOrSame(value, definition));
    }

    [Fact]
    public void FindPreviousOrSame_MovesToPreviousHour()
    {
        var definition = CreateDefinition(
            hours: [8, 12, 18],
            minutes: [59],
            seconds: [59],
            milliseconds: [999]);

        DateTime value = new(2026, 9, 8, 14, 0, 0, 0);

        DateTime result = Searching.ScheduleSearcher.FindPreviousOrSame(value, definition);

        Assert.Equal(new DateTime(2026, 9, 8, 12, 59, 59, 999), result);
    }

    [Fact]
    public void FindPreviousOrSame_MovesToPreviousDay()
    {
        var definition = CreateDefinition(
            days: [8, 15],
            weekdays: [2],
            hours: [10],
            minutes: [0],
            seconds: [0],
            milliseconds: [0]);

        DateTime value = new(2026, 9, 15, 11, 0, 0, 0);

        DateTime result = Searching.ScheduleSearcher.FindPreviousOrSame(value, definition);

        Assert.Equal(new DateTime(2026, 9, 15, 10, 0, 0, 0), result);
    }

    [Fact]
    public void FindPreviousOrSame_HandlesLastDayOfMonth()
    {
        var definition = CreateDefinition(
            days: [32],
            weekdays: [0, 1, 2, 3, 4, 5, 6],
            hours: [10],
            minutes: [0],
            seconds: [0],
            milliseconds: [0]);

        DateTime value = new(2026, 9, 30, 23, 0, 0, 0);

        DateTime result = Searching.ScheduleSearcher.FindPreviousOrSame(value, definition);

        Assert.Equal(new DateTime(2026, 9, 30, 10, 0, 0, 0), result);
    }

    [Fact]
    public void FindPreviousOrSame_Throws_WhenThereIsNoPreviousYear()
    {
        var definition = CreateDefinition(
            years: [2026],
            months: [1],
            days: [1],
            weekdays: [4],
            hours: [0],
            minutes: [0],
            seconds: [0],
            milliseconds: [0]);

        DateTime value = new(2025, 12, 31);

        Assert.Throws<InvalidOperationException>(
            () => Searching.ScheduleSearcher.FindPreviousOrSame(value, definition));
    }

    private static ScheduleDefinition CreateDefinition(
        int[]? years = null,
        int[]? months = null,
        int[]? days = null,
        int[]? weekdays = null,
        int[]? hours = null,
        int[]? minutes = null,
        int[]? seconds = null,
        int[]? milliseconds = null)
    {
        return new ScheduleDefinition(
            new ScheduleField(2000, 2100, years ?? [2026]),
            new ScheduleField(1, 12, months ?? [9]),
            new ScheduleField(1, 32, days ?? [8]),
            new ScheduleField(0, 6, weekdays ?? [2]),
            new ScheduleField(0, 23, hours ?? [10]),
            new ScheduleField(0, 59, minutes ?? [0]),
            new ScheduleField(0, 59, seconds ?? [0]),
            new ScheduleField(0, 999, milliseconds ?? [0]));
    }
}
