using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using VTECHERP.Domain.Shared.Helper.Excel.Model;
using VTECHERP.Extensions;
using Volo.Abp;

namespace Vinpearl.Modelling.Library.Utility.Excel
{
    public class ExcelHelper
    {
        public static byte[] ExportExcel<T>(List<T> datas, string sheetName = "Sheet 1")
        {
            using var workbook = new XLWorkbook();
            var typeOfT = typeof(T);
            var properties = typeOfT.GetProperties();
            var firstRowIndex = 1;
            var columnIndex = 1;
            var worksheet = workbook.Worksheets.Add(sheetName);

            foreach (var property in properties)
            {
                var headerName = property.HeaderName();
                var cell = worksheet.Cell(firstRowIndex, columnIndex++);
                cell.Value = headerName;
                cell.Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            }

            for (int i = 0; i < datas.Count; i++)
            {
                var data = datas[i];
                var row = i + firstRowIndex + 1;
                columnIndex = 1;

                foreach (var property in properties)
                {
                    var value = property.GetValue(data);
                    worksheet.Cell(row, columnIndex).DataType = XLDataType.Text;
                    worksheet.Cell(row, columnIndex).SetValue(value);
                    worksheet.Cell(row, columnIndex).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(row, columnIndex).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(row, columnIndex).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(row, columnIndex).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    columnIndex++;
                }
            }

            worksheet.ColumnsUsed().AdjustToContents();
            //worksheet.Rows().AdjustToContents();

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }
        public static List<T> ImportExcel<T>(byte[] file)
        {
            using var stream = new MemoryStream(file);
            var workBook = new XLWorkbook(stream, XLEventTracking.Disabled);
            var listData = new List<T>();
            var typeOfT = typeof(T);
            var properties = typeOfT.GetProperties();
            foreach (var sheet in workBook.Worksheets)
            {
                var mappingColumns = new Dictionary<int, string>();
                var firstRow = sheet.FirstRowUsed();
                foreach (var property in properties)
                {
                    var headerName = property.HeaderName();
                    var propertyHeaderCell = firstRow.CellsUsed(cell => cell.GetString() == headerName).FirstOrDefault();
                    if (propertyHeaderCell != null)
                    {
                        mappingColumns.Add(propertyHeaderCell.WorksheetColumn().ColumnNumber(), property.Name);
                    }
                }

                foreach (var row in sheet.RowsUsed())
                {
                    if (row.RowNumber() == 1)
                    {
                        continue;
                    }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    T data = (T)Activator.CreateInstance(typeof(T));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    if (data != null)
                    {
                        var cells = row.CellsUsed();
                        foreach (var cell in cells)
                        {
                            var colIndex = cell.WorksheetColumn().ColumnNumber();
                            var colValue = cell.GetString();
                            mappingColumns.TryGetValue(colIndex, out var propertyName);
                            if (propertyName.IsNullOrEmpty())
                            {
                                continue;
                            }
                            var property = data.GetType().GetProperty(propertyName);

                            if (property != null)
                            {
                                if (colValue.IsNullOrEmpty())
                                {
                                    if (property.PropertyType.IsGenericType &&
                                        property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                    {
                                        property.SetValue(data, null);
                                    }
                                    else
                                    {
                                        var defaultValue = Activator.CreateInstance(property.PropertyType);
                                        property.SetValue(data, Convert.ChangeType(defaultValue, property.PropertyType));
                                    }
                                }
                                else
                                {
                                    if (property.PropertyType == typeof(DateTime?))
                                    {
                                        var value = (DateTime?)Convert.ToDateTime(colValue);
                                        property.SetValue(data, value);
                                    }
                                    else if (property.PropertyType == typeof(int?))
                                    {
                                        var value = (int?)Convert.ToInt32(colValue);
                                        property.SetValue(data, value);
                                    }
                                    else
                                    {
                                        property.SetValue(data, Convert.ChangeType(colValue, property.PropertyType));
                                    }
                                }
                            }
                        }
                        listData.Add(data);
                    }
                }
            }
            //var worksheet = workBook.Worksheets.First();

            return listData;
        }
        public static List<T> ImportExcel<T>(byte[] file, string sheetName)
        {
            using var stream = new MemoryStream(file);
            var workBook = new XLWorkbook(stream, XLEventTracking.Disabled);
            var listData = new List<T>();
            var typeOfT = typeof(T);
            var properties = typeOfT.GetProperties();
            var sheet = workBook.Worksheets.FirstOrDefault(p => p.Name == sheetName);
            if (sheet != null)
            {
                var mappingColumns = new Dictionary<int, string>();
                var firstRow = sheet.FirstRowUsed();
                foreach (var property in properties)
                {
                    var headerName = property.HeaderName();
                    var propertyHeaderCell = firstRow.CellsUsed(cell => cell.GetString() == headerName).FirstOrDefault();
                    if (propertyHeaderCell != null)
                    {
                        mappingColumns.Add(propertyHeaderCell.WorksheetColumn().ColumnNumber(), property.Name);
                    }
                }

                foreach (var row in sheet.RowsUsed())
                {
                    if (row.RowNumber() == 1)
                    {
                        continue;
                    }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    T data = (T)Activator.CreateInstance(typeof(T));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    if (data != null)
                    {
                        var cells = row.CellsUsed();
                        foreach (var cell in cells)
                        {
                            var colIndex = cell.WorksheetColumn().ColumnNumber();
                            var colValue = cell.GetString();
                            mappingColumns.TryGetValue(colIndex, out var propertyName);
                            if (propertyName.IsNullOrEmpty())
                            {
                                continue;
                            }
                            var property = data.GetType().GetProperty(propertyName);

                            if (property != null)
                            {
                                if (colValue.IsNullOrEmpty())
                                {
                                    if (property.PropertyType.IsGenericType &&
                                        property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                    {
                                        property.SetValue(data, null);
                                    }
                                    else
                                    {
                                        var defaultValue = Activator.CreateInstance(property.PropertyType);
                                        property.SetValue(data, Convert.ChangeType(defaultValue, property.PropertyType));
                                    }
                                }
                                else
                                {
                                    if (property.PropertyType == typeof(DateTime?))
                                    {
                                        var value = (DateTime?)Convert.ToDateTime(colValue);
                                        property.SetValue(data, value);
                                    }
                                    else if (property.PropertyType == typeof(int?))
                                    {
                                        var value = (int?)Convert.ToInt32(colValue);
                                        property.SetValue(data, value);
                                    }
                                    else
                                    {
                                        property.SetValue(data, Convert.ChangeType(colValue, property.PropertyType));
                                    }
                                }
                            }
                        }
                        listData.Add(data);
                    }
                }
            }
            //var worksheet = workBook.Worksheets.First();

            return listData;
        }
        public static byte[] ExportExcel(CustomWorkBook book)
        {
            using var workbook = new XLWorkbook();

            foreach (var sheet in book.Sheets)
            {
                var sheetName = sheet.SheetName.IsNullOrEmpty() ? "No Name" : sheet.SheetName;
                var worksheet = workbook.Worksheets.Add(sheetName);

                foreach (var fixedHeader in sheet.FixedHeaders)
                {
                    IXLRange? range;
                    if (!fixedHeader.Range.IsNullOrEmpty())
                    {
                        range = worksheet.Range(fixedHeader.Range).Merge();
                    }
                    else
                    {
                        range = worksheet.Range(fixedHeader.StartRowIndex, fixedHeader.StartColumnIndex, fixedHeader.EndRowIndex, fixedHeader.EndColumnIndex).Merge();
                    }

                    if (range != null)
                    {
                        ApplyFixedRangeStyle(range, fixedHeader.Value, fixedHeader.BackgroundColor, fixedHeader.FontColor, fixedHeader.Bold, fixedHeader.Italic, fixedHeader.Underline, fixedHeader.FontSize);
                    }
                }

                foreach (var customTable in sheet.Tables)
                {
                    IXLCell startCell;
                    if (customTable.StartColumnIndex.HasValue && customTable.StartRowIndex.HasValue)
                    {
                        startCell = worksheet.Cell(customTable.StartRowIndex.Value, customTable.StartColumnIndex.Value);
                    }
                    else
                    {
                        startCell = worksheet.Cell(customTable.StartCell);
                    }

                    if (startCell == null)
                    {
                        throw new Exception("Cannot get start position for custom table.");
                    }

                    var startCol = startCell.WorksheetColumn().ColumnNumber();
                    var startRow = startCell.WorksheetRow().RowNumber();

                    #region Vertical Direction
                    if (customTable.RowDirection == Directions.Vertical)
                    {
                        var col = startCol;
                        foreach (var dataRow in customTable.Rows)
                        {
                            var row = startRow;
                            var cellSpan = 1;
                            foreach (var dataCell in dataRow.Cells)
                            {
                                cellSpan = dataCell.ColSpan;
                                var rowSpan = dataCell.RowSpan;
                                var cell = worksheet.Cell(row, col);

                                if (dataCell.ColSpan > 1 || dataCell.RowSpan > 1)
                                {
                                    var lastCell = worksheet.Cell(row + dataCell.RowSpan - 1, col + dataCell.ColSpan - 1);
                                    var range = worksheet.Range(cell, lastCell);
                                    range.Merge();
                                    ApplyRangeStyle(range, dataCell);
                                }
                                else
                                {
                                    ApplyCellStyle(cell, dataCell);
                                }

                                row += rowSpan;
                            }

                            col += cellSpan;
                        }
                    }
                    #endregion
                    #region Horizontal Direction
                    else
                    {
                        var row = startRow;
                        foreach (var dataRow in customTable.Rows)
                        {
                            var col = startCol;
                            var cellSpan = 1;
                            foreach (var dataCell in dataRow.Cells)
                            {
                                var cell = worksheet.Cell(row, col);
                                cellSpan = dataCell.RowSpan;
                                var colSpan = 1;
                                if (dataCell.ColSpan > 1 || dataCell.RowSpan > 1)
                                {
                                    colSpan = dataCell.ColSpan;
                                    var lastCell = worksheet.Cell(row + dataCell.RowSpan - 1, col + dataCell.ColSpan - 1);

                                    var range = worksheet.Range(cell, lastCell);
                                    range.Merge();
                                    ApplyRangeStyle(range, dataCell);
                                }
                                else
                                {
                                    ApplyCellStyle(cell, dataCell);
                                }

                                col += colSpan;
                            }

                            row += cellSpan;
                        }
                    }
                    #endregion
                }

                worksheet.ColumnsUsed().AdjustToContents();
                //worksheet.Rows().AdjustToContents();
                if (sheet.IsFreezeRow)
                {
                    worksheet.SheetView.FreezeRows(sheet.FreezeRow);
                }
                if (sheet.IsFreezeColumn)
                {
                    worksheet.SheetView.FreezeColumns(sheet.FreezeColumn);
                }
            }

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }
        public static byte[] InsertCustomTable(byte[] file, CustomDataTable customTable, string sheetName = "Sheet 1")
        {
            using var stream = new MemoryStream(file);
            var workBook = new XLWorkbook(stream, XLEventTracking.Disabled);
            workBook.TryGetWorksheet(sheetName, out IXLWorksheet worksheet);
            if (worksheet == null)
            {
                worksheet = workBook.Worksheets.First();
            }
            //worksheet.Protect();
            IXLCell? startCell = null;
            if (!customTable.StartCell.IsNullOrEmpty())
            {
                startCell = worksheet.Cell(customTable.StartCell);
            }
            else if (customTable.StartColumnIndex.HasValue && customTable.StartRowIndex.HasValue)
            {
                startCell = worksheet.Cell(customTable.StartRowIndex.Value, customTable.StartColumnIndex.Value);
            }

            if (startCell == null)
            {
                throw new Exception("Cannot get start position for custom table.");
            }

            if (startCell != null)
            {
                var startCol = startCell.WorksheetColumn().ColumnNumber();
                var startRow = startCell.WorksheetRow().RowNumber();

                if (customTable.RowDirection == Directions.Vertical)
                {
                    var col = startCol;
                    foreach (var dataRow in customTable.Rows)
                    {
                        var row = startRow;
                        var cellSpan = 1;
                        foreach (var dataCell in dataRow.Cells)
                        {
                            cellSpan = dataCell.RowSpan;
                            var cell = worksheet.Cell(row, col);
                            var rowSpan = dataCell.RowSpan;
                            if (dataCell.ColSpan > 1 || dataCell.RowSpan > 1)
                            {
                                var lastCell = worksheet.Cell(row + dataCell.RowSpan - 1, col + dataCell.ColSpan - 1);
                                var range = worksheet.Range(cell, lastCell);
                                range.Merge();
                                ApplyRangeStyle(range, dataCell);
                            }
                            else
                            {
                                ApplyCellStyle(cell, dataCell);
                            }

                            row += rowSpan;
                        }

                        col += cellSpan;
                    }
                }
                else
                {
                    var row = startRow;
                    foreach (var dataRow in customTable.Rows)
                    {
                        var col = startCol;
                        var cellSpan = 1;
                        foreach (var dataCell in dataRow.Cells)
                        {
                            var cell = worksheet.Cell(row, col);
                            cellSpan = dataCell.RowSpan;
                            var colSpan = dataCell.ColSpan;
                            if (dataCell.ColSpan > 1 || dataCell.RowSpan > 1)
                            {
                                var lastCell = worksheet.Cell(row + dataCell.RowSpan - 1, col + dataCell.ColSpan - 1);

                                var range = worksheet.Range(cell, lastCell);
                                range.Merge();
                                ApplyRangeStyle(range, dataCell);
                            }
                            else
                            {
                                ApplyCellStyle(cell, dataCell);
                            }

                            col += colSpan;
                        }

                        row += cellSpan;
                    }
                }
            }
            worksheet.ColumnsUsed().AdjustToContents();
            //worksheet.Rows().AdjustToContents();
            using var ms = new MemoryStream();
            workBook.SaveAs(ms);
            return ms.ToArray();
        }
        public static MappingData ReadMappingData(byte[] file, ReadMappingDataRequest request)
        {
            using var stream = new MemoryStream(file);
            var workBook = new XLWorkbook(stream, XLEventTracking.Disabled);

            return ReadMappingData(workBook, request);
        }

        public static MappingData ReadMappingData(XLWorkbook workBook, ReadMappingDataRequest request)
        {
            try
            {
                var mappingData = new MappingData();
                foreach (var worksheet in workBook.Worksheets)
                {
                    var mappingColumns = new Dictionary<int, string>();
                    if (worksheet.FirstCellUsed() == null)
                    {
                        continue;
                    }
                    var firstRow = worksheet.FirstCellUsed().WorksheetRow().RowNumber();
                    var firstColumn = worksheet.FirstCellUsed().WorksheetColumn().ColumnNumber();
                    var lastRow = worksheet.LastCellUsed().WorksheetRow().RowNumber();
                    var lastColumn = worksheet.LastCellUsed().WorksheetColumn().ColumnNumber();

                    if (!request.StartCell.IsNullOrEmpty())
                    {
                        var cell = worksheet.Cell(request.StartCell);
                        firstRow = cell.WorksheetRow().RowNumber();
                        firstColumn = cell.WorksheetColumn().ColumnNumber();
                    }
                    else if (request.StartRow != null && request.StartColumn != null)
                    {
                        firstRow = request.StartRow.Value;
                        firstColumn = request.StartColumn.Value;
                    }

                    if (!request.EndCell.IsNullOrEmpty())
                    {
                        var cell = worksheet.Cell(request.EndCell);
                        lastRow = cell.WorksheetRow().RowNumber();
                        lastColumn = cell.WorksheetColumn().ColumnNumber();
                    }
                    else if (request.EndRow != null && request.EndColumn != null)
                    {
                        lastRow = request.EndRow.Value;
                        lastColumn = request.EndColumn.Value;
                    }
                    if (request.ReadDirection == Directions.Horizontal)
                    {
                        var row = firstRow;

                        if (request.FirstRowKeys)
                        {
                            var column = firstColumn;
                            while (column <= lastColumn)
                            {
                                var cell = worksheet.Cell(row, column);
                                var value = GetCellValue(cell).ToString();
                                if (request.TryReadHeaderAbove)
                                {
                                    var cellAbove = cell.CellAbove();
                                    var cellAboveValue = GetCellValue(cellAbove).ToString();
                                    if (cellAboveValue != null && cellAboveValue != value)
                                    {
                                        mappingData.Keys.Add($"{cellAboveValue}_{value}");
                                        mappingColumns.Add(cell.WorksheetColumn().ColumnNumber(), $"{cellAboveValue}_{value}");
                                        column++;
                                        continue;
                                    }
                                }
                                if (value != null && !value.IsNullOrEmpty())
                                {
                                    mappingData.Keys.Add(value);
                                    mappingColumns.Add(cell.WorksheetColumn().ColumnNumber(), value);
                                }
                                column++;
                            }
                            row++;
                        }

                        while (row <= lastRow)
                        {
                            var rawDataRow = new List<string>();
                            var mappedDataRow = new List<MapData>();
                            var column = firstColumn;
                            while (column <= lastColumn)
                            {
                                var cell = worksheet.Cell(row, column);
                                var value = GetCellValue(cell).ToString();
                                mappingColumns.TryGetValue(cell.WorksheetColumn().ColumnNumber(), out var mappedColumn);
   
                                rawDataRow.Add(value ?? "");
                                if (mappedColumn != null)
                                {
                                    mappedDataRow.Add(new MapData
                                    {
                                        Code = mappedColumn,
                                        Value = value ?? "",
                                        CellAddress = cell.Address.ToString() ?? "",
                                        Row = row,
                                        Column = column,
                                    });
                                }
                                column++;
                            }
                            mappingData.RawDatas.Add(rawDataRow);
                            if (mappedDataRow.Any())
                            {
                                mappingData.MappedDatas.Add(mappedDataRow);
                            }
                            row++;
                        }
                    }
                    else
                    {
                        var column = firstColumn;

                        if (request.FirstRowKeys)
                        {
                            var row = firstRow;
                            while (row <= lastRow)
                            {
                                var cell = worksheet.Cell(row, column);
                                var value = GetCellValue(cell).ToString();
                                if (value != null && !value.IsNullOrEmpty())
                                {
                                    mappingData.Keys.Add(value);
                                    mappingColumns.Add(cell.WorksheetColumn().ColumnNumber(), value);
                                }
                                row++;
                            }
                            column++;
                        }

                        while (column <= lastColumn)
                        {
                            var rawDataRow = new List<string>();
                            var mappedDataRow = new List<MapData>();
                            var row = firstRow;
                            while (column <= lastColumn)
                            {
                                var cell = worksheet.Cell(row, column);
                                var value = GetCellValue(cell).ToString();
                                mappingColumns.TryGetValue(cell.WorksheetColumn().ColumnNumber(), out var mappedColumn);
                                
                                rawDataRow.Add(value ?? "");
                                if (mappedColumn != null)
                                {
                                    mappedDataRow.Add(new MapData
                                    {
                                        Code = mappedColumn,
                                        Value = value ?? "",
                                        CellAddress = cell.Address.ToString() ?? "",
                                        Row = row,
                                        Column = column,
                                    });
                                }
                                row++;
                            }
                            mappingData.RawDatas.Add(rawDataRow);
                            if (mappedDataRow.Any())
                            {
                                mappingData.MappedDatas.Add(mappedDataRow);
                            }
                            column++;
                        }
                    }
                }

                return mappingData;
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
        private static object GetCellValue(IXLCell cell)
        {
            var range = cell.MergedRange();

            if (range != null && (range.RangeAddress.ColumnSpan > 1 || range.RangeAddress.RowSpan > 1))
            {
                //merged cel
                var valueCell = range.FirstCell();
                return valueCell.Value;
            }
            else
            {
                //non-merged cell
                return cell.Value;
            }
        }
        public static string ReadCellValue(byte[] file, string cellAddress)
        {
            using var stream = new MemoryStream(file);
            var workBook = new XLWorkbook(stream, XLEventTracking.Disabled);

            var worksheet = workBook.Worksheets.First();

            var cell = worksheet.Cell(cellAddress);
            if (cell != null)
            {
                var cellValue = GetCellValue(cell)?.ToString();
                if (cellValue != null)
                {
                    return cellValue;
                }
            }
            return "";
        }
        //public static byte[] CreateEmptyExcelWithValidateData<T>(List<ExcelValidationData> validationDatas, string sheetName = "Sheet 1")
        //{
        //    using var workbook = new XLWorkbook();
        //    var typeOfT = typeof(T);
        //    var properties = typeOfT.GetProperties();
        //    var firstRowIndex = 1;
        //    var columnIndex = 1;
        //    var worksheet = workbook.Worksheets.Add(sheetName);

        //    foreach (var property in properties)
        //    {
        //        var cell = worksheet.Cell(firstRowIndex, columnIndex);

        //        var headerName = property.HeaderName();
        //        if (validationDatas.Any(p => p.Code == property.Name))
        //        {
        //            var validateData = validationDatas.First(p => p.Code == property.Name);
        //            var validationSheet = CreateValidateDataWorksheet(workbook, validateData);

        //            var range = worksheet.Range(firstRowIndex + 1, columnIndex, 100, columnIndex);
        //            range.SetDataValidation().List(validationSheet.Range(validateData.DataRange), true);

        //            validationSheet.Hide();
        //        }

        //        cell.Value = headerName;
        //        cell.Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
        //        cell.Style.Font.FontColor = XLColor.White;
        //        columnIndex++;
        //    }

        //    worksheet.ColumnsUsed().AdjustToContents();

        //    using var ms = new MemoryStream();
        //    workbook.SaveAs(ms);
        //    return ms.ToArray();
        //}
        //public static IXLWorksheet CreateValidateDataWorksheet(XLWorkbook workbook, ExcelValidationData data)
        //{
        //    if (workbook == null)
        //    {
        //        throw new Exception();
        //    }
        //    var sheet = workbook.AddWorksheet(data.Code);
        //    var startCellAddress = "A1";

        //    var cell = sheet.Cell(startCellAddress);
        //    if (data.Values.Count() > 0)
        //    {
        //        foreach (var value in data.Values)
        //        {
        //            cell.Value = value;
        //            cell = cell.CellBelow();
        //        }

        //        var lastCellAddress = cell.CellAbove().Address;
        //        data.DataRange = $"{startCellAddress}:{lastCellAddress}";
        //    }
        //    else
        //    {
        //        throw new Exception();
        //    }

        //    return sheet;
        //}

        /// <summary>
        /// Read the sheet name of the excel file based on it's index
        /// </summary>
        /// <param name="file">Excel file</param>
        /// <param name="index">Sheet index. Default = 0</param>
        /// <returns></returns>
        public static string ReadSheetName(byte[] file, int index = 0)
        {
            using var stream = new MemoryStream(file);
            var workBook = new XLWorkbook(stream, XLEventTracking.Disabled);
            return workBook.Worksheets.Count > 0 ? workBook.Worksheets.ElementAt(index).Name : "";
        }

        protected static void ApplyFixedRangeStyle(IXLRange range, string value, XLColor bgColor, XLColor fontColor, bool bold, bool italic, bool underline, int fontSize)
        {
            range.Value = value;
            range.Style.Fill.BackgroundColor = bgColor;
            range.Style.Font.FontColor = fontColor;
            range.Style.Font.SetBold(bold);
            range.Style.Font.SetItalic(italic);
            if (underline)
                range.Style.Font.SetUnderline();
            range.Style.Font.FontSize = fontSize;
            range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        protected static void ApplyRangeStyle(IXLRange range, Cell dataCell)
        {
            range.Value = dataCell.Value;
            range.Style.Fill.BackgroundColor = dataCell.BackgroundColor;
            range.Style.Font.FontColor = dataCell.FontColor;
            range.Style.Font.SetBold(dataCell.Bold);
            range.Style.Font.SetItalic(dataCell.Italic);
            if (dataCell.Underline)
                range.Style.Font.SetUnderline();
            range.Style.Font.FontSize = dataCell.FontSize;
            range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            range.Style.Alignment.SetHorizontal(dataCell.HorizontalAlignment);
            range.Style.Alignment.SetVertical(dataCell.VerticalAlignment);

            range.Style.Protection.SetLocked(!dataCell.Editable);

            if (!dataCell.Format.IsNullOrEmpty())
            {
                range.Style.NumberFormat.Format = dataCell.Format;
                if (dataCell.Format == "@")
                {
                    range.DataType = XLDataType.Text;
                    range.Value = Convert.ToString(dataCell.Value);
                }
            }
            else
            {
                range.Style.NumberFormat.NumberFormatId = dataCell.NumberFormatId;
            }

            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            if (dataCell.CellFormula != null)
            {
                switch (dataCell.CellFormula.Type)
                {
                    case CellFormulaType.A1:
                        range.FormulaA1 = dataCell.CellFormula.Formula;
                        break;
                    case CellFormulaType.R1C1:
                        range.FormulaR1C1 = dataCell.CellFormula.Formula;
                        break;
                }
            }

            //if(dataCell.ConditionalFormat != null)
            //{
            //    var firstCell = range.FirstCellUsed();
            //    switch (dataCell.ConditionalFormat.Condition)
            //    {
            //        case CellCondition.SelfTrunc:
            //            range.AddConditionalFormat()
            //            .WhenEquals(
            //                string.Format(
            //                    "=TRUNC(${0}${1})",
            //                    firstCell.WorksheetColumn().ColumnLetter(),
            //                    firstCell.WorksheetRow().RowNumber()))
            //            .NumberFormat
            //            .Format = dataCell.ConditionalFormat.Format;
            //            break;
            //    }
            //}
        }

        protected static void ApplyCellStyle(IXLCell cell, Cell dataCell)
        {
            cell.Value = dataCell.Value;
            cell.Style.Fill.BackgroundColor = dataCell.BackgroundColor;
            cell.Style.Font.FontColor = dataCell.FontColor;
            cell.Style.Font.SetBold(dataCell.Bold);
            cell.Style.Font.SetItalic(dataCell.Italic);
            if (dataCell.Underline)
                cell.Style.Font.SetUnderline();
            cell.Style.Font.FontSize = dataCell.FontSize;
            cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            cell.Style.Alignment.SetHorizontal(dataCell.HorizontalAlignment);
            cell.Style.Alignment.SetVertical(dataCell.VerticalAlignment);

            cell.Style.Protection.SetLocked(!dataCell.Editable);

            if (!dataCell.Format.IsNullOrEmpty())
            {
                cell.Style.NumberFormat.Format = dataCell.Format;
                if (dataCell.Format == "@")
                {
                    cell.DataType = XLDataType.Text;
                    cell.Value = Convert.ToString(dataCell.Value);
                }
            }
            else
            {
                cell.Style.NumberFormat.NumberFormatId = dataCell.NumberFormatId;
            }

            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            if (dataCell.CellFormula != null)
            {
                switch (dataCell.CellFormula.Type)
                {
                    case CellFormulaType.A1:
                        cell.FormulaA1 = dataCell.CellFormula.Formula;
                        break;
                    case CellFormulaType.R1C1:
                        cell.FormulaR1C1 = dataCell.CellFormula.Formula;
                        break;
                }
            }

            //if (dataCell.ConditionalFormat != null)
            //{
            //    switch (dataCell.ConditionalFormat.Condition)
            //    {
            //        case CellCondition.SelfTrunc:
            //            var cellTrunc = string.Format(
            //                    "=TRUNC(${0}${1})",
            //                    cell.WorksheetColumn().ColumnLetter(),
            //                    cell.WorksheetRow().RowNumber());
            //            cell.AddConditionalFormat()
            //            .WhenEquals(cellTrunc)
            //            .NumberFormat
            //            .Format = dataCell.ConditionalFormat.Format;
            //            break;
            //        case CellCondition.Percentage:
            //            var percentageConditional = string.Format(
            //                    "=ABS(MOD(${0}${1},0.01)) < 0.0000000001",
            //                    cell.WorksheetColumn().ColumnLetter(),
            //                    cell.WorksheetRow().RowNumber());

            //            cell.AddConditionalFormat()
            //            .WhenIsTrue(percentageConditional)
            //            .NumberFormat
            //            .Format = dataCell.ConditionalFormat.Format;
            //            break;
            //    }
            //}
        }
    }
}
