using SpreadShet;
using System.Net;

bool isInputValid = false;
string? spreadSheetDimensionsInput;

ConsoleLogs.ShowSpecifySpreadSheetDimensionsTitle();
do
{
    spreadSheetDimensionsInput = Console.ReadLine();

    if (!SpreadSheetInputHandler.IsInputValid(spreadSheetDimensionsInput))
    {
        ConsoleLogs.WrongInputErrorMessage();
        continue;
    }

    isInputValid = true;

} while (!isInputValid);

SpreadSheetDimensions spreadSheetDimensions = SpreadSheetInputHandler.ParseDimensions(spreadSheetDimensionsInput);
SpreadSheet spreadSheet = new(spreadSheetDimensions);
SpreadSheetService spreadSheetService = new(spreadSheet);

ConsoleLogs.ShowSpreadSheetFillTitle();
ConsoleLogs.ShowSpreadSheetFillInstructions();

spreadSheetService.FillCellWithValue();

ConsoleLogs.ShowUnevaluatedSpreadSheetTitle();
spreadSheetService.PrintUnevaluatedSpreadSheet();

ConsoleLogs.ShowEvaluatedSpreadSheetTitle();
spreadSheetService.PrintEvaluatedSpreadSheet();




