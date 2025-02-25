namespace Fanex.NexHR.Core.NPOIExtensions
{
    using NPOI.SS.Util;

    public static class HSSFCellRangeAddressExtensions
    {
        public static CellRangeAddress Clone(this CellRangeAddressBase range)
        {
            var result = new CellRangeAddress(range.FirstRow, range.LastRow, range.FirstColumn, range.LastColumn);

            return result;
        }
    }
}