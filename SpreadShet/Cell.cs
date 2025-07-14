using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadShet
{
    public class Cell
    {
        public string? RawValue {  get; private set; }
        public string? ParsedValue {  get; private set; }
        public CellAddress Address { get; }
        public CellType CellType { get; private set; }

        public Cell(CellAddress cellAddress, string? rawValue)
        {
            Address = cellAddress;
            RawValue = rawValue;
            CellType = DetermineCellType(RawValue);
            ParsedValue = CheckingForSimpleValues(CellType, RawValue);
        }
        public void Evaluate(CellValueParser parser, Dictionary<CellAddress, Cell> cellMap)
        {
            ParsedValue = parser.ParseCellValue(CellType, RawValue, cellMap);
        }
        public string? CheckingForSimpleValues(CellType CellType, string? RawValue)
        {
            if (CellType != CellType.Formula) return RawValue;
            return null;
        }
        public CellType DetermineCellType(string? rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return CellType.Empty;

            if (rawValue.StartsWith('='))
                return CellType.Formula;

            if (double.TryParse(rawValue, out _))
                return CellType.Number;

            return CellType.Text;
        }
    }
}
