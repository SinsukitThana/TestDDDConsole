using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using TestDDDConsole.Application.Usecase;
using TestDDDConsole.Domain.Entity;
using TestDDDConsole.Domain.Repository;
using TestDDDConsole.ORM;
using TestDDDConsole.Infrastructure.Database.Repository;
using TestDDDConsole.Infrastructure.Database;

namespace TestDDDConsole.Unittest.UsecaseTest
{
    public class ImportDataTests: IClassFixture<MemoryDatabase>
    {
        private readonly DataContext _context;
        private readonly IExcelToDatabase _mockRepository;
        private readonly ImportData _importData;

        public ImportDataTests(MemoryDatabase memorycontext)
        {
            _context = memorycontext.Context;
            _mockRepository = new ExcelToDatabase(_context);
            _importData = new ImportData(_mockRepository);
        }

        [Fact]
        public async Task InsertUpdateDeleteData_WithValidData_ReturnsTrue()
        {
            //try
            //{
            //    // Arrange
            //    var fakeData = new lstImportData
            //    {
            //        List_Insert = new List<object?> { new PERSON { ID = 1, NAME = "John" } },
            //        List_Update = new List<object?> { new JOB { ID = 2, VALUE = "Developer" } },
            //        List_Delete = new List<object?> { new ADDRESS { ID = 3, VALUE = "New York" } }
            //    };

            //    // Act
            //    var result = await _importData.InsertUpdateDeleteData(fakeData);
            //    var address = _context.ADDRESS.ToList();
            //    var person = _context.PERSON.ToList();
            //    var job = _context.JOB.ToList();
            //    // Assert
            //    Assert.True(result);
            //    Assert.Equal(3, address.ToList().Count());
            //    Assert.Equal(3, person.Count());
            //    Assert.Equal(3, job.Count());

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Test Failed: {ex.ToString}");
            //    throw;
            //}
        }
    }
}
