using System;
using System.Collections.Generic;
using System.Text;

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
}
