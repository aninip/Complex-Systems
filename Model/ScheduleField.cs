using System.Numerics;

namespace Complex_Systems.Model;

public readonly struct ScheduleField
{
    private readonly ulong[] _words;

    public int Min { get; }
    public int Max { get; }

    public ScheduleField(int min, int max)
    {
        if (min < 0)
            throw new ArgumentOutOfRangeException(nameof(min));

        if (max < min)
            throw new ArgumentOutOfRangeException(nameof(max));

        Min = min;
        Max = max;

        int wordCount = (max + 64) / 64;
        _words = new ulong[wordCount];
    }

    /// <summary>
    /// Устанавливает указанное значение как разрешённое.
    /// </summary>
    public void Add(int value)
    {
        ValidateValue(value);

        int wordIndex = value / 64;
        int bitIndex = value % 64;

        _words[wordIndex] |= 1UL << bitIndex;
    }

    /// <summary>
    /// Проверяет, разрешено ли указанное значение.
    /// </summary>
    public bool Contains(int value)
    {
        if (value < Min || value > Max)
            return false;

        int wordIndex = value / 64;
        int bitIndex = value % 64;

        return (_words[wordIndex] & (1UL << bitIndex)) != 0;
    }

    /// <summary>
    /// Возвращает ближайшее разрешённое значение,
    /// которое больше или равно указанному.
    /// </summary>
    public int? GetNextOrSame(int value)
    {
        if (value < Min)
            value = Min;

        if (value > Max)
            return null;

        int wordIndex = value / 64;
        int bitIndex = value % 64;

        // Убираем все биты левее value.
        ulong word = _words[wordIndex] & (~0UL << bitIndex);

        if (word != 0)
        {
            int offset = BitOperations.TrailingZeroCount(word);
            int result = wordIndex * 64 + offset;

            return result <= Max ? result : null;
        }

        // Ищем в следующих словах.
        for (wordIndex++; wordIndex < _words.Length; wordIndex++)
        {
            word = _words[wordIndex];

            if (word == 0)
                continue;

            int offset = BitOperations.TrailingZeroCount(word);
            int result = wordIndex * 64 + offset;

            return result <= Max ? result : null;
        }

        return null;
    }

    /// <summary>
    /// Возвращает ближайшее разрешённое значение,
    /// которое строго больше указанного.
    /// </summary>
    public int? GetNext(int value)
    {
        if (value < Min - 1)
            value = Min - 1;

        return GetNextOrSame(value + 1);
    }

    /// <summary>
    /// Возвращает ближайшее разрешённое значение,
    /// которое меньше или равно указанному.
    /// </summary>
    public int? GetPreviousOrSame(int value)
    {
        if (value > Max)
            value = Max;

        if (value < Min)
            return null;

        int wordIndex = value / 64;
        int bitIndex = value % 64;

        // Оставляем только биты от начала слова до value включительно.
        ulong mask = bitIndex == 63
            ? ulong.MaxValue
            : (1UL << (bitIndex + 1)) - 1;

        ulong word = _words[wordIndex] & mask;

        if (word != 0)
        {
            int offset = 63 - BitOperations.LeadingZeroCount(word);
            int result = wordIndex * 64 + offset;

            return result >= Min ? result : null;
        }

        // Ищем в предыдущих словах.
        for (wordIndex--; wordIndex >= 0; wordIndex--)
        {
            word = _words[wordIndex];

            if (word == 0)
                continue;

            int offset = 63 - BitOperations.LeadingZeroCount(word);
            int result = wordIndex * 64 + offset;

            return result >= Min ? result : null;
        }

        return null;
    }

    /// <summary>
    /// Возвращает ближайшее разрешённое значение,
    /// которое строго меньше указанного.
    /// </summary>
    public int? GetPrevious(int value)
    {
        if (value > Max + 1)
            value = Max + 1;

        return GetPreviousOrSame(value - 1);
    }

    private void ValidateValue(int value)
    {
        if (value < Min || value > Max)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                $"Value must be between {Min} and {Max}.");
        }
    }
}