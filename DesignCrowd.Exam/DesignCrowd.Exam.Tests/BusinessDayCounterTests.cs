using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace DesignCrowd.Exam.Tests
{
    [TestFixture]
    public class BusinessDayCounterTests
    {
        [Test]
        [TestCase("10/07/2013", "10/08/2013", 0)]
        [TestCase("10/07/2013", "10/09/2013", 1)]
        [TestCase("10/05/2013", "10/14/2013", 5)]
        [TestCase("10/07/2013", "01/01/2014", 61)]
        [TestCase("10/07/2013", "10/05/2013", 0)]
        public void WeekdaysBetweenTwoDates_With_valid_data_Should_return_weekdays(DateTime startDate, DateTime endDate, int expected)
        {
            var businessDayCounter = new BusinessDayCounter();
            int result = businessDayCounter.WeekdaysBetweenTwoDates(startDate, endDate);
            Assert.IsTrue(result == expected);
        }

        private static TestData[] _testData = new[]{
            new TestData() {
                StartDate = new DateTime(2013, 10, 07),
                EndDate = new DateTime(2013, 10, 09),
                Holidays = new List<DateTime>() {
                    new DateTime(2013, 12, 25),
                    new DateTime(2013, 12, 26),
                    new DateTime(2014, 01, 01)
                },
                ExpectedResult = 1
            },
            new TestData() {
                StartDate = new DateTime(2013, 12, 24),
                EndDate = new DateTime(2013, 12, 27),
                Holidays = new List<DateTime>() {
                    new DateTime(2013, 12, 25),
                    new DateTime(2013, 12, 26),
                    new DateTime(2014, 01, 01)
                },
                ExpectedResult = 0
            },
            new TestData() {
                StartDate = new DateTime(2013, 10, 07),
                EndDate = new DateTime(2014, 01, 01),
                Holidays = new List<DateTime>() {
                    new DateTime(2013, 12, 25),
                    new DateTime(2013, 12, 26),
                    new DateTime(2014, 01, 01)
                },
                ExpectedResult = 59
            }
        };

        [Test]
        public void BusinessDaysBetweenTwoDates_With_valid_data_Should_return_weekdays([ValueSource("_testData")] TestData testData)
        {
            var businessDayCounter = new BusinessDayCounter();
            int result = businessDayCounter.BusinessDaysBetweenTwoDates(testData.StartDate, testData.EndDate, testData.Holidays);
            Assert.IsTrue(result == testData.ExpectedResult);
        }

        private static TestData[] _testDataHolidayRules = new[]{
            new TestData() {
                StartDate = new DateTime(2011, 12, 01),
                EndDate = new DateTime(2012, 10, 09),
                Rules = new List<HolidayRules>() {
                    new HolidayRules {
                        Day = 25,
                        Month = 04
                    },
                    new HolidayRules {
                        Day = 01,
                        Month = 01,
                        IfWeekEndMoveToNextModay = true
                    },
                    new HolidayRules {
                        DayOfWeek = DayOfWeek.Monday,
                        Week = 2,
                        Month = 06
                    },
                },
                ExpectedResult = 219
            },
        };

        [Test]
        public void BusinessDaysBetweenTwoDates_With_valid_holidaRules_Should_return_weekdays([ValueSource("_testDataHolidayRules")] TestData testData)
        {
            var businessDayCounter = new BusinessDayCounter();
            int result = businessDayCounter.BusinessDaysBetweenTwoDates(testData.StartDate, testData.EndDate, testData.Rules);
            Assert.IsTrue(result == testData.ExpectedResult);
        }
    }

    public class TestData
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<DateTime> Holidays { get; set; }
        public List<HolidayRules> Rules { get; set; }
        public int ExpectedResult { get; set; }
    }

}
