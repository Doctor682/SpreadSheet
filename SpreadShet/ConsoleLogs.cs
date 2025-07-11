using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadShet
{
    public static class ConsoleLogs
    {
        public const string InputStartTitle = "SPECIFY THE TABLE DIMENSIONS";
        public const string InputInstruction = "Format: \"column_size,row_size\"";

        public static void ShowStartMessage()
        {
            Console.WriteLine(InputStartTitle);
        }
        public static void ShowInputInstruction()
        {
            Console.WriteLine(InputInstruction);
        }
        public static void WrongInputErrorMessage()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Your input is incorrect. Please try again.");
            Console.ResetColor();
        }
        public static void ShowSpecifySpreadSheetDimensionsTitle()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            ShowStartMessage();
            ShowInputInstruction();
            Console.ResetColor();
        }
        public static void ShowSpreadSheetFillTitle()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("FILL SPREADSHEET WITH VALUES");
        }
        public static void ShowSpreadSheetFillInstructions()
        {
            Console.WriteLine("You will fill values row by row, from column A to Z.");
            Console.WriteLine("Accepted input formats:");
            Console.WriteLine("  - Text: any word or sentence, e.g., hello");
            Console.WriteLine("  - Number: numeric values, e.g., 252 or 3.14");
            Console.WriteLine("  - Formula: starts with '=', uses cell addresses and basic operators (+, -, *, /)");
            Console.WriteLine("      Example: =A1+B2 or =C3*2");
            Console.WriteLine("TIP: Formulas must reference valid cell addresses and contain only numbers or only text.");
            Console.ResetColor();
        }
        public static void ShowUnevaluatedSpreadSheetTitle()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("UNEVALUATED SPREADSHEET:");
            Console.ResetColor();
        }
        public static void ShowEvaluatedSpreadSheetTitle()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("EVALUATED SPREADSHEET:");
            Console.ResetColor();
        }
    }
}
