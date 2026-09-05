namespace Complex_Systems.Model
{
    /// <summary> 
    /// Представляет разобранное расписание в виде готовых правил,
    /// необходимых для поиска подходящих моментов времени. 
    /// </summary>
    internal sealed class ScheduleDefinition(
        ScheduleField years,
        ScheduleField months,
        ScheduleField days,
        ScheduleField weekdays,
        ScheduleField hours,
        ScheduleField minutes,
        ScheduleField seconds,
        ScheduleField milliseconds)
    {
        public ScheduleField Years => years;
        public ScheduleField Months => months;
        public ScheduleField Days => days;
        public ScheduleField Weekdays => weekdays;
        public ScheduleField Hours => hours;
        public ScheduleField Minutes => minutes;
        public ScheduleField Seconds => seconds;
        public ScheduleField Milliseconds => milliseconds;
    }
}
