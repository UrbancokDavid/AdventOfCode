using System;
using System.Collections.Generic;
using System.Text;

namespace December4
{
    public class Guard
    {
        private List<int> _times;
        private List<KeyValuePair<int, int>> _startTimes;

        public Guard(int id)
        {
            Id = id;
            Times = new List<int>();
            _times = new List<int>();
            _startTimes = new List<KeyValuePair<int, int>>();
        }

        public int Id { get; }

        public int MinutesAsleep { get; set; }

        public KeyValuePair<int, int> MaxSleep { get; set; }

        public List<int> Times
        {
            get => _times;

            set
            {
                if (value.Count % 2 == 0)
                {
                    var position = value.Count - 1;
                    while (position > 0)
                    {
                        var minutes = value[position] - value[position - 1];

                        MinutesAsleep += minutes;
                        var startTimes = StartTimes;
                        startTimes.Add(new KeyValuePair<int, int>(minutes, value[position - 1]));
                        StartTimes = startTimes;
                        position -= 2;
                    }
                }

                _times = value;
            }
        }

        private List<KeyValuePair<int, int>> StartTimes
        {
            get => _startTimes;

            set
            {
                var maxSleep = new KeyValuePair<int, int>(0, 0);

                foreach (var startTime in value)
                {
                    if (startTime.Key > maxSleep.Key)
                    {
                        maxSleep = startTime;
                    }
                }

                MaxSleep = maxSleep;
                _startTimes = value;
            }
        }
    }
}
