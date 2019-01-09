using System;
using System.Collections.Generic;
using System.Text;

namespace LocationHistoryAnalyzer
{
    public enum ActivityType
    {
        UNKNOWN,
        TILTING,
        STILL,
        ON_FOOT,
        WALKING,
        IN_VEHICLE,
        ON_BICYCLE,
        IN_RAIL_VEHICLE,
        IN_ROAD_VEHICLE,
        RUNNING,
        IN_TWO_WHEELER_VEHICLE,
        IN_FOUR_WHEELER_VEHICLE,
        EXITING_VEHICLE
    }

    public class ActivityEstimate
    {
        public ActivityType Type { get; set; }
        public int Confidence { get; set; }
    }

    public class ActivityEvent
    {
        public string TimestampMs { get; set; }
        public List<ActivityEstimate> Activity { get; set; }
    }

    public class Location
    {
        static System.DateTime baseDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);

        private string _timestampMs;

        public string TimestampMs
        {
            get => _timestampMs;
            set 
            { 
                _timestampMs = value;
                Timestamp = baseDateTime.AddMilliseconds(long.Parse(value)).ToLocalTime();
            }
        }

        public DateTime Timestamp { get; private set; }
        public int LatitudeE7 { get; set; }
        public int LongitudeE7 { get; set; }
        public int Accuracy { get; set; }
        public List<ActivityEvent> Activity { get; set; }
        public int? Velocity { get; set; }
        public int? Heading { get; set; }
        public int? Altitude { get; set; }
        public int? VerticalAccuracy { get; set; }
    }

    public class LocationHistory
    {
        public List<Location> Locations { get; set; }
    }
}
