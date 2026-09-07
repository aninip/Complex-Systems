using Complex_Systems.Model;
using System.Globalization;

namespace Complex_Systems.Parsing
{
    internal static class FieldParser
    {
        /// <summary>
        /// Разбирает выражение одного поля расписания.
        /// </summary>
        /// <param name="expression">
        /// Например: "*", "*/4", "1-5", "1,2,3-10/2".
        /// </param>
        /// <param name="minValue">Минимальное допустимое значение поля.</param>
        /// <param name="maxValue">Максимальное допустимое значение поля.</param>
        /// <returns>Заполненное поле расписания.</returns>
        /// <exception cref="ArgumentException">
        /// Если выражение имеет неправильный формат или содержит недопустимые значения.
        /// </exception>
        public static ScheduleField Parse(string expression, int minValue, int maxValue)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentException(
                    "Schedule field cannot be empty.",
                    nameof(expression));
            }

            var field = new ScheduleField(minValue, maxValue);

            string[] parts = expression.Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                throw new ArgumentException(
                    $"Invalid schedule field: '{expression}'.",
                    nameof(expression));
            }

            foreach (string part in parts)
            {
                ParsePart(part.Trim(), field, minValue, maxValue);
            }

            return field;
        }

        private static void ParsePart(
            string part,
            ScheduleField field,
            int minValue,
            int maxValue)
        {
            if (string.IsNullOrWhiteSpace(part))
            {
                throw new ArgumentException(
                    "Schedule field contains an empty part.");
            }

            string[] stepParts = part.Split('/');

            if (stepParts.Length > 2)
            {
                throw new ArgumentException(
                    $"Invalid step expression: '{part}'.");
            }

            int step = 1;

            if (stepParts.Length == 2)
            {
                if (string.IsNullOrWhiteSpace(stepParts[1]))
                {
                    throw new ArgumentException(
                        $"Step is missing in expression: '{part}'.");
                }

                step = ParsePositiveInteger(stepParts[1], part);

                if (step <= 0)
                {
                    throw new ArgumentException(
                        $"Step must be greater than zero: '{part}'.");
                }
            }

            string rangeExpression = stepParts[0].Trim();

            if (rangeExpression == "*")
            {
                AddRange(
                    field,
                    minValue,
                    maxValue,
                    step);

                return;
            }

            string[] rangeParts = rangeExpression.Split('-');

            if (rangeParts.Length > 2)
            {
                throw new ArgumentException(
                    $"Invalid range expression: '{part}'.");
            }

            int start = ParseValue(rangeParts[0], part);
            int end = start;

            if (rangeParts.Length == 2)
            {
                if (string.IsNullOrWhiteSpace(rangeParts[1]))
                {
                    throw new ArgumentException(
                        $"Range end is missing: '{part}'.");
                }

                end = ParseValue(rangeParts[1], part);
            }

            ValidateRange(start, end, minValue, maxValue, part);

            AddRange(
                field,
                start,
                end,
                step);
        }

        private static void AddRange(
            ScheduleField field,
            int start,
            int end,
            int step)
        {
            for (int value = start; value <= end; value += step)
            {
                field.Add(value);
            }
        }

        private static int ParseValue(
            string value,
            string expression)
        {
            if (!int.TryParse(
                    value.Trim(),
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out int result))
            {
                throw new ArgumentException(
                    $"Invalid numeric value in expression: '{expression}'.");
            }

            return result;
        }

        private static int ParsePositiveInteger(
            string value,
            string expression)
        {
            if (!int.TryParse(
                    value.Trim(),
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out int result))
            {
                throw new ArgumentException(
                    $"Invalid step in expression: '{expression}'.");
            }

            return result;
        }

        private static void ValidateRange(
            int start,
            int end,
            int minValue,
            int maxValue,
            string expression)
        {
            if (start < minValue || start > maxValue)
            {
                throw new ArgumentException(
                    $"Value {start} is outside the allowed range " +
                    $"{minValue}..{maxValue}: '{expression}'.");
            }

            if (end < minValue || end > maxValue)
            {
                throw new ArgumentException(
                    $"Value {end} is outside the allowed range " +
                    $"{minValue}..{maxValue}: '{expression}'.");
            }

            if (start > end)
            {
                throw new ArgumentException(
                    $"Range start cannot be greater than range end: '{expression}'.");
            }
        }
    }
}




