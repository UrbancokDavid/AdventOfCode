using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace December4
{
    /*
     --- Day 4: Repose Record ---

    You've sneaked into another supply closet - this time, it's across from the prototype suit manufacturing lab. You need to sneak inside and fix the issues with the suit, but there's a guard stationed outside the lab, so this is as close as you can safely get.

    As you search the closet for anything that might help, you discover that you're not the first person to want to sneak in. Covering the walls, someone has spent an hour starting every midnight for the past few months secretly observing this guard post! They've been writing down the ID of the one guard on duty that night - the Elves seem to have decided that one guard was enough for the overnight shift - as well as when they fall asleep or wake up while at their post (your puzzle input).

    For example, consider the following records, which have already been organized into chronological order:

    [1518-11-01 00:00] Guard #10 begins shift
    [1518-11-01 00:05] falls asleep
    [1518-11-01 00:25] wakes up
    [1518-11-01 00:30] falls asleep
    [1518-11-01 00:55] wakes up
    [1518-11-01 23:58] Guard #99 begins shift
    [1518-11-02 00:40] falls asleep
    [1518-11-02 00:50] wakes up
    [1518-11-03 00:05] Guard #10 begins shift
    [1518-11-03 00:24] falls asleep
    [1518-11-03 00:29] wakes up
    [1518-11-04 00:02] Guard #99 begins shift
    [1518-11-04 00:36] falls asleep
    [1518-11-04 00:46] wakes up
    [1518-11-05 00:03] Guard #99 begins shift
    [1518-11-05 00:45] falls asleep
    [1518-11-05 00:55] wakes up

    Timestamps are written using year-month-day hour:minute format. The guard falling asleep or waking up is always the one whose shift most recently started. Because all asleep/awake times are during the midnight hour (00:00 - 00:59), only the minute portion (00 - 59) is relevant for those events.

    Visually, these records show that the guards are asleep at these times:

    Date   ID   Minute
                000000000011111111112222222222333333333344444444445555555555
                012345678901234567890123456789012345678901234567890123456789
    11-01  #10  .....####################.....#########################.....
    11-02  #99  ........................................##########..........
    11-03  #10  ........................#####...............................
    11-04  #99  ....................................##########..............
    11-05  #99  .............................................##########.....

    The columns are Date, which shows the month-day portion of the relevant day; ID, which shows the guard on duty that day; and Minute, which shows the minutes during which the guard was asleep within the midnight hour. (The Minute column's header shows the minute's ten's digit in the first row and the one's digit in the second row.) Awake is shown as ., and asleep is shown as #.

    Note that guards count as asleep on the minute they fall asleep, and they count as awake on the minute they wake up. For example, because Guard #10 wakes up at 00:25 on 1518-11-01, minute 25 is marked as awake.

    If you can figure out the guard most likely to be asleep at a specific time, you might be able to trick that guard into working tonight so you can have the best chance of sneaking in. You have two strategies for choosing the best guard/minute combination.

    Strategy 1: Find the guard that has the most minutes asleep. What minute does that guard spend asleep the most?

    In the example above, Guard #10 spent the most minutes asleep, a total of 50 minutes (20+25+5), while Guard #99 only slept for a total of 30 minutes (10+10+10). Guard #10 was asleep most during minute 24 (on two days, whereas any other minute the guard was asleep was only seen on one day).

    While this example listed the entries in chronological order, your entries are in the order you found them. You'll need to organize them before they can be analyzed.

    What is the ID of the guard you chose multiplied by the minute you chose? (In the above example, the answer would be 10 * 24 = 240.)

     */
    public class Program
    {
        private const string FILENAME = @"..\..\..\input.txt";
        
        public static void Main(string[] args)
        {
            Part1();
            
            Console.ReadKey();
        }

        private static void Part1()
        {
            var log = new List<string>();

            try
            {
                using (var reader = new StreamReader(FILENAME))
                {
                    while (!reader.EndOfStream)
                    {
                        log.Add(reader.ReadLine());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("The file could not be read.");
                Console.WriteLine(ex.Message);
            }

            var comparison = new Comparison<string>(Compare);

            log.Sort(comparison);

            var guards = new List<Guard>();
            var currentId = 0;
            Guard guard = null;

            foreach (var line in log)
            {
                var startPosition = line.IndexOf('#') + 1;

                if (startPosition > 0)
                {
                    currentId = int.Parse(line.Substring(startPosition, line.IndexOf('b') - startPosition));
                    guard = guards.Where(g => g.Id == currentId).FirstOrDefault();

                    if (guard is null)
                    {
                        guard = new Guard(currentId);
                        guards.Add(guard);
                    }
                }
                else
                {
                    var minute = GetMinute(line);
                    var times = guard.Times;
                    times.Add(minute);
                    guard.Times = times;
                }
            }

            guard = guards.First();
            foreach(var g in guards)
            {
                if (g.MinutesAsleep > guard.MinutesAsleep)
                {
                    guard = g;
                }
            }

            Console.WriteLine($"Guard ID: {guard.Id} sleeps the most: {guard.MinutesAsleep} minutes beginning at minutes: {guard.MaxSleep.Value}.");
            Console.WriteLine($"Total: {guard.MinutesAsleep * guard.MaxSleep.Value}");
        }

        private static int Compare(string x, string y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is null && !(y is null))
            {
                return 1;
            }

            if (!(x is null) && y is null)
            {
                return -1;
            }

            if (GetMonth(x) < GetMonth(y))
            {
                return -1;
            }

            if (GetMonth(x) > GetMonth(y))
            {
                return 1;
            }

            if (GetDay(x) < GetDay(y))
            {
                return -1;
            }

            if (GetDay(x) > GetDay(y))
            {
                return 1;
            }

            if (GetMinute(x) < GetMinute(y))
            {
                return -1;
            }

            if (GetMinute(x) > GetMinute(y))
            {
                return 1;
            }

            return 0;
        }

        private static int GetMinute(string x)
        {
            var position = x.IndexOf(':');
            var minuteDigits = x.Substring(position + 1, 2);

            if (int.TryParse(minuteDigits, out var minutes))
            {
                return minutes;
            }

            throw new InvalidDataException("Invalid data type.");
        }

        private static int GetDay(string x)
        {
            var position = x.LastIndexOf('-');
            var dayDigits = x.Substring(position + 1, 2);

            if (int.TryParse(dayDigits, out var day))
            {
                return day;
            }

            throw new InvalidDataException("Invalid data type.");
        }

        private static int GetMonth(string x)
        {
            var position = x.IndexOf('-');
            var monthDigits = x.Substring(position + 1, 2);

            if (int.TryParse(monthDigits, out var month))
            {
                return month;
            }

            throw new InvalidDataException("Invalid data type.");
        }
    }
}
