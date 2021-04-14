using System;
using System.Text.RegularExpressions;

namespace Person.Model.ValueObjects
{
    [Obsolete("Soon this will be moved to new library DateTime.Model.ValueObjects", false)]
    public struct MonthOfYear
    {
        public MonthOfYear(string monthYear)
        {
            var numericMonthYear =  Regex.Replace(monthYear, "[^.0-9]", "");

            if (!IsValid(numericMonthYear))
                throw new ArgumentException("MonthOfYear should be a valid month/year value pair argument.");

            Month = short.Parse(numericMonthYear[0..2]);
            Year = short.Parse(numericMonthYear[2..6]);
        }

        public short Month { get; private set; }
        public short Year { get; private set; }
        
        public static implicit operator int(MonthOfYear monthOfYear) =>
            int.Parse($"{monthOfYear.Year}{monthOfYear.Month:00}");

        public static implicit operator string(MonthOfYear monthOfYear) =>
            monthOfYear.ToString();

        public static implicit operator MonthOfYear(string monthYear)
        {
            return new MonthOfYear(monthYear);
        }

        public override string ToString()
        {
            return $"{Month:00}/{Year}";
        }

        public static bool IsValid(string monthYear)
        {
            return Regex.IsMatch(monthYear, @"(0[1-9]|1[0-2])(19|2[0-1])\d{2}$");
        }
    }
}
