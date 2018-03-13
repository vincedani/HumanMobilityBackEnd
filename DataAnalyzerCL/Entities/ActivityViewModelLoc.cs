using System;

namespace DataAnalyzer.Entities
{
    public class ActivityViewModelLoc
    {
        public long ID { get; set; }

        public string UserId { get; set; }

        public DateTime SaveTime { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }
    }
}
