namespace Complex_Systems.Model
{
    internal sealed class ScheduleDefinition
    {
        public ScheduleField Years { get; }
        public ScheduleField Months { get; }
        public ScheduleField Days { get; }
        public ScheduleField Weekdays { get; }
        public ScheduleField Hours { get; }
        public ScheduleField Minutes { get; }
        public ScheduleField Seconds { get; }
        public ScheduleField Milliseconds { get; }
    }
}
