using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadShet
{
    public class CellValueParser
    {

        public string? ParseCellValue(CellType cellType, string? rawValue, Dictionary<CellAddress, Cell> _cellMap)
        {
            switch (cellType)
            {
                case CellType.Formula:
                    return ParseFormula(rawValue, _cellMap);

                case CellType.Text:
                    return ParseText(rawValue);

                case CellType.Number:
                    return ParseNumbers(rawValue);

                default: return null;
            }
        }

        private string? ParseFormula(string? rawValue, Dictionary<CellAddress, Cell> _cellMap)
        {
            string formulaParsingResult = FormulaProcessor.EvaluateFormula(rawValue, _cellMap);
            return formulaParsingResult;
        }
        private string? ParseText(string? rawValue)
        {
            return rawValue;
        }
        private string? ParseNumbers(string? rawValue)
        {
            return double.TryParse(rawValue, out double number) ? number.ToString() : null;
        }
    }
}
