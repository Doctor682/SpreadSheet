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
            var (cellType, error) = CheckCellValuesConsistency(formula, _cellMap);
            if(error == ErrorValue)
            {
                return ErrorValue;       
            }  

            switch (cellType)
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

            foreach (string? address in ExtractCellAddresses(formula))
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
                CellAddress cellAddr = CellAddress.Parse(address);
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
                List<string> tokens = TokenizeFormula(formula);
                List<double> values = new List<double>();
                List<string> operators = new List<string>();

                for (int tokenIndex = 0; tokenIndex < tokens.Count; tokenIndex++)
                {
                    if (IsOperator(tokens[tokenIndex]))
                    {
                        if (tokens[tokenIndex] == "*" || tokens[tokenIndex] == "/")
                        {
                            double left = values[^1];
                            double right = GetNumericValue(tokens[tokenIndex + 1], _cellMap);
                            values[^1] = tokens[tokenIndex] == "*" ? left * right : left / right;
                            tokenIndex++;
                        }
                        else
                        {
                            operators.Add(tokens[tokenIndex]);
                        }
                    }
                    else
                    {
                        values.Add(GetNumericValue(tokens[tokenIndex], _cellMap));
                    }
                }
                double result = values[0];
                for (int operatorsIndex = 0; operatorsIndex < operators.Count; operatorsIndex++)
                {
                    result = operators[operatorsIndex] switch
                    {
                        "+" => result + values[operatorsIndex + 1],
                        "-" => result - values[operatorsIndex + 1],
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
                List<string> tokens = TokenizeFormula(formula);
                StringBuilder result = new StringBuilder();
                string? currentOperator = null;

                foreach (string? token in tokens)
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
            List<string> tokens = new List<string>();
            StringBuilder currentToken = new StringBuilder();

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

            Cell cell = cellMap[CellAddress.Parse(token)];

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

        private static string GetTextValue(string token, Dictionary<CellAddress, Cell> map)
        {
            if (token.StartsWith("\"") && token.EndsWith("\""))
                return token[1..^1];

            Cell cell = map[CellAddress.Parse(token)];

            if (cell.CellType == CellType.Formula)
            {
                string formulaResult = EvaluateFormula(cell.RawValue, map);
                return formulaResult == ErrorValue ? string.Empty : formulaResult;
            }
            return cell.ParsedValue ?? string.Empty;
        }


        private static bool IsOperator(string token) =>
            token.Length == 1 && GetAllOperatorSymbols().Contains(token[0]);
    }
}
