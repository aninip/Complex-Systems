using Complex_Systems.Model;
using Complex_Systems.Parsing;

namespace Complex_Systems.ScheduleTests
{
    public class FieldParserTests
    {
        [Fact]
        public void Star_ShouldAddAllValues()
        {
            ScheduleField field = FieldParser.Parse("*", 0, 5);

            for (int i = 0; i <= 5; i++)
            {
                Assert.True(field.Contains(i));
            }
        }

        [Fact]
        public void Step_ShouldAddValuesWithStep()
        {
            ScheduleField field = FieldParser.Parse("*/4", 0, 23);

            Assert.True(field.Contains(0));
            Assert.True(field.Contains(4));
            Assert.True(field.Contains(8));
            Assert.True(field.Contains(12));
            Assert.True(field.Contains(16));
            Assert.True(field.Contains(20));

            Assert.False(field.Contains(1));
            Assert.False(field.Contains(5));
            Assert.False(field.Contains(21));
        }

        [Fact]
        public void RangeWithStep_ShouldAddExpectedValues()
        {
            ScheduleField field = FieldParser.Parse("1-10/3", 0, 20);

            Assert.True(field.Contains(1));
            Assert.True(field.Contains(4));
            Assert.True(field.Contains(7));
            Assert.True(field.Contains(10));

            Assert.False(field.Contains(2));
            Assert.False(field.Contains(5));
        }

        [Fact]
        public void ListAndRanges_ShouldBeCombined()
        {
            ScheduleField field =
                FieldParser.Parse(
                    "1,2,3-5,10-20/3",
                    0,
                    30);

            int[] expected =
            {
            1, 2, 3, 4, 5,
            10, 13, 16, 19
        };

            foreach (int value in expected)
            {
                Assert.True(field.Contains(value));
            }
        }

        [Fact]
        public void ZeroStep_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(
                () => FieldParser.Parse("*/0", 0, 23));
        }

        [Fact]
        public void ValueOutsideRange_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(
                () => FieldParser.Parse("24", 0, 23));
        }
    }
}
