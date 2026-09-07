using Complex_Systems.Model;
using Complex_Systems.Parsing;

namespace Complex_Systems.Tests;

public sealed class FieldParserTests
{
    [Fact]
    public void Parse_Star_AddsAllValuesInRange()
    {
        ScheduleField field = FieldParser.Parse("*", 0, 5);

        for (int value = 0; value <= 5; value++)
        {
            Assert.True(field.Contains(value));
        }
    }

    [Fact]
    public void Parse_StepWithStar_AddsValuesWithStep()
    {
        ScheduleField field = FieldParser.Parse("*/4", 0, 23);

        int[] expectedValues = [0, 4, 8, 12, 16, 20];

        for (int value = 0; value <= 23; value++)
        {
            bool expected = expectedValues.Contains(value);

            Assert.Equal(expected, field.Contains(value));
        }
    }

    [Fact]
    public void Parse_Range_AddsAllValuesInRange()
    {
        ScheduleField field = FieldParser.Parse("5-10", 0, 23);

        for (int value = 0; value <= 23; value++)
        {
            bool expected = value is >= 5 and <= 10;

            Assert.Equal(expected, field.Contains(value));
        }
    }

    [Fact]
    public void Parse_RangeWithStep_AddsExpectedValues()
    {
        ScheduleField field = FieldParser.Parse("1-10/3", 0, 20);

        int[] expectedValues = [1, 4, 7, 10];

        for (int value = 0; value <= 20; value++)
        {
            bool expected = expectedValues.Contains(value);

            Assert.Equal(expected, field.Contains(value));
        }
    }

    [Fact]
    public void Parse_SingleValue_AddsOnlySpecifiedValue()
    {
        ScheduleField field = FieldParser.Parse("7", 0, 10);

        for (int value = 0; value <= 10; value++)
        {
            Assert.Equal(value == 7, field.Contains(value));
        }
    }

    [Fact]
    public void Parse_List_AddsAllSpecifiedValues()
    {
        ScheduleField field = FieldParser.Parse("1,3,7,10", 0, 10);

        int[] expectedValues = [1, 3, 7, 10];

        for (int value = 0; value <= 10; value++)
        {
            bool expected = expectedValues.Contains(value);

            Assert.Equal(expected, field.Contains(value));
        }
    }

    [Fact]
    public void Parse_ListAndRanges_CombinesAllValues()
    {
        ScheduleField field = FieldParser.Parse(
            "1,2,3-5,10-20/3",
            0,
            30);

        int[] expectedValues =
        [
            1, 2,
        3, 4, 5,
        10, 13, 16, 19
        ];

        for (int value = 0; value <= 30; value++)
        {
            bool expected = expectedValues.Contains(value);

            Assert.Equal(expected, field.Contains(value));
        }
    }

    [Fact]
    public void Parse_OverlappingParts_DoesNotAffectResult()
    {
        ScheduleField field = FieldParser.Parse(
            "1-5,3-7",
            0,
            10);

        int[] expectedValues = [1, 2, 3, 4, 5, 6, 7];

        for (int value = 0; value <= 10; value++)
        {
            bool expected = expectedValues.Contains(value);

            Assert.Equal(expected, field.Contains(value));
        }
    }

    [Fact]
    public void Parse_UsesSpecifiedMinimumAndMaximum()
    {
        ScheduleField field = FieldParser.Parse(
            "50-100/10",
            50,
            100);

        int[] expectedValues = [50, 60, 70, 80, 90, 100];

        for (int value = 50; value <= 100; value++)
        {
            bool expected = expectedValues.Contains(value);

            Assert.Equal(expected, field.Contains(value));
        }
    }

    [Fact]
    public void Parse_StarWithNonZeroMinimum_UsesSpecifiedRange()
    {
        ScheduleField field = FieldParser.Parse("*", 10, 15);

        for (int value = 10; value <= 15; value++)
        {
            Assert.True(field.Contains(value));
        }

        Assert.False(field.Contains(9));
        Assert.False(field.Contains(16));
    }

    [Fact]
    public void Parse_EmptyExpression_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => FieldParser.Parse("", 0, 23));
    }

    [Fact]
    public void Parse_WhitespaceExpression_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => FieldParser.Parse("   ", 0, 23));
    }

    [Fact]
    public void Parse_EmptyListPart_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => FieldParser.Parse("1,,3", 0, 23));
    }

    [Fact]
    public void Parse_EmptyPartAtBeginning_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => FieldParser.Parse(",1", 0, 23));
    }

    [Fact]
    public void Parse_EmptyPartAtEnd_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => FieldParser.Parse("1,", 0, 23));
    }

    [Fact]
    public void Parse_ZeroStep_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => FieldParser.Parse("*/0", 0, 23));
    }

    [Fact]
    public void Parse_NegativeStep_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => FieldParser.Parse("*/-1", 0, 23));
    }

    [Fact]
    public void Parse_NonNumericStep_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => FieldParser.Parse("*/abc", 0, 23));
    }

    [Fact]
    public void Parse_MissingStep_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => FieldParser.Parse("*/", 0, 23));
    }

    [Fact]
    public void Parse_TooManyStepSeparators_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => FieldParser.Parse("1-5/2/3", 0, 23));
    }

    [Fact]
    public void Parse_ValueOutsideRange_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => FieldParser.Parse("24", 0, 23));
    }

    [Fact]
    public void Parse_RangeStartOutsideRange_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => FieldParser.Parse("24-30", 0, 23));
    }

    [Fact]
    public void Parse_RangeEndOutsideRange_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => FieldParser.Parse("10-24", 0, 23));
    }

    [Fact]
    public void Parse_ReversedRange_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => FieldParser.Parse("10-5", 0, 23));
    }

    [Fact]
    public void Parse_MissingRangeStart_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => FieldParser.Parse("-5", 0, 23));
    }

    [Fact]
    public void Parse_MissingRangeEnd_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => FieldParser.Parse("5-", 0, 23));
    }

    [Fact]
    public void Parse_TooManyRangeSeparators_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => FieldParser.Parse("1-5-10", 0, 23));
    }

    [Fact]
    public void Parse_NonNumericValue_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => FieldParser.Parse("abc", 0, 23));
    }
}
