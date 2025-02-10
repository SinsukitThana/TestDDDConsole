using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDDDConsole.Domain.Entity;

namespace TestDDDConsole.ORM
{
    public class ResultData
    {
        public string Error { get; set; } = string.Empty;
        public TableObject Data { get; set; } = null;
    }

}
