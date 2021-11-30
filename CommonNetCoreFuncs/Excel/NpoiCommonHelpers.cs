﻿using NPOI.SS;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace CommonNetCoreFuncs.Excel
{
    public static class NpoiCommonHelpers
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public enum EStyles
        {
            Header,
            HeaderThickTop,
            Body,
            Error,
            Custom
        }

        public enum EFonts
        {
            Default,
            Header,
            BigWhiteHeader
        }

        /// <summary>
        /// Checks if cell is empty
        /// </summary>
        /// <param name="cell"></param>
        /// <returns>True if cell is empty</returns>
        public static bool IsCellEmpty(this ICell cell)
        {
            if (string.IsNullOrWhiteSpace(cell.GetStringValue()))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get ICell offset from cellReference
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="cellReference">Cell reference in A1 notation</param>
        /// <param name="colOffset">X axis offset from the named cell reference</param>
        /// <param name="rowOffset">Y axis offset from the named cell reference</param>
        /// <returns>ICell object of the specified offset of the named cell</returns>
        public static ICell GetCellFromReference(this ISheet ws, string cellReference, int colOffset = 0, int rowOffset = 0)
        {
            try
            {
                CellReference cr = new(cellReference);
                IRow row = ws.GetRow(cr.Row + rowOffset);
                ICell cell = row.GetCell(cr.Col + colOffset, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                return cell;
            }
            catch (Exception ex)
            {
                logger.Error(ex, (ex.InnerException ?? new()).ToString());
                return null;
            }
        }

        /// <summary>
        /// Get ICell offset from the startCell
        /// </summary>
        /// <param name="startCell"></param>
        /// <param name="colOffset">X axis offset from the named cell reference</param>
        /// <param name="rowOffset">Y axis offset from the named cell reference</param>
        /// <returns>ICell object of the specified offset of the startCell</returns>
        public static ICell GetCellOffset(this ICell startCell, int colOffset = 0, int rowOffset = 0)
        {
            try
            {
                ISheet ws = startCell.Sheet;
                IRow row = ws.GetRow(startCell.RowIndex + rowOffset);
                ICell cell = row.GetCell(startCell.ColumnIndex + colOffset, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                return cell;
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex?.InnerException.ToString());
                return null;
            }
        }

        /// <summary>
        /// Get ICell offset from the cell indicated with the x and y coordinates
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="x">X coordinate of starting cell</param>
        /// <param name="y">Y coordinate of starting cell</param>
        /// <param name="colOffset">X axis offset from the named cell reference</param>
        /// <param name="rowOffset">Y axis offset from the named cell reference</param>
        /// <returns>ICell object of the specified offset of the cell indicated with the x and y coordinates</returns>
        public static ICell GetCellFromCoordinates(this ISheet ws, int x, int y, int colOffset = 0, int rowOffset = 0)
        {
            try
            {
                IRow row = ws.GetRow(y + rowOffset);
                if (row == null)
                {
                    row = ws.CreateRow(y + rowOffset);
                }
                ICell cell = row.GetCell(x + colOffset, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                return cell;
            }
            catch (Exception ex)
            {
                logger.Error(ex, (ex.InnerException ?? new()).ToString());
                return null;
            }
        }

        /// <summary>
        /// Get ICell offset from the cell with named reference cellName
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="cellName"></param>
        /// <param name="colOffset"></param>
        /// <param name="rowOffset"></param>
        /// <returns>ICell object of the specified offset of the cell with named reference cellName</returns>
        public static ICell GetCellFromName(this XSSFWorkbook wb, string cellName, int colOffset = 0, int rowOffset = 0)
        {
            try
            {
                IName name = wb.GetName(cellName);
                CellReference[] crs = new AreaReference(name.RefersToFormula, SpreadsheetVersion.EXCEL2007).GetAllReferencedCells();
                ISheet ws = null;
                int rowNum = -1;
                int colNum = -1;
                for (int i = 0; i < crs.Length; i++)
                {
                    if (ws == null)
                    {
                        ws = wb.GetSheet(crs[i].SheetName);
                    }

                    if (rowNum == -1 || rowNum > crs[i].Row)
                    {
                        rowNum = crs[i].Row;
                    }

                    if (colNum == -1 || colNum > crs[i].Col)
                    {
                        colNum = crs[i].Col;
                    }
                }

                if (ws != null && colNum > -1 && rowNum > -1)
                {
                    IRow row = ws.GetRow(rowNum + rowOffset);
                    if (row == null)
                    {
                        row = ws.CreateRow(rowNum + rowOffset);
                    }
                    ICell cell = row.GetCell(colNum + colOffset, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    return cell;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, (ex.InnerException ?? new()).ToString());
                return null;
            }
        }

        /// <summary>
        /// Clear contents from cell with named reference cellName
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="cellName"></param>
        public static void ClearAllFromName(this XSSFWorkbook wb, string cellName)
        {
            IName name = wb.GetName(cellName);
            CellReference[] crs = new AreaReference(name.RefersToFormula, SpreadsheetVersion.EXCEL2007).GetAllReferencedCells();
            ISheet ws = wb.GetSheet(crs[0].SheetName);

            if (ws == null || crs.Length == 0 || name == null)
            {
                return;
            }

            for (int i = 0; i < crs.Length; i++)
            {
                IRow row = ws.GetRow(crs[i].Row);
                if (row != null)
                {
                    ICell cell = row.GetCell(crs[i].Col);
                    if (cell != null)
                    {
                        row.RemoveCell(cell);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes cell at indicated row and column
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns>ICell object of the cell that was created</returns>
        public static ICell CreateCell(this IRow row, int col)
        {
            return row.CreateCell(col);
        }

        /// <summary>
        /// Writes an excel file to the specified path
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="path"></param>
        /// <returns>True if write was successful</returns>
        public static bool WriteExcelFile(XSSFWorkbook wb, string path)
        {
            try
            {
                using (FileStream fs = new(path, FileMode.Create, FileAccess.Write))
                {
                    wb.Write(fs);
                }
                wb.Close();
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, (ex.InnerException ?? new()).ToString());
                return false;
            }
        }


        /// <exception cref="Exception">Ignore.</exception>
        /// <summary>
        /// Get cell style based on enum EStyle options
        /// </summary>
        /// <param name="style"></param>
        /// <param name="wb"></param>
        /// <param name="cellLocked"></param>
        /// <param name="htmlColor"></param>
        /// <param name="font"></param>
        /// <param name="alignment"></param>
        /// <returns>IXLStyle object containing all of the styling associated with the input EStyles option</returns>
        public static ICellStyle GetStyle(EStyles style, XSSFWorkbook wb, bool cellLocked = false, string htmlColor = null, IFont font = null, NPOI.SS.UserModel.HorizontalAlignment? alignment = null)
        {
            ICellStyle cellStyle = wb.CreateCellStyle();
            switch (style)
            {
                case EStyles.Header:
                    cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                    cellStyle.FillPattern = FillPattern.SolidForeground;
                    cellStyle.SetFont(GetFont(EFonts.Header, wb));
                    break;

                case EStyles.HeaderThickTop:
                    cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Medium;
                    cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                    cellStyle.FillPattern = FillPattern.SolidForeground;
                    cellStyle.SetFont(GetFont(EFonts.Header, wb));
                    break;

                case EStyles.Body:
                    cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.COLOR_NORMAL;
                    cellStyle.SetFont(GetFont(EFonts.Default, wb));
                    break;

                case EStyles.Error:
                    cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;
                    cellStyle.FillPattern = FillPattern.SolidForeground;
                    break;

                case EStyles.Custom:
                    XSSFCellStyle xStyle = (XSSFCellStyle)wb.CreateCellStyle();
                    if (alignment != null) { xStyle.Alignment = (NPOI.SS.UserModel.HorizontalAlignment)alignment; }
                    byte[] rgb = new byte[] { ColorTranslator.FromHtml(htmlColor).R, ColorTranslator.FromHtml(htmlColor).G, ColorTranslator.FromHtml(htmlColor).B };
                    xStyle.SetFillForegroundColor(new XSSFColor(rgb));
                    xStyle.FillPattern = FillPattern.SolidForeground;
                    if (font != null) { xStyle.SetFont(font); }
                    cellStyle = xStyle;
                    break;

                default:
                    break;
            }
            cellStyle.IsLocked = cellLocked;
            return cellStyle;
        }

        /// <summary>
        /// Get font styling based on EFonts option
        /// </summary>
        /// <param name="font"></param>
        /// <param name="wb"></param>
        /// <returns>IXLFont object containing all of the styling associated with the input EFonts option</returns>
        public static IFont GetFont(EFonts font, XSSFWorkbook wb)
        {
            IFont cellFont = wb.CreateFont();
            switch (font)
            {
                case EFonts.Default:
                    cellFont.IsBold = false;
                    cellFont.FontHeightInPoints = 10;
                    cellFont.FontName = "Calibri";
                    break;

                case EFonts.Header:
                    cellFont.IsBold = true;
                    cellFont.FontHeightInPoints = 10;
                    cellFont.FontName = "Calibri";
                    break;

                default:
                    break;
            }
            return cellFont;
        }


        /// <summary>
        /// Generates a simple excel file containing the passed in data in a tabular format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wb"></param>
        /// <param name="ws"></param>
        /// <param name="data"></param>
        /// <returns>True if excel file was created successfully</returns>
        public static bool ExportFromTable<T>(XSSFWorkbook wb, ISheet ws, List<T> data)
        {
            try
            {
                if (data != null)
                {
                    if (data.Count > 0)
                    {
                        ICellStyle headerStyle = GetStyle(EStyles.Header, wb);
                        ICellStyle bodyStyle = GetStyle(EStyles.Body, wb);

                        int x = 0;
                        int y = 0;

                        var header = data[0];
                        var props = header.GetType().GetProperties();
                        foreach (var prop in props)
                        {
                            ICell c = ws.GetCellFromCoordinates(x, y);
                            c.SetCellValue(prop.Name.ToString());
                            c.CellStyle = headerStyle;
                            x++;
                        }
                        x = 0;
                        y++;

                        foreach (var item in data)
                        {
                            var props2 = item.GetType().GetProperties();
                            foreach (var prop in props2)
                            {
                                var val = prop.GetValue(item) ?? string.Empty;
                                ICell c = ws.GetCellFromCoordinates(x, y);
                                c.SetCellValue(val.ToString());
                                c.CellStyle = bodyStyle;
                                x++;
                            }
                            x = 0;
                            y++;
                        }

                        ws.SetAutoFilter(new CellRangeAddress(0, 0, 0, props.Length - 1));

                        foreach (var prop in props)
                        {
                            ws.AutoSizeColumn(x, true);
                            x++;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, (ex.InnerException ?? new()).ToString());
                return false;
            }
        }

        /// <summary>
        /// Gets string value contained in cell
        /// </summary>
        /// <param name="cell"></param>
        /// <returns>String representation of the value in cell</returns>
        public static string GetStringValue(this ICell cell)
        {
            return cell.CellType switch
            {
                CellType.Unknown => string.Empty,
                CellType.Numeric => cell.NumericCellValue.ToString(),
                CellType.String => cell.StringCellValue,
                CellType.Formula => cell.CachedFormulaResultType switch
                {
                    CellType.Unknown => string.Empty,
                    CellType.Numeric => cell.NumericCellValue.ToString(),
                    CellType.String => cell.StringCellValue,
                    CellType.Blank => string.Empty,
                    CellType.Boolean => cell.BooleanCellValue.ToString(),
                    CellType.Error => cell.ErrorCellValue.ToString(),
                    _ => string.Empty,
                },
                CellType.Blank => string.Empty,
                CellType.Boolean => cell.BooleanCellValue.ToString(),
                CellType.Error => cell.ErrorCellValue.ToString(),
                _ => string.Empty,
            };
        }


        /// <summary>
        /// Writes excel file to a MemoryStream object
        /// </summary>
        /// <param name="memoryStream"></param>
        /// <param name="wb"></param>
        /// <returns></returns>
        public static async Task WriteFileToMemoryStreamAsync(this MemoryStream memoryStream, XSSFWorkbook wb)
        {
            using MemoryStream tempStream = new();
            wb.Write(tempStream, true);
            await tempStream.FlushAsync();
            tempStream.Seek(0, SeekOrigin.Begin);
            await tempStream.CopyToAsync(memoryStream);
            await tempStream.DisposeAsync();
            await memoryStream.FlushAsync();
            memoryStream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Adds images into a workbook at the designated named ranges
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="imageData"></param>
        /// <param name="cellNames"></param>
        public static void AddImages(this XSSFWorkbook wb, List<byte[]> imageData, List<string> cellNames)
        {
            if (wb != null && imageData.Count > 0 && cellNames.Count > 0 && imageData.Count == cellNames.Count)
            {
                ISheet ws = null;
                ICreationHelper helper = wb.GetCreationHelper();
                IDrawing drawing = null;
                for (int i = 0; i < imageData.Count; i++)
                {
                    if (imageData[i].Length > 0 && wb != null && cellNames[i] != null)
                    {
                        ICell cell = wb.GetCellFromName(cellNames[i]);
                        CellRangeAddress area = GetRangeOfMergedCells(cell);

                        if (cell != null && area != null)
                        {
                            if (ws == null)
                            {
                                ws = cell.Sheet;
                                drawing = ws.CreateDrawingPatriarch();
                            }

                            IClientAnchor anchor = helper.CreateClientAnchor();

                            int imgWidth;
                            int imgHeight;
                            using (MemoryStream ms = new(imageData[i]))
                            {
                                using Image img = Image.FromStream(ms);
                                imgWidth = img.Width;
                                imgHeight = img.Height;
                            }
                            double imgAspect = (double)imgWidth / imgHeight;

                            int rangeWidth = ws.GetRangeWidthInPx(area.FirstColumn, area.LastColumn);
                            int rangeHeight = ws.GetRangeHeightInPx(area.FirstRow, area.LastRow);
                            double rangeAspect = (double)rangeWidth / rangeHeight;

                            double scale;

                            if (rangeAspect < imgAspect)
                            {
                                scale = (rangeWidth - 3.0) / imgWidth; //Set to width of cell -3px
                            }
                            else
                            {
                                scale = (rangeHeight - 3.0) / imgHeight; //Set to width of cell -3px
                            }
                            int resizeWidth = (int)Math.Round(imgWidth * scale, 0, MidpointRounding.ToZero);
                            int resizeHeight = (int)Math.Round(imgHeight * scale, 0, MidpointRounding.ToZero);
                            int xMargin = (int)Math.Round((rangeWidth - resizeWidth) * XSSFShape.EMU_PER_PIXEL / 2.0, 0, MidpointRounding.ToZero);
                            int yMargin = (int)Math.Round((rangeHeight - resizeHeight) * XSSFShape.EMU_PER_PIXEL * 1.75 / 2.0, 0, MidpointRounding.ToZero);

                            anchor.AnchorType = AnchorType.DontMoveAndResize;
                            anchor.Col1 = area.FirstColumn;
                            anchor.Row1 = area.FirstRow;
                            anchor.Col2 = area.LastColumn + 1;
                            anchor.Row2 = area.LastRow + 1;
                            anchor.Dx1 = xMargin;
                            anchor.Dy1 = yMargin;
                            anchor.Dx2 = -xMargin;
                            anchor.Dy2 = -yMargin;

                            int pictureIndex = wb.AddPicture(imageData[i], PictureType.PNG);
                            drawing.CreatePicture(anchor, pictureIndex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets CellRangeAddress of merged cells
        /// </summary>
        /// <param name="cell"></param>
        /// <returns>CellRangeAddress of merged cells cell</returns>
        public static CellRangeAddress GetRangeOfMergedCells(this ICell cell)
        {
            if (cell != null && cell.IsMergedCell)
            {
                ISheet sheet = cell.Sheet;
                for (int i = 0; i < sheet.NumMergedRegions; i++)
                {
                    CellRangeAddress region = sheet.GetMergedRegion(i);
                    if (region.ContainsRow(cell.RowIndex) &&
                        region.ContainsColumn(cell.ColumnIndex))
                    {
                        return region;
                    }
                }
                return null;
            }
            return CellRangeAddress.ValueOf($"{cell.Address}:{cell.Address}");
        }

        /// <summary>
        /// Get the width of a specified range in pixels
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="startCol"></param>
        /// <param name="endCol"></param>
        /// <returns>Double representation of the width of the column range in pixels</returns>
        public static int GetRangeWidthInPx(this ISheet ws, int startCol, int endCol)
        {
            if (startCol > endCol)
            {
                int endTemp = startCol;
                startCol = endCol;
                endCol = endTemp;
            }

            float totalWidth = 0;
            for (int i = startCol; i < endCol + 1; i++)
            {
                float columnWidth = ws.GetColumnWidthInPixels(i);
                if (columnWidth == 0.0)
                {
                    logger.Warn($"Width of Column {i} is 0! Check referenced excel sheet: {ws.SheetName}");
                }
                totalWidth += ws.GetColumnWidthInPixels(i);
            }
            int widthInt = (int)Math.Round(totalWidth, 0, MidpointRounding.ToZero);
            return widthInt;
        }

        /// <summary>
        /// Get the height of a specified range in pixels
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="startCol"></param>
        /// <param name="endCol"></param>
        /// <returns>Double representation of the height of the rows range in pixels</returns>
        public static int GetRangeHeightInPx(this ISheet ws, int startRow, int endRow)
        {
            if (startRow > endRow)
            {
                int endTemp = startRow;
                startRow = endRow;
                endRow = endTemp;
            }

            float totaHeight = 0;
            for (int i = startRow; i < endRow + 1; i++)
            {
                totaHeight += ws.GetRow(i).HeightInPoints;
            }
            int heightInt = (int)Math.Round(totaHeight * XSSFShape.EMU_PER_POINT / XSSFShape.EMU_PER_PIXEL, 0, MidpointRounding.ToZero); //Approximation of point to px
            return heightInt;
        }

        /// <summary>
        /// Get cells contained within a range
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="range">String cell reference in A1 notation</param>
        /// <returns>Array of cells contained within the range specified</returns>
        public static ICell[,] GetRange(ISheet sheet, string range)
        {
            string[] cellStartStop = range.Split(':');

            CellReference cellRefStart = new(cellStartStop[0]);
            CellReference cellRefStop = new(cellStartStop[1]);

            ICell[,] cells = new ICell[cellRefStop.Row - cellRefStart.Row + 1, cellRefStop.Col - cellRefStart.Col + 1];

            for (int i = cellRefStart.Row; i < cellRefStop.Row + 1; i++)
            {
                IRow row = sheet.GetRow(i);
                for (int j = cellRefStart.Col; j < cellRefStop.Col + 1; j++)
                {
                    cells[i - cellRefStart.Row, j - cellRefStart.Col] = row.GetCell(j);
                }
            }

            return cells;
        }

        public static void AddDataValidation(this ISheet ws, CellRangeAddressList cellRangeAddressList, List<string> options)
        {
            IDataValidationHelper validationHelper = ws.GetDataValidationHelper();
            IDataValidationConstraint constraint = validationHelper.CreateExplicitListConstraint(options.ToArray());
            IDataValidation dataValidation = validationHelper.CreateValidation(constraint, cellRangeAddressList);

            dataValidation.ShowErrorBox = true;
            dataValidation.ErrorStyle = 0;
            dataValidation.CreateErrorBox("InvalidValue", "Selected value must be in list");
            dataValidation.ShowErrorBox = true;
            dataValidation.ShowPromptBox = false;
            //ws.AddValidationData(dataValidation);

            ws.AddValidationData(dataValidation);
        }
    }
}