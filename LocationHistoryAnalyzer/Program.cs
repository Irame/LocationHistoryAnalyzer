using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace LocationHistoryAnalyzer
{
    class DateTimeSpan
    {
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }
        public TimeSpan Span => End - Start;
        public bool IsValid { get; private set; }

        public void AddDateTime(DateTime dateTime)
        {
            if (!IsValid)
            {
                Start = dateTime;
                End = dateTime;
                IsValid = true;
                return;
            }

            if (dateTime < Start)
                Start = dateTime;
            else
                End = dateTime;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string filePath = args[0];
            var locationHistory = JsonConvert.DeserializeObject<LocationHistory>(File.ReadAllText(filePath));

            double[] targetPos = args[1].Split(',').Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToArray();
            double targetLat = targetPos[0];
            double targetLon = targetPos[1];

            Dictionary<DateTime, List<DateTimeSpan>> timeSpansPerDay = new Dictionary<DateTime, List<DateTimeSpan>>();
            
            DateTimeSpan curSpan = new DateTimeSpan();
            foreach (var location in locationHistory.Locations)
            {
                double latDiff = targetLat - location.LatitudeE7/10000000.0;
                double lonDiff = targetLon - location.LongitudeE7/10000000.0;

                double dist = Math.Sqrt(latDiff * latDiff + lonDiff * lonDiff);
                
                var day = location.Timestamp.Date;

                if (dist < 0.005)
                {
                    curSpan.AddDateTime(location.Timestamp);
                }
                else if (curSpan.IsValid)
                {
                    if (!timeSpansPerDay.TryGetValue(day, out List<DateTimeSpan> timeSpans))
                    {
                        timeSpans = new List<DateTimeSpan>();
                        timeSpansPerDay.Add(day, timeSpans);
                    }
                    timeSpans.Add(curSpan);
                    curSpan = new DateTimeSpan();
                }
            }

            TimeSpan timeBank = TimeSpan.Zero;
            List<string> lines = new List<string>();
            foreach (var time in timeSpansPerDay.Reverse())
            {
                if (!IsValidDay(time.Key))
                    continue;

                timeBank -= TimeSpan.FromHours(8);
                    
                TimeSpan dayWorkTime = time.Value.Select(x => x.Span).Aggregate((sum, span) => sum + span);
                
                timeBank += dayWorkTime;

                string s = $"[{time.Key:ddd dd.MM.yyyy}]: {dayWorkTime:hh\\:mm} <{(timeBank < TimeSpan.Zero ? "-" : "+")}{timeBank:hh\\:mm}> "
                         + $"({string.Join(", ", time.Value.OrderBy(x => x.Start).Select(x => x.Start.ToString("t") + " - " + x.End.ToString("t")))})";
                
                Console.WriteLine(s);
                lines.Add(s);
            }

            File.WriteAllLines("Output.txt", lines);


            Console.Read();
        }

        private static bool IsValidDay(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                return false;
            if (date < new DateTime(2018, 12,  1, 0, 0, 0, DateTimeKind.Local))
                return false;
            if (   date >= new DateTime(2018, 12, 24, 0, 0, 0, DateTimeKind.Local)
                && date <= new DateTime(2019,  1,  2, 0, 0, 0, DateTimeKind.Local))
                return false;
            return true;
        }
    }
}
