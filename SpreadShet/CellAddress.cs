using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpreadShet
{
    public struct CellAddress
    {
        public int Row { get; }
        public string Column { get; }
        public CellAddress(string column, int row)
        {
            Column = column;
            Row = row;
        }
        public static CellAddress Parse(string cellAddress)
        {
            if (string.IsNullOrWhiteSpace(cellAddress))
                throw new ArgumentException("Address is null or empty.");

            Match match = Regex.Match(cellAddress, @"^([A-Z]+)(\d+)$");
            if (!match.Success)
                throw new ArgumentException("Invalid cell address format. Expected format like 'A1', 'B2', 'AA10'.");

            string column = match.Groups[1].Value;
            int row = int.Parse(match.Groups[2].Value);

            return new CellAddress(column, row);
        }
        public override string ToString()
        {
            return $"{(Column)}{Row}";
        }
    }
}
