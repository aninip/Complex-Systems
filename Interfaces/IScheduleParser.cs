using Complex_Systems.Model;

namespace Complex_Systems.Interfaces
{
    internal interface IScheduleParser
    {
        ScheduleDefinition Parse(string scheduleString);
    }
}