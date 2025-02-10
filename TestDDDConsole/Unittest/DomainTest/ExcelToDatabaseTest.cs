using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDDDConsole.Application.Usecase;
using TestDDDConsole.Domain.Repository;
using TestDDDConsole.Infrastructure.Database;
using TestDDDConsole.Infrastructure.Database.Repository;
using TestDDDConsole.ORM;
using Xunit;

namespace TestDDDConsole.Unittest.DomainTest
{
    public class ExcelToDatabaseTest : IClassFixture<MemoryDatabase>
    {
        private readonly DataContext _context;
        private ExcelToDatabase excelToDatabaseRepository;

        public ExcelToDatabaseTest(MemoryDatabase memorycontext)
        {
            _context = memorycontext.Context;
            excelToDatabaseRepository = new ExcelToDatabase(_context);
        }

        //[Fact]
        //public async Task GetDataFromDatabaseTest()
        //{
        //    // Act
        //    var resultAddress = excelToDatabaseRepository.GetDataFromDatabase("ADDRESS") as IEnumerable<ADDRESS> ?? Enumerable.Empty<ADDRESS>().ToList();
        //    var resultPerson = excelToDatabaseRepository.GetDataFromDatabase("PERSON") as IEnumerable<PERSON> ?? Enumerable.Empty<PERSON>().ToList();
        //    var resultJob = excelToDatabaseRepository.GetDataFromDatabase("JOB") as IEnumerable<JOB> ?? Enumerable.Empty<JOB>().ToList();
        //    // Assert
        //    Assert.NotNull(resultAddress);
        //    Assert.Equal(2, resultAddress.ToList().Count);

        //    Assert.NotNull(resultAddress);
        //    Assert.Equal(2, resultPerson.ToList().Count);

        //    Assert.NotNull(resultJob);
        //    Assert.Equal(2, resultJob.ToList().Count);
        //}
    }
}
