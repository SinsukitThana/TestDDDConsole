using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDDDConsole.Application.Service;
using TestDDDConsole.Application.Usecase;
using TestDDDConsole.Domain.Entity;
using TestDDDConsole.Domain.Repository;
using TestDDDConsole.Infrastructure.Database;
using TestDDDConsole.Infrastructure.Database.Repository;
using TestDDDConsole.ORM;
using Xunit;

namespace TestDDDConsole.Unittest.UsecaseTest
{
    public class DatabaseInformationTest : IClassFixture<MemoryDatabase>
    {
        private readonly DataContext _context;
        private readonly DatabaseInformation _databaseInformation;
        public DatabaseInformationTest(MemoryDatabase memorycontext)
        {
            _context = memorycontext.Context;
            IDatabaseSystem repository = new DatabaseSystem(_context);
            _databaseInformation = new DatabaseInformation(repository);
        }

        [Fact]
        public async Task GetTableAndColumns_ReturnsCorrectData()
        {
            try
            {
                // Act
                var result = await _databaseInformation.GetTableAndColumns();

                var resultAddress = result.Where(x => x.Table == "ADDRESS").FirstOrDefault();
                // Assert

                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                Assert.NotNull(resultAddress);
                Assert.Equal(2, resultAddress.Column.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test Failed: {ex.ToString}");
                throw;
            }
        }

    }
}
