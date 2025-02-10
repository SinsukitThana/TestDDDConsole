using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDDDConsole.ORM;

namespace TestDDDConsole.Domain.Entity
{
    public class TableAndColumn
    {
        public string Table { get; set; } = "";
        public List<ListColumn>? Column { get; set; } 
    }
    public class ListColumn
    {
        public string TableName { get; set; } = "";
        public string ColumnName { get; set; } = "";
        public string Datatype { get; set; } = "";
        public string IsPrimaryKey { get; set; } = "";
        public string IsIdentity { get; set; } = "";
    }
}
