using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDDDConsole.Domain.Entity;
using TestDDDConsole.Domain.Repository;
using TestDDDConsole.ORM;
using static TestDDDConsole.Infrastructure.Database.Repository.ExcelToDatabase;

namespace TestDDDConsole.Application.Usecase
{
    public interface IDatabaseInformation
    {
        Task<List<TableAndColumn>> GetTableAndColumns();
    }
    public class DatabaseInformation : IDatabaseInformation
    {
        private IDatabaseSystem repository;

        public class DatabaseInformationUsecaseException : Exception
        {
            public DatabaseInformationUsecaseException(string? message) : base(message)
            { }

        }
        public DatabaseInformation(IDatabaseSystem repository)
        {
            this.repository = repository;
        }

        public async Task<List<TableAndColumn>> GetTableAndColumns()
        {
            try
            {
                List<TableAndColumn> result = new List<TableAndColumn>();
                List<List_Table> lstTable = await repository.GetAllTableName();
                var lstTableName = lstTable.Select(x => x.TableName).Distinct().ToList();
                foreach (var table in lstTableName)
                {

                    List<ListColumn> lstColumn = lstTable.Where(x => x.TableName == table).Select(x => new ListColumn { ColumnName = x.ColumnName, Datatype = x.Datatype, IsIdentity = x.IsIdentity, IsPrimaryKey = x.IsPrimaryKey }).ToList();
                    result.Add(new TableAndColumn { Table = table, Column = lstColumn });
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new DatabaseInformationUsecaseException(ex.Message);
            }
        }
    }
}
