using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDDDConsole.ORM
{
    public class List_Table
    {
        [Key]
        public string TableName { get; set; }
        [Key]
        public string ColumnName { get; set; }
        public string Datatype { get; set; }
        public string IsPrimaryKey { get; set; }
        public string IsIdentity { get; set; }
    }
}
