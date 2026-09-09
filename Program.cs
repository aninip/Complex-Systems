using System.Diagnostics;

namespace Complex_Systems;

public static class Program
{
    public static void Main()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("=== Schedule test ===");
            Console.WriteLine();

            string scheduleString = "*.9.*/2 1-5 10:00:00.000";
            DateTime time = DateTime.Now;

            RunSchedule(scheduleString, time);

            Console.WriteLine();
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1 - Ввести своё расписание и дату");
            Console.WriteLine("0 - Выход");
            Console.Write("Ваш выбор: ");

            string? choice = Console.ReadLine();

            if (choice == "0")
                return;

            if (choice != "1")
                continue;

            Console.WriteLine();
            Console.Write("Введите расписание: ");
            scheduleString = Console.ReadLine() ?? string.Empty;

            Console.Write("Введите дату и время (например, 2026-09-05 12:00:00.000): ");

            if (!DateTime.TryParse(Console.ReadLine(), out time))
            {
                Console.WriteLine("Некорректная дата.");
                Console.WriteLine("Нажмите Enter для продолжения...");
                Console.ReadLine();

                continue;
            }

            Console.WriteLine();
            RunSchedule(scheduleString, time);

            Console.WriteLine();
            Console.WriteLine("Нажмите Enter для возврата в меню...");
            Console.ReadLine();
        }
    }

    private static void RunSchedule(string scheduleString, DateTime time)
    {
        try
        {
            var schedule = new Schedule(scheduleString);

            Console.WriteLine($"Расписание: {scheduleString}");
            Console.WriteLine($"Исходная дата: {time:yyyy-MM-dd HH:mm:ss.fff}");
            Console.WriteLine();

            Measure("NearestEvent", () => schedule.NearestEvent(time));
            Measure("NearestPrevEvent", () => schedule.NearestPrevEvent(time));
            Measure("NextEvent", () => schedule.NextEvent(time));
            Measure("PrevEvent", () => schedule.PrevEvent(time));
        }
        catch (Exception exception)
        {
            Console.WriteLine();
            Console.WriteLine($"Ошибка: {exception.Message}");
        }
    }

    private static void Measure(string operationName, Func<DateTime> operation)
    {
        var stopwatch = Stopwatch.StartNew();

        DateTime result = operation();

        stopwatch.Stop();

        Console.WriteLine(
            $"{operationName,-18}: {result:yyyy-MM-dd HH:mm:ss.fff} | " +
            $"Время: {stopwatch.Elapsed.TotalMicroseconds:F3} мкс");
    }


}
