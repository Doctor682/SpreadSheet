using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadShet
{
    public struct SpreadSheetDimensions
    {
        public int RowSize;
        public int ColumnSize;

        public SpreadSheetDimensions(int columnSize, int rowSize)
        {
            ColumnSize = columnSize;
            RowSize = rowSize;
        }
        public int GetDimensions()
        {
            return ColumnSize*RowSize;
        }
    }

}