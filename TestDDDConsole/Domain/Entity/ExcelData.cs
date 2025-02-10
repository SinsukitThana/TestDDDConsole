using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDDDConsole.ORM;

namespace TestDDDConsole.Domain.Entity
{

    public class TableObject
    {
        public string TableName { get; set; } = "";
        public List<RowObject>? Rows { get; set; }
    }

    public enum ActionMode
    {
        INSERT,
        UPDATE,
        DELETE
    }

    public class RowObject
    {
        public List<ColumnObject>? Columns { get; set; }
        public ActionMode Mode { get; set; }
    }

    public class ColumnObject
    {
        public string ColumnName { get; set; } = "";
        public string Value { get; set; } = "";
        public string DbType { get; set; } = "";
        public bool IsPrimaryKey { get; set; }
        public bool IsIdentity { get; set; }
    }

    public class QueryDatabase
    {
        public string Query { get; set; } = "";

    }
}
