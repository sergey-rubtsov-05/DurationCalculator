using System;
using System.Collections.Generic;
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
    }

    public class Calculator
    {
        private readonly TimeSpan _lunchStartTime = 13.OfHours();
        private readonly TimeSpan _lunchEndTime = 14.OfHours();
        private readonly TimeSpan _lunchDuration;

        public Calculator()
        {
            _lunchDuration = _lunchEndTime - _lunchStartTime;
        }

        public TimeSpan GetDuration(DateTime startTime, DateTime endTime)
        {
            var duration = endTime - startTime;

            if (IsContainsLunch(startTime, endTime))
            {
                duration = duration - _lunchDuration;
            }

            return duration;
        }

        private bool IsContainsLunch(DateTime startTime, DateTime endTime)
        {
            return startTime.TimeOfDay < _lunchStartTime && endTime.TimeOfDay > _lunchEndTime;
        }
    }

    public static class DurationExtensions
    {
        public static TimeSpan OfHours(this int hours)
        {
            return new TimeSpan(0, hours, 0, 0);
        }
    }
}
