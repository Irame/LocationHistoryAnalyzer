using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GeoCoordinatePortable;

namespace LocationHistoryAnalyzer
{
    class LocationAttendance : ReadOnlyCollection<DateTimeSpan>
    {
        private readonly GeoCoordinate _location;

        public LocationAttendance(LocationHistory locationHistory, GeoCoordinate location) : base(new List<DateTimeSpan>())
        {
            _location = location;

            DateTimeSpan curSpan = new DateTimeSpan();
            foreach (var loc in locationHistory.Locations)
            {
                double dist = loc.Coordinates.GetDistanceTo(location);

                if (dist < 500)
                {
                    curSpan.AddDateTime(loc.Timestamp);
                }
                else if (curSpan.IsValid)
                {
                    Items.Add(curSpan);
                    curSpan = new DateTimeSpan();
                }
            }
            if (curSpan.IsValid)
                Items.Add(curSpan);

            ((List<DateTimeSpan>) Items).Sort((s1, s2) => s1.Start.CompareTo(s2.Start));
        }

        public Dictionary<DateTime, List<DateTimeSpan>> GroupByDays()
        {
            return this.GroupBy(span => span.Start.Date).ToDictionary(grouping => grouping.Key, grouping => grouping.ToList());
        }

        public string ToText()
        {
            var sb = new StringBuilder("Attendences per Day").AppendLine();
            foreach (var dayPair in GroupByDays())
            {
                sb.Append($"[{dayPair.Key:ddd dd.MM.yyyy}]: ");
                sb.AppendJoin(",", dayPair.Value.Select(span => $"{span.Start:t} - {span.End:t}"));
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
