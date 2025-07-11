using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadShet
{
    public class FormulaProcessor
    {
        private const string ErrorValue = "err!";
        public static char[] GetAllOperatorSymbols()
        {
            return new[] { '+', '-', '*', '/' };
        }
        public static string EvaluateFormula(string? rawFormula, Dictionary<CellAddress, Cell> _cellMap)
        {

            if (string.IsNullOrEmpty(rawFormula))
                return ErrorValue;

            string formula = rawFormula[1..];
            var checkedCellsValues = CheckCellValuesConsistency(formula, _cellMap);
            if(checkedCellsValues.error == ErrorValue)
            {
                return ErrorValue;       
            }  

            switch (checkedCellsValues.type)
            {
                case FormulaType.Text:
                    return EvaluateTextFormula(formula, _cellMap);

                case FormulaType.Numeric:
                    return EvaluateNumericFormula(formula, _cellMap);

                default:
                    return ErrorValue;
            }
        }

        public static (FormulaType type, string error) CheckCellValuesConsistency(string formula, Dictionary<CellAddress, Cell> _cellMap)
        {
            var (containsNumbers, containsText, error) = AnalyzeFormulaComponents(formula, _cellMap);
            if (error != string.Empty)
                return (FormulaType.Invalid, error);

            return DetermineFormulaType(containsNumbers, containsText);
        }

        private static (bool numbers, bool text, string error) AnalyzeFormulaComponents(string formula, Dictionary<CellAddress, Cell> _cellMap)
        {
            bool containsNumbers = false;
            bool containsText = false;

            foreach (var address in ExtractCellAddresses(formula))
            {
                var (isNumber, isText, error) = AnalyzeCell(address, _cellMap);
                if (error != string.Empty)
                    return (false, false, error);

                containsNumbers |= isNumber;
                containsText |= isText;
            }

            return (containsNumbers, containsText, string.Empty);
        }

        private static IEnumerable<string> ExtractCellAddresses(string formula)
        {
            return formula.Split(GetAllOperatorSymbols(), StringSplitOptions.RemoveEmptyEntries)
                          .Select(a => a.Trim());
        }

        private static (bool isNumber, bool isText, string error) AnalyzeCell(string address, Dictionary<CellAddress, Cell> _cellMap)
        {
            if (double.TryParse(address, out _))
            {
                return (true, false, string.Empty);
            }

            try
            {
                var cellAddr = CellAddress.Parse(address);
                if (!_cellMap.TryGetValue(cellAddr, out Cell? cell))
                    return (false, false, string.Empty);

                return AnalyzeCellContent(cell, _cellMap);
            }
            catch (ArgumentException)
            {
                return (false, false, ErrorValue);
            }
        }

        private static (bool isNumber, bool isText, string error) AnalyzeCellContent(Cell cell, Dictionary<CellAddress, Cell> _cellMap)
        {
            switch (cell.CellType)
            {
                case CellType.Number:
                    return (true, false, string.Empty);

                case CellType.Text:
                    return (false, true, string.Empty);

                case CellType.Formula:
                    return AnalyzeFormulaCell(cell, _cellMap);

                case CellType.Empty:
                    return (false, false, string.Empty);

                default:
                    return (false, false, ErrorValue);
            }
        }

        private static (bool isNumber, bool isText, string error) AnalyzeFormulaCell(Cell cell, Dictionary<CellAddress, Cell> cellMap)
        {
            if (cell.RawValue == null)
                return (false, false, string.Empty);

            var (nestedType, nestedError) = CheckCellValuesConsistency(cell.RawValue[1..], cellMap);
            if (nestedError == ErrorValue)
                return (false, false, ErrorValue);

            return nestedType switch
            {
                FormulaType.Numeric => (true, false, string.Empty),
                FormulaType.Text => (false, true, string.Empty),
                _ => (false, false, string.Empty)
            };
        }

        private static (FormulaType type, string error) DetermineFormulaType(bool containsNumbers, bool containsText)
        {
            if (containsNumbers && containsText)
                return (FormulaType.Invalid, ErrorValue);

            if (containsNumbers)
                return (FormulaType.Numeric, string.Empty);

            if (containsText)
                return (FormulaType.Text, string.Empty);

            return (FormulaType.Invalid, string.Empty);
        }

        public static string EvaluateNumericFormula(string formula, Dictionary<CellAddress, Cell> _cellMap)
        {
            try
            {
                var tokens = TokenizeFormula(formula);
                var values = new List<double>();
                var operators = new List<string>();

                for (int i = 0; i < tokens.Count; i++)
                {
                    if (IsOperator(tokens[i]))
                    {
                        if (tokens[i] == "*" || tokens[i] == "/")
                        {
                            double left = values[^1];
                            double right = GetNumericValue(tokens[i + 1], _cellMap);
                            values[^1] = tokens[i] == "*" ? left * right : left / right;
                            i++;
                        }
                        else
                        {
                            operators.Add(tokens[i]);
                        }
                    }
                    else
                    {
                        values.Add(GetNumericValue(tokens[i], _cellMap));
                    }
                }
                double result = values[0];
                for (int i = 0; i < operators.Count; i++)
                {
                    result = operators[i] switch
                    {
                        "+" => result + values[i + 1],
                        "-" => result - values[i + 1],
                        _ => result
                    };
                }

                return result.ToString();
            }
            catch
            {
                return ErrorValue;
            }
        }

        public static string EvaluateTextFormula(string formula, Dictionary<CellAddress, Cell> cellMap)
        {
            try
            {
                var tokens = TokenizeFormula(formula);
                var result = new StringBuilder();
                string currentOperator = null;

                foreach (var token in tokens)
                {
                    if (IsOperator(token))
                    {
                        currentOperator = token;
                        continue;
                    }

                    string value = GetTextValue(token, cellMap);

                    if (currentOperator == "+")
                    {
                        result.Append(value);
                        currentOperator = null; 
                    }
                    else if (currentOperator != null && currentOperator != "+")
                    {
                        return ErrorValue; 
                    }
                    else
                    {
                        result.Append(value);
                    }
                }

                return result.ToString();
            }
            catch
            {
                return ErrorValue;
            }
        }

        private static List<string> TokenizeFormula(string formula)
        {
            var tokens = new List<string>();
            var currentToken = new StringBuilder();

            foreach (char c in formula)
            {
                if (GetAllOperatorSymbols().Contains(c))
                {
                    if (currentToken.Length > 0)
                    {
                        tokens.Add(currentToken.ToString());
                        currentToken.Clear();
                    }
                    tokens.Add(c.ToString());
                }
                else if (!char.IsWhiteSpace(c))
                {
                    currentToken.Append(c);
                }
            }

            if (currentToken.Length > 0)
            {
                tokens.Add(currentToken.ToString());
            }

            return tokens;
        }

        private static double GetNumericValue(string token, Dictionary<CellAddress, Cell> cellMap)
        {
            if (double.TryParse(token, out double number))
                return number;

            var cell = cellMap[CellAddress.Parse(token)];

            if (cell.CellType == CellType.Formula)
            {
                string formulaResult = EvaluateFormula(cell.RawValue, cellMap);
                if (formulaResult == ErrorValue || !double.TryParse(formulaResult, out number))
                    throw new InvalidOperationException("Not a number");
                return number;
            }

            return cell.CellType == CellType.Number
                ? double.Parse(cell.ParsedValue!)
                : throw new InvalidOperationException("Not a number");
        }

        private static string GetTextValue(string token, Dictionary<CellAddress, Cell> _cellMap)
        {
            if (token.StartsWith("\"") && token.EndsWith("\""))
                return token[1..^1];

            var cell = _cellMap[CellAddress.Parse(token)];
            return cell.ParsedValue ?? string.Empty;
        }

        private static bool IsOperator(string token) =>
            token.Length == 1 && GetAllOperatorSymbols().Contains(token[0]);
    }
}
