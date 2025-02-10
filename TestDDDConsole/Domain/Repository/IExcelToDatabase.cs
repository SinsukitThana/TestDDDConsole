using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDDDConsole.Domain.Entity;
using TestDDDConsole.ORM;

namespace TestDDDConsole.Domain.Repository
{
    public interface IExcelToDatabase
    {
        Task ExcuteQuery(List<string> query);
        List<string> GenerateQuery(ActionMode mode, List<RowObject> rows, string tableName);
        string CreateQuery(RowObject row, string tableName);
        string PhaseValue(string data, ColumnObject columns);
        string UpdateQuery(RowObject row, string tableName);
    }
}
