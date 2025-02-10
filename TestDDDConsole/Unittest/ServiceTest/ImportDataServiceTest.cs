using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDDDConsole.Application.Service;
using TestDDDConsole.Application.Usecase;
using TestDDDConsole.Domain.Repository;
using Xunit;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using TestDDDConsole.Domain.Entity;

namespace TestDDDConsole.Unittest.ServiceTest
{
    public class ImportDataServiceTest
    {
        private readonly IImportDataService importDataService;
        WorkbookPart? workbookPart;
        Sheet? sheet;
        string? basePath;
        public ImportDataServiceTest()
        {
            importDataService = new ImportDataService();
            basePath = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName;
        }
        [Fact]
        public async Task ImportDataService_GetBase64FromExcel_Test()
        {
            string relativePath = @"File\FileImport.xlsx";
            string fullPath = Path.Combine(basePath ?? "", relativePath);
            string base64 = importDataService.GetBase64FromExcel(fullPath);

            Assert.NotNull(base64);
            Assert.NotEmpty(base64);
        }
        [Fact]
        public async Task ImportDataService_GetRows_Test()
        {
            string relativePath = @"File\FileImport.xlsx";
            string fullPath = Path.Combine(basePath ?? "", relativePath);
            SpreadsheetDocument document = ReadExcel(fullPath);
            List<Row> rows = importDataService.GetRows(sheet, workbookPart);
            var rowCount = rows.Count;
            document.Dispose();

            Assert.NotNull(rows);
            Assert.NotEqual(0, rowCount);
        }

        [Fact]
        public async Task ValidateExcelData_Test()
        {
            try
            {
                var mockRepo = new Mock<IDatabaseInformation>();
                string relativePath = @"File\FileImport.xlsx";
                string fullPath = Path.Combine(basePath ?? "", relativePath);
                SpreadsheetDocument document = ReadExcel(fullPath);

                WorkbookPart? workbookPart = document?.WorkbookPart;
                Sheet? sheet = workbookPart?.Workbook?.Sheets?.Elements<Sheet>().FirstOrDefault();

                List<Row>? rows = importDataService.GetRows(sheet, workbookPart);

                List<object?> lstData = new List<object?>();
                var columns = rows?[1];

                //Get Table Name
                Row? headRow = rows?[0];
                var headCells = headRow?.Elements<Cell>().ToList();

                List<TableAndColumn> mockResult = new List<TableAndColumn>();
                List<ListColumn> columnMock = new List<ListColumn>();

                columnMock.Add(new ListColumn { ColumnName = "ID", Datatype = "int", IsIdentity = "YES", IsPrimaryKey = "YES", TableName = "PERSON" });
                columnMock.Add(new ListColumn { ColumnName = "NAME", Datatype = "varchar", IsIdentity = "NO", IsPrimaryKey = "NO", TableName = "PERSON" });
                columnMock.Add(new ListColumn { ColumnName = "SURNAME", Datatype = "varchar", IsIdentity = "NO", IsPrimaryKey = "NO", TableName = "PERSON" });
                columnMock.Add(new ListColumn { ColumnName = "ADDRESS", Datatype = "int", IsIdentity = "NO", IsPrimaryKey = "NO", TableName = "PERSON" });
                columnMock.Add(new ListColumn { ColumnName = "JOB", Datatype = "int", IsIdentity = "NO", IsPrimaryKey = "NO", TableName = "PERSON" });

                mockResult.Add(new TableAndColumn { Table = "PERSON", Column = columnMock });
                mockRepo.Setup(repo => repo.GetTableAndColumns()).Returns(Task.FromResult(mockResult));
                List<TableAndColumn> lstTableAndColumn = await mockRepo.Object.GetTableAndColumns();
                string tableName = importDataService.GetCellValue(workbookPart, headCells?[1]) ?? "";
                var getTable = lstTableAndColumn.Where(x => x.Table == tableName).FirstOrDefault();
                importDataService.ValidateExcelData(tableName, rows, mockRepo.Object, workbookPart, getTable);
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false);
            }

        }
        [Fact]
        public async Task SetRowResult_Test()
        {
            try
            {
                var mockRepo = new Mock<IDatabaseInformation>();
                string relativePath = @"File\FileTest\FileImportPerson.xlsx";
                string fullPath = Path.Combine(basePath ?? "", relativePath);
                SpreadsheetDocument document = ReadExcel(fullPath);

                WorkbookPart? workbookPart = document?.WorkbookPart;
                Sheet? sheet = workbookPart?.Workbook?.Sheets?.Elements<Sheet>().FirstOrDefault();

                List<Row>? rows = importDataService.GetRows(sheet, workbookPart);

                List<object?> lstData = new List<object?>();
                var columns = rows?[1];

                //Get Table Name
                Row? headRow = rows?[0];
                var headCells = headRow?.Elements<Cell>().ToList();

                TableAndColumn mockResult = new TableAndColumn();
                List<ListColumn> columnMock = new List<ListColumn>();

                columnMock.Add(new ListColumn { ColumnName = "ID", Datatype = "int", IsIdentity = "YES", IsPrimaryKey = "YES", TableName = "PERSON" });
                columnMock.Add(new ListColumn { ColumnName = "NAME", Datatype = "varchar", IsIdentity = "NO", IsPrimaryKey = "NO", TableName = "PERSON" });
                columnMock.Add(new ListColumn { ColumnName = "SURNAME", Datatype = "varchar", IsIdentity = "NO", IsPrimaryKey = "NO", TableName = "PERSON" });
                columnMock.Add(new ListColumn { ColumnName = "ADDRESS", Datatype = "int", IsIdentity = "NO", IsPrimaryKey = "NO", TableName = "PERSON" });
                columnMock.Add(new ListColumn { ColumnName = "JOB", Datatype = "int", IsIdentity = "NO", IsPrimaryKey = "NO", TableName = "PERSON" });

                mockResult = new TableAndColumn { Table = "PERSONSS", Column = columnMock };
                // Read data rows

                Row row = rows[2];
                var cells = row.Elements<Cell>().ToList();
                RowObject rowObject = importDataService.SetRowResult(cells, workbookPart, columns, mockResult);
                Assert.NotNull(rowObject);
            }
            catch (Exception)
            {
                Assert.True(false);
            }

        }
        private SpreadsheetDocument ReadExcel(string file)
        {
            string base64 = importDataService.GetBase64FromExcel(file);
            byte[] fileAsByte = Convert.FromBase64String(base64);
            using (MemoryStream stream = new MemoryStream(fileAsByte))
            {
                SpreadsheetDocument document = SpreadsheetDocument.Open(stream, false);
                workbookPart = document?.WorkbookPart;
                sheet = workbookPart?.Workbook?.Sheets?.Elements<Sheet>().FirstOrDefault();
                return document;
            }

        }
    }
}
