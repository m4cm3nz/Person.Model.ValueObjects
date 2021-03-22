﻿using System;
using System.Text.RegularExpressions;

namespace Person.Model.ValueObjects
{
    public struct MonthOfYear
    {
        public MonthOfYear(string monthYear)
        {
            var numericMonthYear =  Regex.Replace(monthYear, "[^.0-9]", "");

            if (!Regex.IsMatch(numericMonthYear, @"(0[1-9]|1[0-2])(19|2[0-1])\d{2}$"))
                throw new ArgumentException("Invalid month/year");

            Month = short.Parse(numericMonthYear[0..2]);
            Year = short.Parse(numericMonthYear[2..6]);
        }

        public MonthOfYear(string month, string year)
        {
            if (!IsValid(month, year))
                throw new ArgumentException("MonthOfYear should be a valid month/year value pair argument.");

            Month = short.Parse(month);
            Year = short.Parse(year);
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

        public static bool IsValid(string month, string year)
        {
            var validMonth = DateTime.TryParse($"{DateTime.Today.Year}-{month}-01", out _);
            var validYear = DateTime.TryParse($"{year}-{month}-01", out _);

            return validMonth && validYear;
        }
    }
}