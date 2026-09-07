using Complex_Systems.Model;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Complex_Systems.Parsing;

/// <summary>
/// Разбирает отдельное поле расписания
/// </summary>
internal static class FieldParser
{
    /// <summary>
    /// Разбирает выражение одного поля расписания и создаёт
    /// неизменяемое поле с разрешёнными значениями.
    /// </summary>
    /// <param name="expression">
    /// Например: "*", "*/4", "1-5", "1,2,3-10/2".
    /// </param>
    /// <param name="minValue">Минимально возможное значение поля.</param>
    /// <param name="maxValue">Максимально возможное значение поля.</param>
    /// <returns>Неизменяемое поле с разрешёнными значениями.</returns>
    /// <exception cref="ArgumentException">
    /// Если выражение имеет неправильный формат или содержит значение вне допустимого диапазона.
    /// </exception>
    public static ScheduleField Parse(string expression, int minValue, int maxValue)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new ArgumentException(
                "Schedule field cannot be empty.",
                nameof(expression));
        }

        var allowedValues = new List<int>();

        string[] parts = expression.Split(',');

        foreach (string part in parts)
        {
            if (string.IsNullOrWhiteSpace(part))
            {
                throw new ArgumentException(
                    "Schedule field contains an empty part.");
            }

            ParsePart(part.Trim(), minValue, maxValue, allowedValues);
        }

        return new ScheduleField(minValue, maxValue, CollectionsMarshal.AsSpan(allowedValues));
    }

    /// <summary> 
    /// Разбирает один элемент списка и добавляет полученные 
    /// разрешённые значения в указанный список.
    /// </summary> 
    /// <param name="part">Отдельный элемент выражения</param>
    /// <param name="allowedValues">Список, в который добавляются разрешённые значения.</param>
    private static void ParsePart(string part, int minValue, int maxValue, List<int> allowedValues)
    {
        // Отделяем шаг от основной части выражения
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
        }

        string rangeExpression = stepParts[0].Trim();

        // Звёздочка означает весь допустимый диапазон поля.
        if (rangeExpression == "*")
        {
            AddRange(
                allowedValues,
                minValue,
                maxValue,
                step);

            return;
        }

        // Если это не "*", проверяем наличие диапазона.
        string[] rangeParts = rangeExpression.Split('-');

        if (rangeParts.Length > 2)
        {
            throw new ArgumentException($"Invalid range expression: '{part}'.");
        }

        if (string.IsNullOrWhiteSpace(rangeParts[0]))
        {
            throw new ArgumentException($"Range start is missing: '{part}'.");
        }

        int start = ParseValue(rangeParts[0], part);

        // Если диапазон не указан, единственным значением считается само начало
        int end = start;

        if (rangeParts.Length == 2)
        {
            if (string.IsNullOrWhiteSpace(rangeParts[1]))
            {
                throw new ArgumentException($"Range end is missing: '{part}'.");
            }

            end = ParseValue(rangeParts[1], part);
        }

        ValidateRange(start, end, minValue, maxValue, part);

        AddRange(
            allowedValues,
            start,
            end,
            step);
    }

    /// <summary>
    /// Добавляет в список значения диапазона с указанным шагом.
    /// </summary>
    private static void AddRange(List<int> allowedValues, int start, int end, int step)
    {
        for (int value = start; value <= end; value += step)
        {
            allowedValues.Add(value);
        }
    }

    /// <summary>
    /// Преобразует строковое значение в целое число.
    /// </summary>
    /// <param name="value">Строковое представление числа.</param>
    /// <param name="expression">Исходное выражение.</param>
    /// <returns>Распарсенное целое число.</returns>
    private static int ParseValue(string value, string expression)
    {
        if (!int.TryParse(
                value.Trim(),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int result))
        {
            throw new ArgumentException($"Invalid numeric value in expression: '{expression}'.");
        }

        return result;
    }

    /// <summary>
    /// Преобразует строковый шаг в положительное целое число.
    /// </summary>
    /// <param name="value">Строковое представление шага.</param>
    /// <param name="expression">Исходное выражение, задающее шаг.</param>
    /// <returns>Положительное значение шага.</returns>
    private static int ParsePositiveInteger(string value, string expression)
    {
        if (!int.TryParse(
                value.Trim(),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int result)
            || result <= 0)
        {
            throw new ArgumentException($"Step must be a positive integer: '{expression}'.");
        }

        return result;
    }

    /// <summary>
    /// Проверяет, что границы диапазона находятся в пределах допустимого диапазона поля.
    /// </summary>
    /// <param name="start">Начало диапазона.</param>
    /// <param name="end">Конец диапазона.</param>
    /// <param name="minValue">Минимально возможное значение поля.</param>
    /// <param name="maxValue">Максимально возможное значение поля.</param>
    /// <param name="expression">Исходное выражение.</param>
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
