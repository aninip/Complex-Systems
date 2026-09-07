namespace Complex_Systems;

public static class Program
{
    public static void Main()
    {
        var schedule = new Schedule("*.9.*/2 1-5 10:00:00.000");

        DateTime time = DateTime.Now;
        //DateTime time = new DateTime(2026, 9, 5, 12, 0, 0);

        schedule.NearestEvent(time);
        schedule.NearestPrevEvent(time);
        schedule.NextEvent(time);
        schedule.PrevEvent(time);
    }
}