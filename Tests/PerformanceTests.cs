namespace Complex_Systems.Tests;

public class PerformanceTests
{
    private const string LogFilePath = "TestResults/PerformanceTests.log";

    [Fact]
    public void NextEvent_DefaultSchedule_ShouldBeFast()
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
            nameof(NextEvent_DefaultSchedule_ShouldBeFast),
            $"Iterations: {iterations}",
            $"Elapsed: {stopwatch.Elapsed}",
            $"Average: {stopwatch.Elapsed.TotalMicroseconds / iterations:F3} μs");

        Assert.True(stopwatch.Elapsed < TimeSpan.FromMilliseconds(500));
    }

    [Fact]
    public void NearestEvent_RareSchedule_ShouldBeFast()
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
            nameof(NearestEvent_RareSchedule_ShouldBeFast),
            $"Iterations: {iterations}",
            $"Elapsed: {stopwatch.Elapsed}",
            $"Average: {stopwatch.Elapsed.TotalMicroseconds / iterations:F3} μs");

        Assert.True(stopwatch.Elapsed < TimeSpan.FromMilliseconds(500));
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
// Результаты тестов: 
//[2026 - 09 - 08 21:59:09] FindNext_DefaultSchedule_ShouldBeFast
//Iterations: 100000
//Elapsed: 00:00:00.0280963
//Average: 0,281 μs

//[2026 - 09 - 08 21:59:09] FindNext_ShouldNotAllocateLargeAmountsOfMemory
//Iterations: 100000
//Allocated: 0 bytes
//Per call: 0,00 bytes

//[2026 - 09 - 08 21:59:09] FindNext_RareSchedule_ShouldBeFast
//Iterations: 10000
//Elapsed: 00:00:00.0251697
//Average: 2,517 μs

//[2026 - 09 - 08 22:01:41] FindNext_DefaultSchedule_ShouldBeFast
//Iterations: 100000
//Elapsed: 00:00:00.0266549
//Average: 0,267 μs

