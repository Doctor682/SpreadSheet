using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpreadShet
{
    public static class SpreadSheetInputHandler
    {
        private const string SpreadSheetSizePattern = @"^\d+,\d+$";
        private static readonly Regex SpreadSheetSizeRegex = new Regex(SpreadSheetSizePattern);

        public static bool IsInputValid(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            return SpreadSheetSizeRegex.IsMatch(input);
        }
        public static SpreadSheetDimensions ParseDimensions(string? input)
        {
            string[] splitedInput = input.Split(',');
            int columnSize = int.Parse(splitedInput[0]);
            int rowSize = int.Parse(splitedInput[1]);

            return new SpreadSheetDimensions(columnSize, rowSize);
        }
    }
}
