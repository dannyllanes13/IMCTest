using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace IMCTest.Service.Validators
{
    public class RegEx
    {
        private const string UsZipRegEx = @"^\d{5}(?:[-\s]\d{4})?$";

        private const string IsoTwoLettersRegEx = @"^[A-Z]{2}$";

        public static bool isValidZipCode(string zipCode)
        {
            return Regex.Match(zipCode, UsZipRegEx).Success;
        }

        public static bool IsValidCountry(string country)
        {
            return Regex.Match(country, IsoTwoLettersRegEx).Success;
        }
    }
}
