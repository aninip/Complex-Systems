using System.Numerics;

namespace Complex_Systems.Model;

/// <summary>
/// Неизменяемое битовое представление разрешённых значений диапазона
/// </summary>
internal sealed class ScheduleField
{
    /// <summary>
    /// Битовое представление разрешённых значений
    /// </summary>
    private readonly ulong[] _words;

    /// <summary>
    /// Количество значений в диапазоне от Min до Max включительно
    /// </summary>
    private readonly int _valueCount;

    /// <summary>
    /// Минимальное значение диапазона
    /// </summary>
    public int Min { get; }

    /// <summary>
    /// Максимальное значение диапазона
    /// </summary>
    public int Max { get; }

    /// <summary>
    /// Создаёт поле с указанными границами допустимого диапазона и отмечает переданные значения как разрешённые
    /// <para>
    /// Например, для задания разрешённых часов 0, 4, 8, 12, 16 и 20:
    /// <c> new ScheduleField(0, 23, [0, 4, 8, 12, 16, 20])</c>
    /// </para>
    /// </summary>
    /// <param name="min">Минимально возможное значение поля</param>
    /// <param name="max">Максимально возможное значение поля</param>
    /// <param name="allowedValues">Значения, разрешённые в расписании</param>
    public ScheduleField(int min, int max, ReadOnlySpan<int> allowedValues)
    {
        if (min < 0)
            throw new ArgumentOutOfRangeException(nameof(min));

        if (max < min)
            throw new ArgumentOutOfRangeException(nameof(max));

        Min = min;
        Max = max;

        // Количество значений, которое может содержать поле
        _valueCount = checked(max - min + 1);
        // считаем сколько 64-битных word нужно, чтобы вместить _valueCount значений. и целочисленное округление вверх за счёт '+ 63'
        int wordCount = (_valueCount + 63) / 64;

        _words = new ulong[wordCount];

        foreach (int value in allowedValues)
        {
            ValidateValue(value);

            int offset = value - Min;
            int wordIndex = offset / 64;
            int bitIndex = offset % 64;

            _words[wordIndex] |= 1UL << bitIndex;
        }
    }

    /// <summary>
    /// Проверяет, разрешено ли указанное значение
    /// </summary>
    public bool Contains(int value)
    {
        if (value < Min || value > Max)
            return false;

        int offset = value - Min;
        int wordIndex = offset / 64;
        int bitIndex = offset % 64;

        return (_words[wordIndex] & (1UL << bitIndex)) != 0;
    }

    /// <summary>
    /// Возвращает ближайшее разрешённое значение, которое больше или равно указанному
    /// </summary>
    public int? GetNextOrSame(int value)
    {
        if (value < Min)
            value = Min;

        if (value > Max)
            return null;

        int offset = value - Min;

        int wordIndex = offset / 64;
        int bitIndex = offset % 64;

        // Оставляем текущий бит и все биты правее него
        ulong word = _words[wordIndex] & (~0UL << bitIndex);

        if (word != 0)
            return Min + GetLowestValueOffset(wordIndex, word);

        // Ищем следующее непустое слово
        for (int index = wordIndex + 1; index < _words.Length; index++)
        {
            word = _words[index];

            if (word == 0)
                continue;

            return Min + GetLowestValueOffset(index, word);
        }

        return null;
    }

    /// <summary>
    /// Возвращает ближайшее разрешённое значение, которое строго больше указанного
    /// </summary>
    public int? GetNext(int value)
    {
        if (value >= Max)
            return null;

        if (value < Min)
            return GetNextOrSame(Min);

        return GetNextOrSame(value + 1);
    }

    /// <summary>
    /// Возвращает ближайшее разрешённое значение, которое меньше или равно указанному
    /// </summary>
    public int? GetPreviousOrSame(int value)
    {
        if (value > Max)
            value = Max;

        if (value < Min)
            return null;

        int offset = value - Min;

        int wordIndex = offset / 64;
        int bitIndex = offset % 64;

        // Оставляем все биты от начала слова до текущего включительно
        ulong mask = bitIndex == 63
            ? ulong.MaxValue
            : (1UL << (bitIndex + 1)) - 1;

        ulong word = _words[wordIndex] & mask;

        if (word != 0)
            return Min + GetHighestValueOffset(wordIndex, word);

        // Ищем предыдущее непустое слово
        for (int index = wordIndex - 1; index >= 0; index--)
        {
            word = _words[index];

            if (word == 0)
                continue;

            return Min + GetHighestValueOffset(index, word);
        }

        return null;
    }

    /// <summary>
    /// Возвращает ближайшее разрешённое значение, которое строго меньше указанного
    /// </summary>
    public int? GetPrevious(int value)
    {
        if (value <= Min)
            return null;

        if (value > Max)
            return GetPreviousOrSame(Max);

        return GetPreviousOrSame(value - 1);
    }

    private int GetLowestValueOffset(int wordIndex, ulong word)
    {
        int bitIndex = BitOperations.TrailingZeroCount(word);
        return wordIndex * 64 + bitIndex;
    }

    private int GetHighestValueOffset(int wordIndex, ulong word)
    {
        int bitIndex = 63 - BitOperations.LeadingZeroCount(word);
        return wordIndex * 64 + bitIndex;
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