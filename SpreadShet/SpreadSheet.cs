using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SpreadShet
{
    public class SpreadSheet
    {
        private readonly CellValueParser _parser = new();
        private Dictionary<CellAddress, Cell> _cellMap = new();
        public SpreadSheetDimensions Dimensions { get; }
        public SpreadSheet(SpreadSheetDimensions spreadSheetDimensions)
        {
            Dimensions = spreadSheetDimensions;
        }
        public Cell? GetCell(CellAddress cellAddress)
        {
            if (_cellMap.TryGetValue(cellAddress, out Cell? cell))
                return cell;
            return null;
        }
        public string? GetCellEvaluatedValue(CellAddress address)
        {
            if (!_cellMap.TryGetValue(address, out var cell))
                return string.Empty;

                cell.Evaluate(_parser, _cellMap);

            return cell.ParsedValue;
        }
        public void SetCellValue(CellAddress cellAddress, string? cellInputValue)
        {
            if (!_cellMap.ContainsKey(cellAddress))
                _cellMap[cellAddress] = new Cell(cellAddress, cellInputValue);
        }
    }
}
