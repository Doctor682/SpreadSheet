using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadShet
{
    public class SpreadSheetService
    {
        private SpreadSheet _spreadSheet;
        public SpreadSheetService(SpreadSheet spreadSheet)
        {
             _spreadSheet = spreadSheet;
        }
        public void FillCellWithValue()
        {
            for (int row = 1; row <= _spreadSheet.Dimensions.RowSize; row++)
            {
                for (int col = 0; col < _spreadSheet.Dimensions.ColumnSize; col++)
                {
                    string columnLetter = ((char)('A' + col)).ToString();
                    CellAddress address = new(columnLetter, row);

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"│Enter value for {address}: ");
                    Console.ResetColor();

                    string? value = Console.ReadLine();
                    _spreadSheet.SetCellValue(address, value);

                }
            }
        }
        public void PrintUnevaluatedSpreadSheet()
        {
            const int padRight = -8;
            Console.Write("    ");
            for (int col = 0; col < _spreadSheet.Dimensions.ColumnSize; col++)
            {
                char colLetter = (char)('A' + col);
                Console.Write($"  {colLetter,padRight}");
            }
            Console.WriteLine(" ");

            for (int row = 1; row <= _spreadSheet.Dimensions.RowSize; row++)
            {
                Console.Write($"{row,0}   ");

                for (int col = 0; col < _spreadSheet.Dimensions.ColumnSize; col++)
                {
                    string columnLetter = ((char)('A' + col)).ToString();
                    CellAddress address = new(columnLetter, row);

                    string? cellValue = _spreadSheet.GetCell(address)?.RawValue;

                    Console.Write($"  {cellValue,padRight}");
                }

                Console.WriteLine(" ");
            }
        }

        public void PrintEvaluatedSpreadSheet()
        {
            const int padRight = -8;
            Console.Write("    ");
            for (int col = 0; col < _spreadSheet.Dimensions.ColumnSize; col++)
            {
                char colLetter = (char)('A' + col);
                Console.Write($"  {colLetter,padRight}");
            }
            Console.WriteLine(" ");

            for (int row = 1; row <= _spreadSheet.Dimensions.RowSize; row++)
            {
                Console.Write($"{row,0}   ");

                for (int col = 0; col < _spreadSheet.Dimensions.ColumnSize; col++)
                {
                    string columnLetter = ((char)('A' + col)).ToString();
                    CellAddress address = new(columnLetter, row);
 
                    string? cellValue = _spreadSheet.GetCellEvaluatedValue(address);

                    Console.Write($"  {cellValue,padRight}");
                }

                Console.WriteLine(" ");
            }
        }

    }
}
