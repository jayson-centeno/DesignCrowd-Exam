using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesignCrowd.Exam
{
    public class BusinessDayCounter
    {
        public int WeekdaysBetweenTwoDates(DateTime firstDate, DateTime secondDate)
        {
            int result = CalculateWeekdaysBetweenTwoDates(firstDate, secondDate);

            if (result > 0) {
                //Remove 1 if hit weekdays
                if (firstDate.Date.DayOfWeek != DayOfWeek.Saturday &&
                    firstDate.Date.DayOfWeek != DayOfWeek.Sunday) {
                    result--;
                }

                //Remove 1 if hit weekdays
                if (secondDate.Date.DayOfWeek != DayOfWeek.Saturday &&
                    secondDate.Date.DayOfWeek != DayOfWeek.Sunday) {
                    result--;
                }
            }

            return result;
        }

        public int BusinessDaysBetweenTwoDates(DateTime firstDate, DateTime secondDate, IList<DateTime> publicHolidays)
        {
            int result = CalculateWeekdaysBetweenTwoDates(firstDate, secondDate);

            if (result > 0) {

                var datesToRemove = publicHolidays;

                //Remove 1 if hit weekdays
                if (firstDate.Date.DayOfWeek != DayOfWeek.Saturday &&
                    firstDate.Date.DayOfWeek != DayOfWeek.Sunday) {
                    datesToRemove.Add(firstDate);
                }

                //Remove 1 if hit weekdays
                if (secondDate.Date.DayOfWeek != DayOfWeek.Saturday &&
                    secondDate.Date.DayOfWeek != DayOfWeek.Sunday) {
                    datesToRemove.Add(secondDate);
                }

                datesToRemove = datesToRemove.Distinct().ToList();

                foreach (DateTime date in datesToRemove) {
                    DateTime hDate = date.Date;
                    if (firstDate <= hDate && hDate <= secondDate)
                        result--;
                }

            }

            return result;
        }

        public int BusinessDaysBetweenTwoDates(DateTime firstDate, DateTime secondDate, IList<HolidayRules> publicHolidaysRules)
        {
            int result = CalculateWeekdaysBetweenTwoDates(firstDate, secondDate);

            if (result > 0) {

                var datesToRemove = new List<DateTime>();

                //Remove 1 if hit weekdays
                if (firstDate.Date.DayOfWeek != DayOfWeek.Saturday &&
                    firstDate.Date.DayOfWeek != DayOfWeek.Sunday) {
                    datesToRemove.Add(firstDate);
                }

                //Remove 1 if hit weekdays
                if (secondDate.Date.DayOfWeek != DayOfWeek.Saturday &&
                    secondDate.Date.DayOfWeek != DayOfWeek.Sunday) {
                    datesToRemove.Add(secondDate);
                }

                var years = new List<int>(new[] { firstDate.Year, secondDate.Year });
                int diff = secondDate.Year - firstDate.Year;

                //fill the missing years
                if (diff > 1) {
                    var missing = Enumerable.Range(firstDate.Year, diff).Except(years);
                    years.AddRange(missing);
                    years = years.OrderBy(x => x).ToList();
                }

                //remove duplicate if first and second are the same
                years = years.Distinct().ToList();
                years.ForEach(year => {

                    publicHolidaysRules.ToList().ForEach(rule => {

                        if (rule.Day > 0) {

                            var pRule = rule as IExactRule;

                            var hDate = new DateTime(year, pRule.Month, pRule.Day);

                            if (hDate < firstDate) return;

                            if ((hDate.DayOfWeek == DayOfWeek.Saturday || hDate.DayOfWeek == DayOfWeek.Sunday) &&
                                pRule.IfWeekEndMoveToNextModay) {
                                pRule.Day += hDate.DayOfWeek == DayOfWeek.Saturday ? 2 : 1;
                                hDate = new DateTime(year, pRule.Month, pRule.Day);
                            }

                            datesToRemove.Add(hDate);

                        }
                        else {

                            var pRule = rule as IOccuringRule;
                            var hDate = new DateTime(year, pRule.Month, 01);
                            var endDate = hDate.AddMonths(1).AddDays(-1);

                            var listWholeDates = new List<int>() { hDate.Day, endDate.Day };
                            int diff = endDate.Day - hDate.Day;
                            var missing = Enumerable.Range(hDate.Day, diff).Except(listWholeDates);

                            listWholeDates.AddRange(missing);
                            listWholeDates = listWholeDates.OrderBy(x => x).ToList();

                            var dates = listWholeDates.Select(d => new DateTime(year, pRule.Month, d));
                            dates = dates.Where(d => d.DayOfWeek == DayOfWeek.Monday).Take(rule.Week).ToList();
                            var targetDate = dates.LastOrDefault();

                            datesToRemove.Add(targetDate);

                        }

                    });

                });

                datesToRemove = datesToRemove.Distinct().ToList();
                datesToRemove.ForEach(date => {
                    DateTime hDate = date.Date;
                    if (firstDate <= hDate && hDate <= secondDate)
                        result--;
                });

            }

            return result;
        }

        private int CalculateWeekdaysBetweenTwoDates(DateTime firstDate, DateTime secondDate)
        {
            int result = 0;

            if (secondDate.Date <= firstDate.Date) {
                return result;
            }

            //Compute total days
            int totalNoOfDays = 1 + Convert.ToInt32((secondDate - firstDate).TotalDays);

            //Count the no of weekends
            int nsaturdays = (totalNoOfDays + Convert.ToInt32(firstDate.DayOfWeek)) / 7;

            result += totalNoOfDays - (2 * (nsaturdays - ((firstDate.DayOfWeek == DayOfWeek.Sunday ? 1 : 0) + (secondDate.DayOfWeek == DayOfWeek.Saturday ? 1 : 0))));

            return result;
        }
    }
}
