using Complex_Systems.Interfaces;
using Complex_Systems.Model;

namespace Complex_Systems.Parsing
{
    /// <summary>
    /// Отвечает  за превращение строки в структуру расписания.
    /// </summary>
    internal sealed class ScheduleParser : IScheduleParser
    {
        public ScheduleDefinition Parse(string scheduleString)
        {
            return new ScheduleDefinition();
        }
    }
}
