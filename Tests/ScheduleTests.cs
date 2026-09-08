namespace Complex_Systems.Tests;

public class ScheduleTests
{
    [Fact]
    public void Constructor_WithDefaultSchedule_CreatesSchedule()
    {
        var schedule = new Schedule();

        DateTime value = new DateTime(2026, 9, 8, 10, 20, 30, 500);

        DateTime result = schedule.NearestEvent(value);

        Assert.Equal(value, result);
    }

    [Fact]
    public void Constructor_WithScheduleString_CreatesSchedule()
    {
        var schedule = new Schedule("2026.09.08 10:20:30.500");

        DateTime value = new DateTime(2026, 9, 8, 10, 20, 0, 0);

        DateTime result = schedule.NearestEvent(value);

        Assert.Equal(
            new DateTime(2026, 9, 8, 10, 20, 30, 500),
            result);
    }

    [Fact]
    public void NearestEvent_ReturnsSameTime_WhenTimeMatchesSchedule()
    {
        var schedule = new Schedule("2026.09.08 10:20:30.500");

        DateTime value = new DateTime(2026, 9, 8, 10, 20, 30, 500);

        DateTime result = schedule.NearestEvent(value);

        Assert.Equal(value, result);
    }

    [Fact]
    public void NearestEvent_ReturnsNextEvent_WhenTimeDoesNotMatchSchedule()
    {
        var schedule = new Schedule("2026.09.08 10:20:30.500");

        DateTime value = new DateTime(2026, 9, 8, 10, 20, 0, 0);

        DateTime result = schedule.NearestEvent(value);

        Assert.Equal(
            new DateTime(2026, 9, 8, 10, 20, 30, 500),
            result);
    }

    [Fact]
    public void NearestPrevEvent_ReturnsSameTime_WhenTimeMatchesSchedule()
    {
        var schedule = new Schedule("2026.09.08 10:20:30.500");

        DateTime value = new DateTime(2026, 9, 8, 10, 20, 30, 500);

        DateTime result = schedule.NearestPrevEvent(value);

        Assert.Equal(value, result);
    }

    [Fact]
    public void NearestPrevEvent_ReturnsPreviousEvent_WhenTimeDoesNotMatchSchedule()
    {
        var schedule = new Schedule("2026.09.08 10:20:30.500");

        DateTime value = new DateTime(2026, 9, 8, 10, 20, 40, 0);

        DateTime result = schedule.NearestPrevEvent(value);

        Assert.Equal(
            new DateTime(2026, 9, 8, 10, 20, 30, 500),
            result);
    }

    [Fact]
    public void NextEvent_DoesNotReturnSameTime()
    {
        var schedule = new Schedule("2026.09.08 10:20:30.500");

        DateTime value = new DateTime(2026, 9, 8, 10, 20, 30, 500);

        Assert.Throws<InvalidOperationException>(
            () => schedule.NextEvent(value));
    }

    [Fact]
    public void PrevEvent_DoesNotReturnSameTime()
    {
        var schedule = new Schedule("2026.09.08 10:20:30.500");

        DateTime value = new DateTime(2026, 9, 8, 10, 20, 30, 500);

        Assert.Throws<InvalidOperationException>(
            () => schedule.PrevEvent(value));
    }

    [Fact]
    public void NextEvent_ReturnsNextEvent()
    {
        var schedule = new Schedule("2026.09.08 10:20:30.500");

        DateTime value = new DateTime(2026, 9, 8, 10, 20, 0, 0);

        DateTime result = schedule.NextEvent(value);

        Assert.Equal(
            new DateTime(2026, 9, 8, 10, 20, 30, 500),
            result);
    }

    [Fact]
    public void PrevEvent_ReturnsPreviousEvent()
    {
        var schedule = new Schedule("2026.09.08 10:20:30.500");

        DateTime value = new DateTime(2026, 9, 8, 10, 20, 40, 0);

        DateTime result = schedule.PrevEvent(value);

        Assert.Equal(
            new DateTime(2026, 9, 8, 10, 20, 30, 500),
            result);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenScheduleIsInvalid()
    {
        Assert.Throws<ArgumentException>(
            () => new Schedule("invalid schedule"));
    }
}
