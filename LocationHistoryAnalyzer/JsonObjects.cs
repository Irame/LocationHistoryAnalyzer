using System;
using System.Collections.Generic;
using System.Text;
using GeoCoordinatePortable;
using Newtonsoft.Json;

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
        [JsonConstructor]
        private Location()
        {
            Coordinates = new GeoCoordinate();
        }

        static System.DateTime baseDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
        
        [JsonProperty]
        private string TimestampMs
        {
            set => Timestamp = baseDateTime.AddMilliseconds(long.Parse(value)).ToLocalTime();
        }
        
        [JsonProperty]
        public int LatitudeE7
        {
            set => Coordinates.Latitude = value / 10000000.0;
        }
        
        [JsonProperty]
        public int LongitudeE7
        {
            set => Coordinates.Longitude = value / 10000000.0;
        }
        
        [JsonProperty]
        public int Accuracy { get; private set; }

        [JsonProperty]
        public List<ActivityEvent> Activity { get; private set; }

        [JsonProperty]
        public int? Velocity { get; private set; }

        [JsonProperty]
        public int? Heading { get; private set; }

        [JsonProperty]
        public int? Altitude { get; private set; }

        [JsonProperty]
        public int? VerticalAccuracy { get; private set; }
        
        
        public GeoCoordinatePortable.GeoCoordinate Coordinates { get; private set; }
        public DateTime Timestamp { get; private set; }
    }

    public class LocationHistory
    {
        public List<Location> Locations { get; set; }
    }
}
