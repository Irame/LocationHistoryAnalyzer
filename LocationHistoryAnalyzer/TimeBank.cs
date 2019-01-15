using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace LocationHistoryAnalyzer
{
    class TimeBank
    {
        public class DayInfo
        {
            public List<DateTimeSpan> Attendances { get; set; }
            public TimeSpan WorkTime { get; set; }
            public TimeSpan TimeBalance { get; set; }
        }

        private LocationAttendance _locationAttendance;

        public TimeSpan DailyQuota { get; private set; }

        private Dictionary<DateTime, DayInfo> _dayInfos;
        private IReadOnlyDictionary<DateTime, DayInfo> DayInfos => _dayInfos;

        public TimeBank(LocationAttendance locationAttendance, TimeSpan dailyQuota)
            : this(locationAttendance, dailyQuota, DateTime.MinValue)
        {}

        public TimeBank(LocationAttendance locationAttendance, TimeSpan dailyQuota, DateTime since)
        {
            _locationAttendance = locationAttendance;
            DailyQuota = dailyQuota;
            _dayInfos = new Dictionary<DateTime, DayInfo>();

            TimeSpan curBalance = TimeSpan.Zero;
            foreach (var dayPair in locationAttendance.GroupByDays())
            {
                if (dayPair.Key < since)
                    continue;
                
                DayInfo curInfo = new DayInfo();
                curInfo.Attendances = dayPair.Value;
                curInfo.WorkTime = dayPair.Value.Select(span => span.Span).Aggregate(TimeSpan.Zero, (sum, part) => sum + part);
                curBalance += curInfo.WorkTime - dailyQuota;
                curInfo.TimeBalance = curBalance;
                
                _dayInfos.Add(dayPair.Key, curInfo);
            }
        }

        public string ToText()
        {
            var sb = new StringBuilder($"TimeBank Cource with a daily quota of {DailyQuota:t}h").AppendLine();
            foreach (var dayPair in _dayInfos.OrderByDescending(pair => pair.Key))
            {
                sb.Append($"[{dayPair.Key:ddd dd.MM.yyyy}]: {dayPair.Value.WorkTime:hh\\:mm}h ");
                sb.Append($"<{(dayPair.Value.TimeBalance < TimeSpan.Zero ? "-" : "+")}{dayPair.Value.TimeBalance:hh\\:mm}h>");
                sb.Append(" (");
                sb.AppendJoin(", ", dayPair.Value.Attendances.Select(span => $"{span.Start:t} - {span.End:t}"));
                sb.AppendLine(")");
            }

            return sb.ToString();
        }
    }
}
