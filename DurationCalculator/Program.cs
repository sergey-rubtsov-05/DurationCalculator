using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DurationCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var calculator = new Calculator();
            
            var count = 100000;
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                var startTime = new DateTime(2018, 12, 14, 09, 00, 00, DateTimeKind.Utc);
                var endTime = new DateTime(2018, 12, 17, 18, 00, 00, DateTimeKind.Utc);

                var duration = calculator.GetDuration(startTime, endTime);
            }
            stopwatch.Stop();

            Console.WriteLine($"{count} of calculating: {stopwatch.Elapsed}");
            Console.Read();
        }
    }

    [TestFixture]
    public class Tests
    {
        [Test]
        public void DiffBetweenStartAndEnd()
        {
            var calculator = GetCalculator();
            var startTime = new DateTime(2018, 12, 10, 09, 00, 00, DateTimeKind.Utc);
            var endTime = new DateTime(2018, 12, 10, 13, 00, 00, DateTimeKind.Utc);

            var duration = calculator.GetDuration(startTime, endTime);

            Assert.AreEqual(4.OfHours(), duration);
        }

        private static Calculator GetCalculator()
        {
            return new Calculator();
        }

        [Test]
        public void DurationExcludeLunchTime()
        {
            var calculator = GetCalculator();
            var startTime = new DateTime(2018, 12, 10, 09, 00, 00, DateTimeKind.Utc);
            var endTime = new DateTime(2018, 12, 10, 18, 00, 00, DateTimeKind.Utc);

            var duration = calculator.GetDuration(startTime, endTime);

            Assert.AreEqual(8.OfHours(), duration);
        }

        [Test]
        public void DurationExcludeLunchTimeAndEndOnNextDay()
        {
            var calculator = GetCalculator();
            var startTime = new DateTime(2018, 12, 10, 09, 00, 00, DateTimeKind.Utc);
            var endTime = new DateTime(2018, 12, 11, 18, 00, 00, DateTimeKind.Utc);

            var duration = calculator.GetDuration(startTime, endTime);

            Assert.AreEqual(16.OfHours(), duration);
        }

        [Test]
        public void DurationExcludeWeekend()
        {
            var calculator = GetCalculator();
            var startTime = new DateTime(2018, 12, 14, 09, 00, 00, DateTimeKind.Utc);
            var endTime = new DateTime(2018, 12, 17, 18, 00, 00, DateTimeKind.Utc);

            var duration = calculator.GetDuration(startTime, endTime);

            Assert.AreEqual(16.OfHours(), duration);
        }
    }

    public class Calculator
    {
        private readonly Period[] _workPeriods = new[]
            {new Period(09.OfHours(), 13.OfHours()), new Period(14.OfHours(), 18.OfHours())};

        private readonly DayOfWeek[] _workDays = new[]
            {DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday};

        public TimeSpan GetDuration(DateTime startTime, DateTime endTime)
        {
            TimeSpan duration = new TimeSpan();

            var nextMinute = startTime;
            while (nextMinute <= endTime)
            {
                if (_workDays.Contains(nextMinute.DayOfWeek))
                {
                    foreach (var workPeriod in _workPeriods)
                    {
                        if (workPeriod.Contains(nextMinute.TimeOfDay))
                        {
                            duration += 1.OfMinutes();
                            break;
                        }
                    }
                }

                nextMinute = nextMinute + 1.OfMinutes();
            }

            return duration;
        }
    }

    public static class DurationExtensions
    {
        public static TimeSpan OfHours(this int hours)
        {
            return new TimeSpan(0, hours, 0, 0);
        }

        public static TimeSpan OfMinutes(this int minutes)
        {
            return new TimeSpan(0, 0, minutes, 0);
        }
    }

    public class Period
    {
        public Period(TimeSpan fromInclude, TimeSpan toExclude)
        {
            FromInclude = fromInclude;
            ToExclude = toExclude;
        }

        public TimeSpan FromInclude { get; }
        public TimeSpan ToExclude { get; }

        public bool Contains(TimeSpan time)
        {
            if (time >= FromInclude & time < ToExclude)
                return true;

            return false;
        }
    }
}
