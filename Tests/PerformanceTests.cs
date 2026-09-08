namespace Complex_Systems.Tests;

public class PerformanceTests
{
    private const string LogFilePath = "TestResults/PerformanceTests.log";

    [Fact]
    public void FindNext_DefaultSchedule_ShouldBeFast()
    {
        var schedule = new Schedule();
        DateTime value = new DateTime(2026, 9, 8, 10, 20, 30, 500);
        const int iterations = 100_000;

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++)
        {
            schedule.NextEvent(value);
        }

        stopwatch.Stop();

        WriteLog(
            nameof(FindNext_DefaultSchedule_ShouldBeFast),
            $"Iterations: {iterations}",
            $"Elapsed: {stopwatch.Elapsed}",
            $"Average: {stopwatch.Elapsed.TotalMicroseconds / iterations:F3} μs");

        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void FindNext_RareSchedule_ShouldBeFast()
    {
        var schedule = new Schedule("2100.12.32 23:59:59.999");
        DateTime value = new DateTime(2026, 1, 1, 0, 0, 0, 0);
        const int iterations = 10_000;

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++)
        {
            schedule.NearestEvent(value);
        }

        stopwatch.Stop();

        WriteLog(
            nameof(FindNext_RareSchedule_ShouldBeFast),
            $"Iterations: {iterations}",
            $"Elapsed: {stopwatch.Elapsed}",
            $"Average: {stopwatch.Elapsed.TotalMicroseconds / iterations:F3} μs");

        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void FindNext_ShouldNotAllocateLargeAmountsOfMemory()
    {
        var schedule = new Schedule();
        DateTime value = new DateTime(2026, 9, 8, 10, 20, 30, 500);
        const int iterations = 100_000;

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long allocatedBefore = GC.GetAllocatedBytesForCurrentThread();

        for (int i = 0; i < iterations; i++)
        {
            schedule.NextEvent(value);
        }

        long allocatedAfter = GC.GetAllocatedBytesForCurrentThread();
        long allocatedBytes = allocatedAfter - allocatedBefore;

        WriteLog(
            nameof(FindNext_ShouldNotAllocateLargeAmountsOfMemory),
            $"Iterations: {iterations}",
            $"Allocated: {allocatedBytes:N0} bytes",
            $"Per call: {(double)allocatedBytes / iterations:F2} bytes");

        Assert.True(allocatedBytes < 10_000_000);
    }

    private static void WriteLog(string testName, params string[] lines)
    {
        string directory = Path.GetDirectoryName(LogFilePath)!;
        Directory.CreateDirectory(directory);

        using var writer = new StreamWriter(LogFilePath, append: true);

        writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {testName}");

        foreach (string line in lines)
            writer.WriteLine(line);

        writer.WriteLine();
    }
}