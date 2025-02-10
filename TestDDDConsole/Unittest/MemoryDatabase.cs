using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDDDConsole.Infrastructure.Database;
using TestDDDConsole.ORM;

namespace TestDDDConsole.Unittest
{
    public class MemoryDatabase : IDisposable
    {
        public DataContext Context { get; private set; }
        public MemoryDatabase()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // ใช้ In-Memory DB
                .Options;

            Context = new DataContext(options);
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
