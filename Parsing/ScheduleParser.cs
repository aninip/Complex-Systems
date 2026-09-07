using Complex_Systems.Interfaces;
using Complex_Systems.Model;

namespace Complex_Systems.Parsing
{
    /// <summary>
    /// Разбирает полное строковое представление расписания
    /// и создаёт его внутреннее представление
    /// </summary>
    internal sealed class ScheduleParser : IScheduleParser
    {
        // у нас есть 6 допустимых форматов:
        // [date] [weekday] [time]
        //
        // yyyy.MM.dd  w  HH:mm:ss.fff
        // yyyy.MM.dd     HH:mm:ss.fff
        //                HH:mm:ss.fff
        // yyyy.MM.dd  w  HH:mm:ss
        // yyyy.MM.dd     HH:mm:ss
        //                HH:mm:ss

        //отсутствующая часть трактуется как *, кроме миллисекунд, где отсутствие означает именно 0, согласно заданию.

        // 1 токен:               time
        // 2 токена: date         time
        // 3 токена: date weekday time

        // У времени очень характерная структура:
        //  HH:mm:ss
        //  HH:mm:ss.fff
        // максимум одна точка и три компоненты через ':'

        /// <summary>
        /// Разбирает строку расписания
        /// </summary>
        /// <param name="scheduleString">Строковое представление расписания.</param>
        /// <returns>Готовое внутреннее представление расписания.</returns>
        public ScheduleDefinition Parse(string scheduleString)
        {
            if (string.IsNullOrWhiteSpace(scheduleString))
            {
                throw new ArgumentException(
                    "Schedule string cannot be empty.",
                    nameof(scheduleString));
            }

            // разбиваем по пробелам
            string[] parts = scheduleString
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 1 || parts.Length > 3)
            {
                throw new ArgumentException(
                    $"Invalid schedule format: '{scheduleString}'.",
                    nameof(scheduleString));
            }

            string datePart = "*.*.*";
            string weekdayPart = "*";
            string timePart = "*:*:0";

            ParseParts(parts, ref datePart, ref weekdayPart, ref timePart);

            ParseDate(datePart, out string yearExpression, out string monthExpression, out string dayExpression);

            ParseTime(timePart, out string hourExpression, out string minuteExpression, out string secondExpression, out string millisecondExpression);

            return new ScheduleDefinition(
                FieldParser.Parse(yearExpression, 2000, 2100),
                FieldParser.Parse(monthExpression, 1, 12),
                FieldParser.Parse(dayExpression, 1, 32),
                FieldParser.Parse(weekdayPart, 0, 6),
                FieldParser.Parse(hourExpression, 0, 23),
                FieldParser.Parse(minuteExpression, 0, 59),
                FieldParser.Parse(secondExpression, 0, 59),
                FieldParser.Parse(millisecondExpression, 0, 999));
        }

        private static void ParseParts(
            string[] parts,
            ref string datePart,
            ref string weekdayPart,
            ref string timePart)
        {
            if (parts.Length == 1)
            {
                timePart = parts[0];
                return;
            }
            else if (parts.Length == 2)
            {
                datePart = parts[0];
                timePart = parts[1];
                return;
            }
            else //(parts.Length == 3)
            {
                datePart = parts[0];
                weekdayPart = parts[1];
                timePart = parts[2];
            }
        }

        private static void ParseDate(
            string datePart,
            out string yearExpression,
            out string monthExpression,
            out string dayExpression)
        {
            string[] parts = datePart.Split('.');

            //требуется ровно 3 компонента даты
            if (parts.Length != 3)
            {
                throw new ArgumentException(
                    $"Invalid date expression: '{datePart}'.");
            }

            yearExpression = parts[0];
            monthExpression = parts[1];
            dayExpression = parts[2];
        }

        private static void ParseTime(
            string timePart,
            out string hourExpression,
            out string minuteExpression,
            out string secondExpression,
            out string millisecondExpression)
        {
            // разделяем миллисекунды по '.'
            string[] secondParts = timePart.Split('.');

            if (secondParts.Length > 2)
            {
                throw new ArgumentException(
                    $"Invalid time expression: '{timePart}'.");
            }

            string[] timeParts = secondParts[0].Split(':');

            //требуется ровно 3 компонента времени
            if (timeParts.Length != 3)
            {
                throw new ArgumentException(
                    $"Invalid time expression: '{timePart}'.");
            }

            hourExpression = timeParts[0];
            minuteExpression = timeParts[1];
            secondExpression = timeParts[2];

            //если миллисекунды отсутствуют — подставляем "0"
            millisecondExpression =
                secondParts.Length == 2
                    ? secondParts[1]
                    : "0";
        }
    }
}
