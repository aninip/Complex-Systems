using Complex_Systems.Model;

namespace Complex_Systems.Searching;

/// <summary>
/// Выполняет поиск ближайших допустимых моментов времени по расписанию
/// </summary>
internal static class ScheduleSearcher
{
    /// <summary>
    /// Находит первый момент времени, строго больший value,
    /// который соответствует расписанию.
    /// </summary>
    internal static DateTime FindNext(DateTime value, ScheduleDefinition definition)
    {
        DateTime start = RoundUpToMilliseconds(value);

        if (start == value)
        {
            start = start.AddMilliseconds(1);
        }

        return FindNextOrSame(start, definition);
    }

    /// <summary>
    /// Находит первый момент времени, меньший value,
    /// который соответствует расписанию.
    /// </summary>
    internal static DateTime FindPrevious(DateTime value, ScheduleDefinition definition)
    {
        DateTime start = TruncateToMilliseconds(value);

        if (start == value)
        {
            start = start.AddMilliseconds(-1);
        }

        return FindPreviousOrSame(start, definition);
    }

    /// <summary>
    /// Находит первый момент времени, больший или равный value,
    /// который соответствует расписанию.
    /// </summary>
    internal static DateTime FindNextOrSame(DateTime value, ScheduleDefinition definition)
    {
        DateTime current = RoundUpToMilliseconds(value);


        while (true) //механизм возврата к началу алгоритма после изменения старшего компонента
        {
            // 1. Ищем ближайший разрешённый год, больший или равный текущему
            int? year = definition.Years.GetNextOrSame(current.Year);

            // Если подходящего года нет, события в расписании дальше не существует
            if (year is null)
                throw new InvalidOperationException("There is no next event in the definition.");

            // Если перешли на новый год, начинаем поиск с его начала.
            if (year.Value != current.Year)
            {
                current = CreateDateTime(current, year.Value, 1, 1, 0, 0, 0, 0);
                continue;
            }


            // 2. Ищем ближайший разрешённый месяц, больший или равный текущему
            int? month = definition.Months.GetNextOrSame(current.Month);

            // Если в текущем году подходящего месяца больше нет, переходим к следующему разрешённому году.
            if (month is null)
            {
                current = MoveToNextAllowedYear(current, definition);
                continue;
            }

            // Если перешли на новый месяц, начинаем поиск с его начала.
            if (month.Value != current.Month)
            {
                current = CreateDateTime(current, current.Year, month.Value, 1, 0, 0, 0, 0);
                continue;
            }


            // 3. Ищем ближайший разрешённый день, больший или равный текущему.
            int? day = FindNextAllowedDay(current.Year, current.Month, current.Day, definition);

            // Если в текущем месяце подходящего дня больше нет,
            // переходим к следующему разрешённому месяцу или году.
            if (day is null)
            {
                current = MoveToNextAllowedMonth(current, definition);
                continue;
            }

            // Если перешли на новый день, начинаем поиск с его начала.
            if (day.Value != current.Day)
            {
                current = CreateDateTime(current, current.Year, current.Month, day.Value, 0, 0, 0, 0);
                continue;
            }


            // 4. Ищем ближайший разрешённый час, больший или равный текущему.
            int? hour = definition.Hours.GetNextOrSame(current.Hour);

            // Если в текущем дне подходящего часа больше нет,
            // переходим к следующему разрешённому дню.
            if (hour is null)
            {
                current = MoveToNextAllowedDay(current, definition);
                continue;
            }

            // Если перешли на новый час, начинаем поиск с его начала.
            if (hour.Value != current.Hour)
            {
                current = CreateDateTime(current, current.Year, current.Month, current.Day, hour.Value, 0, 0, 0);
                continue;
            }


            // 5. Ищем ближайшую разрешённую минуту, большую или равную текущей.
            int? minute = definition.Minutes.GetNextOrSame(current.Minute);

            // Если в текущем часе подходящей минуты больше нет,
            // переходим к следующему разрешённому часу.
            if (minute is null)
            {
                current = MoveToNextAllowedHour(current, definition);
                continue;
            }

            // Если перешли на новую минуту, начинаем поиск с её начала.
            if (minute.Value != current.Minute)
            {
                current = CreateDateTime(current, current.Year, current.Month, current.Day, current.Hour, minute.Value, 0, 0);
                continue;
            }


            // 6. Ищем ближайшую разрешённую секунду, большую или равную текущей.
            int? second = definition.Seconds.GetNextOrSame(current.Second);

            // Если в текущей минуте подходящей секунды больше нет,
            // переходим к следующей разрешённой минуте.
            if (second is null)
            {
                current = MoveToNextAllowedMinute(current, definition);
                continue;
            }

            // Если перешли на новую секунду, начинаем поиск с её начала.
            if (second.Value != current.Second)
            {
                current = CreateDateTime(current, current.Year, current.Month, current.Day, current.Hour, current.Minute, second.Value, 0);
                continue;
            }


            // 7. Ищем ближайшую разрешённую миллисекунду, большую или равную текущей.
            int? millisecond = definition.Milliseconds.GetNextOrSame(current.Millisecond);

            // Если в текущей секунде подходящей миллисекунды больше нет,
            // переходим к следующей разрешённой секунде.
            if (millisecond is null)
            {
                current = MoveToNextAllowedSecond(current, definition);
                continue;
            }

            return CreateDateTime(current, current.Year, current.Month, current.Day, current.Hour, current.Minute, current.Second, millisecond.Value);

        }
    }

    /// <summary>
    /// Находит первый момент времени, меньший или равный value,
    /// который соответствует расписанию.
    /// </summary>
    internal static DateTime FindPreviousOrSame(DateTime value, ScheduleDefinition definition)
    {
        DateTime start = TruncateToMilliseconds(value);

        while (true)
        {
            int? year = definition.Years.GetPreviousOrSame(start.Year);

            if (year is null)
                throw new InvalidOperationException("There is no previous event in the definition.");

            if (year.Value != start.Year)
            {
                start = CreateDateTime(start, year.Value, 12, 31, 23, 59, 59, 999);
            }

            int? month = definition.Months.GetPreviousOrSame(start.Month);

            if (month is null)
            {
                start = MoveToPreviousAllowedYear(start, definition);
                continue;
            }

            if (month.Value != start.Month)
            {
                start = CreateDateTime(start, start.Year, month.Value, DateTime.DaysInMonth(start.Year, month.Value), 23, 59, 59, 999);
            }

            int? day = FindPreviousAllowedDay(start.Year, start.Month, start.Day, definition);

            if (day is null)
            {
                start = MoveToPreviousAllowedMonth(start, definition);
                continue;
            }

            if (day.Value != start.Day)
            {
                start = CreateDateTime(start, start.Year, start.Month, day.Value, 23, 59, 59, 999);
            }

            int? hour = definition.Hours.GetPreviousOrSame(start.Hour);

            if (hour is null)
            {
                start = MoveToPreviousAllowedDay(start, definition);
                continue;
            }

            if (hour.Value != start.Hour)
            {
                start = CreateDateTime(start, start.Year, start.Month, start.Day, hour.Value, 59, 59, 999);
            }

            int? minute = definition.Minutes.GetPreviousOrSame(start.Minute);

            if (minute is null)
            {
                start = MoveToPreviousAllowedHour(start, definition);
                continue;
            }

            if (minute.Value != start.Minute)
            {
                start = CreateDateTime(start, start.Year, start.Month, start.Day, start.Hour, minute.Value, 59, 999);
            }

            int? second = definition.Seconds.GetPreviousOrSame(start.Second);

            if (second is null)
            {
                start = MoveToPreviousAllowedMinute(start, definition);
                continue;
            }

            if (second.Value != start.Second)
            {
                start = CreateDateTime(start, start.Year, start.Month, start.Day, start.Hour, start.Minute, second.Value, 999);
            }

            int? millisecond = definition.Milliseconds.GetPreviousOrSame(start.Millisecond);

            if (millisecond is null)
            {
                start = MoveToPreviousAllowedSecond(start, definition);
                continue;
            }

            return CreateDateTime(start, start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second, millisecond.Value);
        }
    }


    #region Next-helpers
    private static int? FindNextAllowedDay(int year, int month, int startDay, ScheduleDefinition definition)
    {
        //количество дней в конкретном месяце с учётом високосного года
        int daysInMonth = DateTime.DaysInMonth(year, month);

        //TODO:  В будущем заменить перебор дней на поиск через ScheduleField.

        for (int currentDay = startDay; currentDay <= daysInMonth; currentDay++)
        {
            DateTime date = new(year, month, currentDay);

            if (IsDateAllowed(date, definition))
                return currentDay;
        }

        return null;
    }
    private static DateTime MoveToNextAllowedYear(DateTime value, ScheduleDefinition definition)
    {
        int? year = definition.Years.GetNext(value.Year);

        if (year is null)
            throw new InvalidOperationException("There is no next event in the definition.");

        return CreateDateTime(value, year.Value, 1, 1, 0, 0, 0);
    }
    private static DateTime MoveToNextAllowedMonth(DateTime value, ScheduleDefinition definition)
    {
        int? month = definition.Months.GetNext(value.Month);

        if (month is not null)
        {
            return CreateDateTime(value, value.Year, month.Value, 1, 0, 0, 0);
        }

        return MoveToNextAllowedYear(value, definition);
    }
    private static DateTime MoveToNextAllowedDay(DateTime value, ScheduleDefinition definition)
    {
        DateTime nextDay = value.AddDays(1);

        int? day = FindNextAllowedDay(nextDay.Year, nextDay.Month, nextDay.Day, definition);

        if (day is not null)
        {
            return CreateDateTime(nextDay, nextDay.Year, nextDay.Month, day.Value, 0, 0, 0, 0);
        }

        // В текущем месяце больше допустимых дней нет.
        // Переходим к следующему допустимому месяцу.
        return MoveToNextAllowedMonth(nextDay, definition);
    }

    private static DateTime MoveToNextAllowedHour(DateTime value, ScheduleDefinition definition)
    {
        int? hour = definition.Hours.GetNext(value.Hour);

        if (hour is not null)
        {
            return CreateDateTime(value, value.Year, value.Month, value.Day, hour.Value, 0, 0);
        }

        return MoveToNextAllowedDay(value, definition);
    }

    private static DateTime MoveToNextAllowedMinute(DateTime value, ScheduleDefinition definition)
    {
        int? minute = definition.Minutes.GetNext(value.Minute);

        if (minute is not null)
        {
            return CreateDateTime(value, value.Year, value.Month, value.Day, value.Hour, minute.Value, 0);
        }

        return MoveToNextAllowedHour(value, definition);
    }

    private static DateTime MoveToNextAllowedSecond(DateTime value, ScheduleDefinition definition)
    {
        int? second = definition.Seconds.GetNext(value.Second);

        if (second is not null)
        {
            return CreateDateTime(value, value.Year, value.Month, value.Day, value.Hour, value.Minute, second.Value, 0);
        }

        return MoveToNextAllowedMinute(value, definition);
    }
    #endregion

    #region Previous-helpers
    private static int? FindPreviousAllowedDay(int year, int month, int startDay, ScheduleDefinition definition)
    {
        int daysInMonth = DateTime.DaysInMonth(year, month);
        int firstDay = Math.Min(startDay, daysInMonth);

        for (int day = firstDay; day >= 1; day--)
        {
            DateTime date = new(year, month, day);

            if (IsDateAllowed(date, definition))
                return day;
        }

        return null;
    }

    private static DateTime MoveToPreviousAllowedYear(DateTime value, ScheduleDefinition definition)
    {
        int? year = definition.Years.GetPrevious(value.Year);

        if (year is null)
            throw new InvalidOperationException("There is no previous event in the definition.");

        return CreateDateTime(value, year.Value, 12, 31, 23, 59, 59, 999);
    }

    private static DateTime MoveToPreviousAllowedMonth(DateTime value, ScheduleDefinition definition)
    {
        int? month = definition.Months.GetPrevious(value.Month);

        if (month is not null)
        {
            return CreateDateTime(value, value.Year, month.Value, DateTime.DaysInMonth(value.Year, month.Value), 23, 59, 59, 999);
        }

        return MoveToPreviousAllowedYear(value, definition);
    }

    private static DateTime MoveToPreviousAllowedDay(DateTime value, ScheduleDefinition definition)
    {
        DateTime previousDay = value.AddDays(-1);

        int? day = FindPreviousAllowedDay(previousDay.Year, previousDay.Month, previousDay.Day, definition);

        if (day is not null)
        {
            return CreateDateTime(previousDay, previousDay.Year, previousDay.Month, day.Value, 23, 59, 59, 999);
        }

        return MoveToPreviousAllowedMonth(previousDay, definition);
    }

    private static DateTime MoveToPreviousAllowedHour(DateTime value, ScheduleDefinition definition)
    {
        int? hour = definition.Hours.GetPrevious(value.Hour);

        if (hour is not null)
        {
            return CreateDateTime(value, value.Year, value.Month, value.Day, hour.Value, 59, 59, 999);
        }

        return MoveToPreviousAllowedDay(value, definition);
    }

    private static DateTime MoveToPreviousAllowedMinute(DateTime value, ScheduleDefinition definition)
    {
        int? minute = definition.Minutes.GetPrevious(value.Minute);

        if (minute is not null)
        {
            return CreateDateTime(value, value.Year, value.Month, value.Day, value.Hour, minute.Value, 59, 999);
        }

        return MoveToPreviousAllowedHour(value, definition);
    }

    private static DateTime MoveToPreviousAllowedSecond(DateTime value, ScheduleDefinition definition)
    {
        int? second = definition.Seconds.GetPrevious(value.Second);

        if (second is not null)
        {
            return CreateDateTime(value, value.Year, value.Month, value.Day, value.Hour, value.Minute, second.Value, 999);
        }

        return MoveToPreviousAllowedMinute(value, definition);
    }
    #endregion

    #region Date validation
    private static bool IsDateAllowed(DateTime date, ScheduleDefinition definition)
    {
        return IsDayAllowed(date, definition)
            && definition.Weekdays.Contains((int)date.DayOfWeek);
    }

    private static bool IsDayAllowed(DateTime date, ScheduleDefinition definition)
    {
        if (definition.Days.Contains(date.Day)) // либо currentDay разрешён
            return true;

        return definition.Days.Contains(32) // либо currentDay последний в месяце, и последний день разрешён
            && date.Day == DateTime.DaysInMonth(date.Year, date.Month);
    }
    #endregion

    #region DateTime precision
    private static DateTime RoundUpToMilliseconds(DateTime value)
    {
        long remainder = value.Ticks % TimeSpan.TicksPerMillisecond;

        if (remainder == 0)
            return value;

        long ticks = value.Ticks + TimeSpan.TicksPerMillisecond - remainder;

        return new DateTime(ticks, value.Kind);
    }

    private static DateTime TruncateToMilliseconds(DateTime value)
    {
        long ticks = value.Ticks - value.Ticks % TimeSpan.TicksPerMillisecond;

        return new DateTime(ticks, value.Kind);
    }
    #endregion

    #region DateTime creation
    private static DateTime CreateDateTime(DateTime source, int year, int month, int day, int hour, int minute, int second, int millisecond = 0)
    {
        return new DateTime(year, month, day, hour, minute, second, millisecond, source.Kind);
    }
    #endregion
}