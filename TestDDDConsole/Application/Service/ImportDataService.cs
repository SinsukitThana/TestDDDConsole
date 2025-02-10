using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TestDDDConsole.ORM;
using TestDDDConsole.Domain.Entity;
using TestDDDConsole.Infrastructure.Database;
using TestDDDConsole.Application.Usecase;
using TestDDDConsole.Domain.Repository;
using TestDDDConsole.Infrastructure.Database.Repository;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.InteropServices.Marshalling;

namespace TestDDDConsole.Application.Service
{
    public interface IImportDataService
    {
        Task<ResultData> GetDataFromExcel(string fileBase64, IDatabaseInformation databaseInformation);
        string GetBase64FromExcel(string filePath);
        List<Row>? GetRows(Sheet sheet, WorkbookPart? workbookPart);
        TableAndColumn ValidateExcelData(string tableName, List<Row> lstRow, IDatabaseInformation databaseInformation, WorkbookPart? workbookPart, TableAndColumn? validateTable);
        string? GetCellValue(WorkbookPart? workbookPart, Cell? cell);
        List<ColumnObject> SetColumnResult(List<Cell> data, string mode, Row columns, WorkbookPart? workbookPart, TableAndColumn tableAndColumn);
        RowObject SetRowResult(List<Cell> cells, WorkbookPart? workbookPart, Row columns, TableAndColumn tableAndColumn);
    }

    public class ImportDataServiceException : Exception
    {
        public ImportDataServiceException(string? message) : base(message)
        { }

    }

    public class ImportDataService : IImportDataService
    {

        public List<Row>? GetRows(Sheet? sheet, WorkbookPart? workbookPart)
        {
            if (sheet == null) throw new ImportDataServiceException("Excel file is invalid or empty");


            string? sheetId = sheet.Id;
            WorksheetPart? worksheetPart = (WorksheetPart?)workbookPart?.GetPartById(sheetId ?? "");
            SheetData? sheetData = worksheetPart?.Worksheet.Elements<SheetData>().First();
            var rows = sheetData?.Elements<Row>().ToList();

            if (rows?.Count < 1)
                throw new ImportDataServiceException("Excel file is invalid or empty.");
            return rows;
        }

        public TableAndColumn ValidateExcelData(string tableName,List<Row>? lstRow, IDatabaseInformation databaseInformation, WorkbookPart? workbookPart, TableAndColumn? validateTable)
        {
            if (validateTable == null)
            {
                throw new ImportDataServiceException("Wrong Table :" + tableName);
            }

            Row? rowColumn = lstRow?[1];
            List<Cell>? cellsColumn = rowColumn?.Elements<Cell>().ToList();
            if (cellsColumn != null)
            {
                foreach (Cell cell in cellsColumn)
                {
                    string? columnName = GetCellValue(workbookPart, cell);
                    if (columnName != "MODE" && validateTable.Column !=null && !validateTable.Column.Where(x => x.ColumnName == columnName).Any())
                    {
                        throw new ImportDataServiceException("Wrong Column :" + columnName);
                    }
                }
            }
            return validateTable;
        }

        // TODO: Change the context to Repository instead
        public async Task<ResultData> GetDataFromExcel(string fileBase64, IDatabaseInformation databaseInformation)
        {
            ResultData resultData = new ResultData();
            TableObject lstImportData = new TableObject();
            try
            {
                byte[] fileAsByte = Convert.FromBase64String(fileBase64);
                List<object?> data = new List<object?>();
                using (MemoryStream stream = new MemoryStream(fileAsByte))
                {
                    using (SpreadsheetDocument document = SpreadsheetDocument.Open(stream, false))
                    {
                        WorkbookPart? workbookPart = document?.WorkbookPart;
                        Sheet? sheet = workbookPart?.Workbook?.Sheets?.Elements<Sheet>().FirstOrDefault();

                        List<Row>? rows = GetRows(sheet, workbookPart);


                        List<object?> lstData = new List<object?>();
                        var columns = rows?[1];

                        //Get Table Name
                        Row? headRow = rows?[0];
                        var headCells = headRow?.Elements<Cell>().ToList();
                        List<TableAndColumn> lstTableAndColumn = await databaseInformation.GetTableAndColumns();
                        string tableName = GetCellValue(workbookPart, headCells?[1]) ?? "";
                        var getTable = lstTableAndColumn.Where(x => x.Table == tableName).FirstOrDefault();

                        //Validate 
                        var validateTable = ValidateExcelData(tableName,rows, databaseInformation, workbookPart, getTable);

                        List<RowObject> lstRows = new List<RowObject>();
                        // Read data rows
                        for (int rowIndex = 2; rowIndex < rows?.Count; rowIndex++)
                        {
                            RowObject rowObject = new RowObject();
                            Row row = rows[rowIndex];
                            var cells = row.Elements<Cell>().ToList();
                            rowObject = SetRowResult(cells, workbookPart, columns, getTable);

                            lstRows.Add(rowObject);
                        }
                        lstImportData.Rows = lstRows;
                        lstImportData.TableName = tableName;

                        resultData.Data = lstImportData;
                    }
                }
            }
            catch (Exception ex)
            {
                resultData.Error = ex.ToString();
            }
            return resultData;
        }
        public RowObject SetRowResult(List<Cell> cells, WorkbookPart? workbookPart,Row? columns, TableAndColumn? tableAndColumn)
        {
            RowObject rowObject = new RowObject();
            string? mode = GetCellValue(workbookPart, cells[cells.Count - 1]);
            List<ColumnObject> columnObject = SetColumnResult(cells, mode, columns, workbookPart, tableAndColumn);
            rowObject.Columns = columnObject;
            if (mode == "INSERT")
            {
                rowObject.Mode = ActionMode.INSERT;
            }
            else if (mode == "UPDATE")
            {
                rowObject.Mode = ActionMode.UPDATE;
            }
            else if (mode == "DELETE")
            {
                rowObject.Mode = ActionMode.DELETE;
            }
            return rowObject;
        }
        public List<ColumnObject> SetColumnResult(List<Cell> data, string? mode, Row? columns, WorkbookPart? workbookPart, TableAndColumn? tableAndColumn)
        {
            List<ColumnObject> lstImportData = new List<ColumnObject>();
            List<Cell>? cellsColumn = columns?.Elements<Cell>().ToList();
            for (int i = 0; i < cellsColumn?.Count - 1; i++)
            {
                ColumnObject columnObject = new ColumnObject();
                string? column = GetCellValue(workbookPart, cellsColumn?[i]);
                var value = GetCellValue(workbookPart, data[i]);

                columnObject.ColumnName = column??"";
                columnObject.Value = value ?? "";

                var getColumn = tableAndColumn?.Column?.Where(x => x.ColumnName == column).FirstOrDefault();
                if (getColumn != null)
                {
                    columnObject.DbType = getColumn.Datatype;
                    columnObject.IsIdentity = (getColumn.IsIdentity == "YES")?true:false;
                    columnObject.IsPrimaryKey = (getColumn.IsPrimaryKey == "YES") ? true : false;
                }

                lstImportData.Add(columnObject);
            }
            return lstImportData;
        }

        public string? GetCellValue(WorkbookPart? workbookPart, Cell? cell)
        {
            if (cell == null || cell.CellValue == null)
            {
                return null;
            }

            string value = cell.CellValue.InnerText;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return workbookPart?.SharedStringTablePart?.SharedStringTable.Elements<SharedStringItem>().ElementAt(int.Parse(value)).InnerText;
            }

            return value;
        }

        public string GetBase64FromExcel(string filePath)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(fileBytes);
            return base64String;
        }
    }

}
