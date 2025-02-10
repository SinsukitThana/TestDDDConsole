using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TestDDDConsole.ORM;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestDDDConsole.Infrastructure.Database
{
    public class DataContext : DbContext
    {
        private IConfiguration _config;
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }//Memory Database

        public DataContext(IConfiguration config) : base()
        {
            _config = config;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured && _config != null)
            {
                var environment = _config.GetValue<string>("Environment");

                if (environment == "Test")
                {
                    optionsBuilder.UseInMemoryDatabase("TestDatabase");
                }
                else
                {
                    var connectionString = _config.GetConnectionString("DBCONNECTION");
                    optionsBuilder.UseSqlServer(connectionString);
                }
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<List_Table>().ToTable("List_Table").HasKey(item => new { item.TableName, item.ColumnName });
        }

        public virtual DbSet<List_Table> List_Table { get; set; }
    }
}
