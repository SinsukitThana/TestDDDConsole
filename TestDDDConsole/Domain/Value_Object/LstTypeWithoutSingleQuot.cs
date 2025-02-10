using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDDDConsole.Domain.Value_Object
{
    public class LstTypeWithoutSingleQuot
    {
        private List<string>? _DataType {  get; set; }
        public List<string> DataType
        {
            get
            {
                _DataType = new List<string> { "int", "tinyint", "smallint", "bigint", "decimal", "numeric", "float", "real", "bit", "binary", "varbinary", "image" };
                return _DataType;
            }
        }
    }
}
