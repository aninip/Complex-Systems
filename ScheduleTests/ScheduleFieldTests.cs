using Complex_Systems.Model;

namespace Complex_Systems.ScheduleTests
{
    public sealed class ScheduleFieldTests
    {
        [Fact]
        public void Contains_ReturnsTrueForAllowedValue()
        {
            var field = new ScheduleField(
            0,
            23,
            [0, 4, 8, 12, 16, 20]);

            Assert.True(field.Contains(0));
            Assert.True(field.Contains(4));
            Assert.True(field.Contains(12));
            Assert.True(field.Contains(20));
        }

        [Fact]
        public void Contains_ReturnsFalseForDisallowedValue()
        {
            var field = new ScheduleField(
                0,
                23,
                [0, 4, 8, 12, 16, 20]);

            Assert.False(field.Contains(1));
            Assert.False(field.Contains(5));
            Assert.False(field.Contains(13));
            Assert.False(field.Contains(21));
        }

        [Fact]
        public void Contains_ReturnsFalseForValueOutsideRange()
        {
            var field = new ScheduleField(
                0,
                23,
                [0, 4, 8, 12, 16, 20]);

            Assert.False(field.Contains(-1));
            Assert.False(field.Contains(24));
        }

        [Fact]
        public void GetNextOrSame_ReturnsSameValueWhenValueIsAllowed()
        {
            var field = new ScheduleField(
                0,
                23,
                [0, 4, 8, 12, 16, 20]);

            Assert.Equal(12, field.GetNextOrSame(12));
        }

        [Fact]
        public void GetNextOrSame_ReturnsNextAllowedValue()
        {
            var field = new ScheduleField(
                0,
                23,
                [0, 4, 8, 12, 16, 20]);

            Assert.Equal(16, field.GetNextOrSame(13));
            Assert.Equal(20, field.GetNextOrSame(17));
        }

        [Fact]
        public void GetNextOrSame_ReturnsFirstAllowedValueWhenValueIsBelowRange()
        {
            var field = new ScheduleField(
                10,
                20,
                [12, 15, 20]);

            Assert.Equal(12, field.GetNextOrSame(5));
        }

        [Fact]
        public void GetNextOrSame_ReturnsNullWhenThereIsNoNextValue()
        {
            var field = new ScheduleField(
                0,
                23,
                [0, 4, 8]);

            Assert.Null(field.GetNextOrSame(9));
            Assert.Null(field.GetNextOrSame(23));
            Assert.Null(field.GetNextOrSame(24));
        }

        [Fact]
        public void GetNext_ReturnsStrictlyGreaterAllowedValue()
        {
            var field = new ScheduleField(
                0,
                23,
                [0, 4, 8, 12, 16, 20]);

            Assert.Equal(16, field.GetNext(12));
            Assert.Equal(20, field.GetNext(16));
        }

        [Fact]
        public void GetNext_ReturnsFirstAllowedValueWhenValueIsBelowRange()
        {
            var field = new ScheduleField(
                10,
                20,
                [12, 15, 20]);

            Assert.Equal(12, field.GetNext(5));
        }

        [Fact]
        public void GetNext_ReturnsNullWhenValueIsAtOrAboveMaximum()
        {
            var field = new ScheduleField(
                0,
                23,
                [0, 4, 8]);

            Assert.Null(field.GetNext(8));
            Assert.Null(field.GetNext(23));
            Assert.Null(field.GetNext(24));
        }

        [Fact]
        public void GetPreviousOrSame_ReturnsSameValueWhenValueIsAllowed()
        {
            var field = new ScheduleField(
                0,
                23,
                [0, 4, 8, 12, 16, 20]);

            Assert.Equal(12, field.GetPreviousOrSame(12));
        }

        [Fact]
        public void GetPreviousOrSame_ReturnsPreviousAllowedValue()
        {
            var field = new ScheduleField(
                0,
                23,
                [0, 4, 8, 12, 16, 20]);

            Assert.Equal(12, field.GetPreviousOrSame(13));
            Assert.Equal(16, field.GetPreviousOrSame(19));
        }

        [Fact]
        public void GetPreviousOrSame_ReturnsLastAllowedValueWhenValueIsAboveRange()
        {
            var field = new ScheduleField(
                10,
                20,
                [12, 15, 20]);

            Assert.Equal(20, field.GetPreviousOrSame(25));
        }

        [Fact]
        public void GetPreviousOrSame_ReturnsNullWhenThereIsNoPreviousValue()
        {
            var field = new ScheduleField(
                0,
                23,
                [8, 12, 16]);

            Assert.Null(field.GetPreviousOrSame(0));
            Assert.Null(field.GetPreviousOrSame(7));
            Assert.Null(field.GetPreviousOrSame(-1));
        }

        [Fact]
        public void GetPrevious_ReturnsStrictlySmallerAllowedValue()
        {
            var field = new ScheduleField(
                0,
                23,
                [0, 4, 8, 12, 16, 20]);

            Assert.Equal(12, field.GetPrevious(16));
            Assert.Equal(16, field.GetPrevious(20));
        }

        [Fact]
        public void GetPrevious_ReturnsLastAllowedValueWhenValueIsAboveRange()
        {
            var field = new ScheduleField(
                10,
                20,
                [12, 15, 20]);

            Assert.Equal(20, field.GetPrevious(25));
        }

        [Fact]
        public void GetPrevious_ReturnsNullWhenValueIsAtOrBelowMinimum()
        {
            var field = new ScheduleField(
                0,
                23,
                [0, 4, 8]);

            Assert.Null(field.GetPrevious(0));
            Assert.Null(field.GetPrevious(-1));
        }

        [Fact]
        public void NonZeroMinimum_IsHandledCorrectly()
        {
            var field = new ScheduleField(
                50,
                100,
                [50, 60, 75, 100]);

            Assert.True(field.Contains(50));
            Assert.True(field.Contains(60));
            Assert.True(field.Contains(75));
            Assert.True(field.Contains(100));

            Assert.False(field.Contains(51));
            Assert.False(field.Contains(99));

            Assert.Equal(60, field.GetNext(55));
            Assert.Equal(75, field.GetNext(60));

            Assert.Equal(60, field.GetPrevious(70));
            Assert.Equal(50, field.GetPrevious(60));
        }

        [Fact]
        public void ValuesSpanningMultipleWords_AreHandledCorrectly()
        {
            var field = new ScheduleField(
                0,
                130,
                [0, 1, 63, 64, 65, 100, 129, 130]);

            Assert.True(field.Contains(0));
            Assert.True(field.Contains(63));
            Assert.True(field.Contains(64));
            Assert.True(field.Contains(65));
            Assert.True(field.Contains(100));
            Assert.True(field.Contains(129));
            Assert.True(field.Contains(130));

            Assert.Equal(64, field.GetNext(63));
            Assert.Equal(100, field.GetNext(65));
            Assert.Equal(129, field.GetNext(100));

            Assert.Equal(65, field.GetPrevious(100));
            Assert.Equal(64, field.GetPrevious(65));
            Assert.Equal(63, field.GetPrevious(64));
        }

        [Fact]
        public void EmptyAllowedValues_ContainsNothing()
        {
            var field = new ScheduleField(
                0,
                23,
                ReadOnlySpan<int>.Empty);

            Assert.False(field.Contains(0));
            Assert.False(field.Contains(12));
            Assert.False(field.Contains(23));

            Assert.Null(field.GetNext(0));
            Assert.Null(field.GetPrevious(23));
        }

        [Fact]
        public void Constructor_ThrowsWhenMinIsNegative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new ScheduleField(
                    -1,
                    10,
                    [0]));
        }

        [Fact]
        public void Constructor_ThrowsWhenMaxIsLessThanMin()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new ScheduleField(
                    10,
                    5,
                    [5]));
        }

        [Fact]
        public void Constructor_ThrowsWhenAllowedValueIsOutsideRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new ScheduleField(
                    10,
                    20,
                    [9]));
        }

        [Fact]
        public void Constructor_ThrowsWhenAllowedValueIsAboveRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new ScheduleField(
                    10,
                    20,
                    [21]));
        }

    }
}
