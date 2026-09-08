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
        DateTime start = TruncateToMilliseconds(value);

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
        DateTime start = RoundUpToMilliseconds(value);

        int year = start.Year;
        int month = start.Month;
        int day = start.Day;
        int hour = start.Hour;
        int minute = start.Minute;
        int second = start.Second;
        int millisecond = start.Millisecond;

        while (true) //механизм возврата к началу алгоритма после изменения старшего компонента
        {
            // 1. Ищем ближайший разрешённый год, больший или равный текущему
            int? nextYear = definition.Years.GetNextOrSame(year);

            // Если подходящего года нет, события в расписании дальше не существует
            if (nextYear is null)
                throw new InvalidOperationException("There is no next event in the definition.");

            // Если год изменился, начинаем поиск с начала нового года.
            // если вернулся тот же самый год, то не перезаписываем локальный набор компонент
            if (nextYear.Value != year)
            {
                //перезаписываем год
                year = nextYear.Value;
                month = 1;
                day = 1;
                hour = 0;
                minute = 0;
                second = 0;
                millisecond = 0;

                continue; //Начнём алгоритм заново уже для нового года.
            }


            // 2. Ищем ближайший разрешённый месяц, больший или равный текущему
            int? nextMonth = definition.Months.GetNextOrSame(month);

            // этот if может сработать, только если год не менялся выше 
            if (nextMonth is null)
            {
                // попробуем обработать следующий год
                int? followingYear = definition.Years.GetNext(year);

                if (followingYear is null)
                    throw new InvalidOperationException("There is no next event in the definition.");

                //перезаписываем год
                year = followingYear.Value;
                month = 1;
                day = 1;
                hour = 0;
                minute = 0;
                second = 0;
                millisecond = 0;

                continue; //Начнём алгоритм заново уже для нового года.
            }

            //перезаписываем месяц
            if (nextMonth.Value != month)
            {
                month = nextMonth.Value;
                day = 1;
                hour = 0;
                minute = 0;
                second = 0;
                millisecond = 0;

                continue; //Начнём алгоритм заново уже для нового месяца
            }


            // 3. Ищем ближайший разрешённый день, больший или равный текущему
            int? nextDay = FindNextAllowedDay(year, month, day, definition);

            // Если в текущем месяце подходящего дня больше нет, переходим к следующему разрешённому месяцу
            if (nextDay is null)
            {
                int? followingMonth = definition.Months.GetNext(month);

                if (followingMonth is not null)
                {
                    month = followingMonth.Value;
                    day = 1;
                    hour = 0;
                    minute = 0;
                    second = 0;
                    millisecond = 0;

                    continue; // Начинаем алгоритм заново уже для нового месяца.
                }

                int? followingYear = definition.Years.GetNext(year);

                if (followingYear is null)
                    throw new InvalidOperationException("There is no next event in the definition.");

                year = followingYear.Value;
                month = 1;
                day = 1;
                hour = 0;
                minute = 0;
                second = 0;
                millisecond = 0;

                continue;
            }

            //перезаписываем день
            if (nextDay.Value != day)
            {
                day = nextDay.Value;
                hour = 0;
                minute = 0;
                second = 0;
                millisecond = 0;

                continue; //начинаем поиск с начала нового дня
            }


            // 4. Ищем ближайший разрешённый час, больший или равный текущему
            int? nextHour = definition.Hours.GetNextOrSame(hour);

            // Если в текущем дне подходящего часа больше нет,
            // переходим к следующему допустимому  дню
            if (nextHour is null)
            {
                int? followingDay = FindNextAllowedDay(year, month, day + 1, definition);

                if (followingDay is not null)
                {
                    day = followingDay.Value;
                    hour = 0;
                    minute = 0;
                    second = 0;
                    millisecond = 0;

                    continue;
                }

                // Если в текущем месяце больше нет допустимых дней,
                // переходим к следующему разрешённому месяцу
                int? followingMonth = definition.Months.GetNext(month);

                if (followingMonth is not null)
                {
                    month = followingMonth.Value;
                    day = 1;
                    hour = 0;
                    minute = 0;
                    second = 0;
                    millisecond = 0;

                    continue;
                }

                // Если следующего месяца нет, переходим к следующему разрешённому году
                int? followingYear = definition.Years.GetNext(year);

                if (followingYear is null)
                    throw new InvalidOperationException("There is no next event in the definition.");

                year = followingYear.Value;
                month = 1;
                day = 1;
                hour = 0;
                minute = 0;
                second = 0;
                millisecond = 0;

                continue;
            }

            // Перезаписываем час
            if (nextHour.Value != hour)
            {
                hour = nextHour.Value;
                minute = 0;
                second = 0;
                millisecond = 0;

                continue; // Начинаем поиск заново уже с начала нового часа.
            }


            // 5. Ищем ближайшую разрешённую минуту, большую или равную текущей
            int? nextMinute = definition.Minutes.GetNextOrSame(minute);

            // Если в текущем часе подходящей минуты больше нет, переходим к следующему допустимому часу
            if (nextMinute is null)
            {
                int? followingHour = definition.Hours.GetNext(hour);

                if (followingHour is not null)
                {
                    hour = followingHour.Value;
                    minute = 0;
                    second = 0;
                    millisecond = 0;

                    continue;
                }

                // Если следующего часа нет, текущий день считаем исчерпанным и переходим к следующему допустимому дню.
                int? followingDay = FindNextAllowedDay(year, month, day + 1, definition);

                if (followingDay is not null)
                {
                    day = followingDay.Value;
                    hour = 0;
                    minute = 0;
                    second = 0;
                    millisecond = 0;

                    continue; // Начинаем алгоритм заново уже с нового дня.
                }

                // Если в текущем месяце больше допустимых дней нет, переходим к следующему разрешённому месяцу.
                int? followingMonth = definition.Months.GetNext(month);

                if (followingMonth is not null)
                {
                    month = followingMonth.Value;
                    day = 1;
                    hour = 0;
                    minute = 0;
                    second = 0;
                    millisecond = 0;

                    continue; // Начинаем алгоритм заново уже с нового месяца.
                }

                // Если следующего месяца нет, переходим к следующему разрешённому году.
                int? followingYear = definition.Years.GetNext(year);

                if (followingYear is null)
                    throw new InvalidOperationException("There is no next event in the definition.");

                year = followingYear.Value;
                month = 1;
                day = 1;
                hour = 0;
                minute = 0;
                second = 0;
                millisecond = 0;

                continue; // Начинаем алгоритм заново уже с нового года.
            }

            // Перезаписываем минуту
            if (nextMinute.Value != minute)
            {
                minute = nextMinute.Value;
                second = 0;
                millisecond = 0;

                continue; // Начинаем поиск заново уже с новой минуты.
            }


            // 6. Ищем ближайшую разрешённую секунду, большую или равную текущей
            int? nextSecond = definition.Seconds.GetNextOrSame(second);

            // Если в текущей минуте подходящей секунды больше нет,
            // переходим к следующей допустимой минуте
            if (nextSecond is null)
            {
                int? followingMinute = definition.Minutes.GetNext(minute);

                if (followingMinute is not null)
                {
                    minute = followingMinute.Value;
                    second = 0;
                    millisecond = 0;

                    continue; // Начинаем алгоритм заново уже с новой минуты.
                }

                // Если следующей минуты нет, текущий час считаем исчерпанным
                // и переходим к следующему допустимому часу.
                int? followingHour = definition.Hours.GetNext(hour);

                if (followingHour is not null)
                {
                    hour = followingHour.Value;
                    minute = 0;
                    second = 0;
                    millisecond = 0;

                    continue; // Начинаем алгоритм заново уже с нового часа.
                }

                // Если следующего часа нет, переходим к следующему допустимому дню.
                int? followingDay = FindNextAllowedDay(year, month, day + 1, definition);

                if (followingDay is not null)
                {
                    day = followingDay.Value;
                    hour = 0;
                    minute = 0;
                    second = 0;
                    millisecond = 0;

                    continue; // Начинаем алгоритм заново уже с нового дня.
                }

                // Если в текущем месяце больше допустимых дней нет,
                // переходим к следующему разрешённому месяцу.
                int? followingMonth = definition.Months.GetNext(month);

                if (followingMonth is not null)
                {
                    month = followingMonth.Value;
                    day = 1;
                    hour = 0;
                    minute = 0;
                    second = 0;
                    millisecond = 0;

                    continue; // Начинаем алгоритм заново уже с нового месяца.
                }

                // Если следующего месяца нет, переходим к следующему разрешённому году.
                int? followingYear = definition.Years.GetNext(year);

                if (followingYear is null)
                    throw new InvalidOperationException("There is no next event in the definition.");

                year = followingYear.Value;
                month = 1;
                day = 1;
                hour = 0;
                minute = 0;
                second = 0;
                millisecond = 0;

                continue; // Начинаем алгоритм заново уже с нового года.
            }

            // Перезаписываем секунду
            if (nextSecond.Value != second)
            {
                second = nextSecond.Value;
                millisecond = 0;

                continue; // Начинаем поиск заново уже с новой секунды.
            }


            // 7. Ищем ближайшую разрешённую миллисекунду, большую или равную текущей
            int? nextMillisecond = definition.Milliseconds.GetNextOrSame(millisecond);

            // Если в текущей секунде подходящей миллисекунды больше нет,
            // переходим к следующей допустимой секунде
            if (nextMillisecond is null)
            {
                int? followingSecond = definition.Seconds.GetNext(second);

                if (followingSecond is not null)
                {
                    second = followingSecond.Value;
                    millisecond = 0;

                    continue; // Начинаем алгоритм заново уже с новой секунды.
                }

                // Если следующей секунды нет, переходим к следующей допустимой минуте.
                int? followingMinute = definition.Minutes.GetNext(minute);

                if (followingMinute is not null)
                {
                    minute = followingMinute.Value;
                    second = 0;
                    millisecond = 0;

                    continue; // Начинаем алгоритм заново уже с новой минуты.
                }

                // Если следующей минуты нет, переходим к следующему допустимому часу.
                int? followingHour = definition.Hours.GetNext(hour);

                if (followingHour is not null)
                {
                    hour = followingHour.Value;
                    minute = 0;
                    second = 0;
                    millisecond = 0;

                    continue; // Начинаем алгоритм заново уже с нового часа.
                }

                // Если следующего часа нет, переходим к следующему допустимому дню.
                int? followingDay = FindNextAllowedDay(year, month, day + 1, definition);

                if (followingDay is not null)
                {
                    day = followingDay.Value;
                    hour = 0;
                    minute = 0;
                    second = 0;
                    millisecond = 0;

                    continue; // Начинаем алгоритм заново уже с нового дня.
                }

                // Если в текущем месяце больше допустимых дней нет,
                // переходим к следующему разрешённому месяцу.
                int? followingMonth = definition.Months.GetNext(month);

                if (followingMonth is not null)
                {
                    month = followingMonth.Value;
                    day = 1;
                    hour = 0;
                    minute = 0;
                    second = 0;
                    millisecond = 0;

                    continue; // Начинаем алгоритм заново уже с нового месяца.
                }

                // Если следующего месяца нет, переходим к следующему разрешённому году.
                int? followingYear = definition.Years.GetNext(year);

                if (followingYear is null)
                    throw new InvalidOperationException("There is no next event in the definition.");

                year = followingYear.Value;
                month = 1;
                day = 1;
                hour = 0;
                minute = 0;
                second = 0;
                millisecond = 0;

                continue; // Начинаем алгоритм заново уже с нового года.
            }

            // Перезаписываем миллисекунду
            millisecond = nextMillisecond.Value;

            return new DateTime(year, month, day, hour, minute, second, millisecond, value.Kind);
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

            bool dayAllowed = definition.Days.Contains(currentDay) // либо currentDay разрешён
                || definition.Days.Contains(32) && currentDay == daysInMonth; // либо currentDay последний в месяце, и последний день разрешён

            bool weekdayAllowed = definition.Weekdays.Contains((int)date.DayOfWeek); //проверяем день недели

            //должны одновременно выполняться и ограничение по числу месяца, и ограничение по дню недели
            if (dayAllowed && weekdayAllowed)
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
        int? month = definition.Months.GetNext(value.Month + 1);

        if (month is not null)
        {
            return CreateDateTime(value, value.Year, month.Value, 1, 0, 0, 0);
        }

        return MoveToNextAllowedYear(value, definition);
    }

    private static DateTime MoveToNextAllowedHour(DateTime value, ScheduleDefinition definition)
    {
        int? hour = definition.Hours.GetNext(value.Hour + 1);

        if (hour is not null)
        {
            return CreateDateTime(value, value.Year, value.Month, value.Day, hour.Value, 0, 0);
        }

        DateTime nextDay = value.AddDays(1);

        return CreateDateTime(nextDay, nextDay.Year, nextDay.Month, nextDay.Day, 0, 0, 0);
    }

    private static DateTime MoveToNextAllowedMinute(DateTime value, ScheduleDefinition definition)
    {
        int? minute = definition.Minutes.GetNext(value.Minute + 1);

        if (minute is not null)
        {
            return CreateDateTime(value, value.Year, value.Month, value.Day, value.Hour, minute.Value, 0);
        }

        DateTime nextHour = value.AddHours(1);

        return CreateDateTime(nextHour, nextHour.Year, nextHour.Month, nextHour.Day, nextHour.Hour, 0, 0);
    }

    private static DateTime MoveToNextAllowedSecond(DateTime value, ScheduleDefinition definition)
    {
        int? second = definition.Seconds.GetNext(value.Second + 1);

        if (second is not null)
        {
            return CreateDateTime(value, value.Year, value.Month, value.Day, value.Hour, value.Minute, second.Value, 0);
        }

        DateTime nextMinute = value.AddMinutes(1);

        return CreateDateTime(nextMinute, nextMinute.Year, nextMinute.Month, nextMinute.Day, nextMinute.Hour, nextMinute.Minute, 0);
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
        int? year = definition.Years.GetPrevious(value.Year - 1);

        if (year is null)
            throw new InvalidOperationException("There is no previous event in the definition.");

        return CreateDateTime(value, year.Value, 12, 31, 23, 59, 59, 999);
    }

    private static DateTime MoveToPreviousAllowedMonth(DateTime value, ScheduleDefinition definition)
    {
        int? month = definition.Months.GetPrevious(value.Month - 1);

        if (month is not null)
        {
            return CreateDateTime(value, value.Year, month.Value, DateTime.DaysInMonth(value.Year, month.Value), 23, 59, 59, 999);
        }

        return MoveToPreviousAllowedYear(value, definition);
    }

    private static DateTime MoveToPreviousAllowedDay(DateTime value, ScheduleDefinition definition)
    {
        DateTime previousDay = value.AddDays(-1);

        return CreateDateTime(previousDay, previousDay.Year, previousDay.Month, previousDay.Day, 23, 59, 59, 999);
    }

    private static DateTime MoveToPreviousAllowedHour(DateTime value, ScheduleDefinition definition)
    {
        int? hour = definition.Hours.GetPrevious(value.Hour - 1);

        if (hour is not null)
        {
            return CreateDateTime(value, value.Year, value.Month, value.Day, hour.Value, 59, 59, 999);
        }

        DateTime previousDay = value.AddDays(-1);

        return CreateDateTime(previousDay, previousDay.Year, previousDay.Month, previousDay.Day, 23, 59, 59, 999);
    }

    private static DateTime MoveToPreviousAllowedMinute(DateTime value, ScheduleDefinition definition)
    {
        int? minute = definition.Minutes.GetPrevious(value.Minute - 1);

        if (minute is not null)
        {
            return CreateDateTime(value, value.Year, value.Month, value.Day, value.Hour, minute.Value, 59, 999);
        }

        DateTime previousHour = value.AddHours(-1);

        return CreateDateTime(previousHour, previousHour.Year, previousHour.Month, previousHour.Day, previousHour.Hour, 59, 59, 999);
    }

    private static DateTime MoveToPreviousAllowedSecond(DateTime value, ScheduleDefinition definition)
    {
        int? second = definition.Seconds.GetPrevious(value.Second - 1);

        if (second is not null)
        {
            return CreateDateTime(value, value.Year, value.Month, value.Day, value.Hour, value.Minute, second.Value, 999);
        }

        DateTime previousMinute = value.AddMinutes(-1);

        return CreateDateTime(previousMinute, previousMinute.Year, previousMinute.Month, previousMinute.Day, previousMinute.Hour, previousMinute.Minute, 59, 999);
    }

    #endregion





    private static bool IsDateAllowed(DateTime date, ScheduleDefinition definition)
    {
        return IsDayAllowed(date, definition)
            && definition.Weekdays.Contains((int)date.DayOfWeek);
    }

    private static bool IsDayAllowed(DateTime date, ScheduleDefinition definition)
    {
        if (definition.Days.Contains(date.Day))
            return true;

        return definition.Days.Contains(32)
            && date.Day == DateTime.DaysInMonth(date.Year, date.Month);
    }


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

    private static DateTime CreateDateTime(DateTime source, int year, int month, int day, int hour, int minute, int second, int millisecond = 0)
    {
        return new DateTime(year, month, day, hour, minute, second, millisecond, source.Kind);
    }
}