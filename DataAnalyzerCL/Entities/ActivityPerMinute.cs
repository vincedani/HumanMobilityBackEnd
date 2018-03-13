using System;

namespace DataAnalyzer.Entities
{
    public class ActivityPerMinute
    {
        #region Members

        public DateTime DateTime { get; }
        public double Activity { get; private set; }
        private int N { get; set; }

        #endregion

        #region Constructors
        
        public ActivityPerMinute(DateTime dateTime, double activity)
        {
            Activity = activity;
            DateTime = dateTime;
            N = 1;
        }

        #endregion

        #region Helpers

        public void AddActivity(double activity)
        {
            N++;
            Activity += activity;
        }

        public void Normalize()
        {
            Activity = Activity / N;
        }

        #endregion
    }
}
