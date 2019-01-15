using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using GeoCoordinatePortable;
using Newtonsoft.Json;

namespace LocationHistoryAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = args[0];
            var locationHistory = JsonConvert.DeserializeObject<LocationHistory>(File.ReadAllText(filePath));

            double[] targetPos = args[1].Split(',').Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToArray();
            
            var locAdc = new LocationAttendance(locationHistory, new GeoCoordinate(targetPos[0], targetPos[1]));
            var timeBank = new TimeBank(locAdc, TimeSpan.FromHours(8), DateTime.Parse(args[2]));
            
            Console.Write(timeBank.ToText());

            Console.Read();
        }
    }
}
