using Complex_Systems.Model;

namespace Complex_Systems.Searching
{
    /// <summary>
    /// Класс выполняющий поиск следующего допустимого
    /// </summary>
    internal sealed class ScheduleSearcher
    {
        //перескакивание сразу к следующему разрешённому компоненту
        //public DateTime FindNext(
        //    DateTime value,
        //    ScheduleDefinition schedule)
        //{
        //    //...
        //}
        ////перескакивание сразу к следующему разрешённому компоненту
        //public DateTime FindPrevious(
        //    DateTime value,
        //    ScheduleDefinition schedule)
        //{
        //    //...
        //}
        //И ещё: 32 не нужно превращать в 28/29/30/31 внутри ScheduleField. Это принципиально, потому что ScheduleField ничего не знает о конкретном месяце. Он только хранит допустимые значения.
        private static bool IsDayAllowed(
    DateTime date,
    ScheduleDefinition schedule)
        {
            if (schedule.Days.Contains(date.Day))
                return true;

            return schedule.Days.Contains(32)
                && date.Day == DateTime.DaysInMonth(date.Year, date.Month);
        }
    }
}
