using System;
using System.Collections.Generic;
using System.Text;

namespace DesignCrowd.Exam
{
    public class HolidayRules : IExactRule, IOccuringRule
    {
        public int Day { get; set; }
        public bool IfWeekEndMoveToNextModay { get; set; }
        public int Month { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int Week { get; set; }
    }

    public interface IExactRule {
        public int Day { get; set; }
        public bool IfWeekEndMoveToNextModay { get; set; }
        public int Month { get; set; }
    }

    public interface IOccuringRule
    {
        public DayOfWeek DayOfWeek { get; set; }
        public int Week { get; set; }
        public int Month { get; set; }
    }
}