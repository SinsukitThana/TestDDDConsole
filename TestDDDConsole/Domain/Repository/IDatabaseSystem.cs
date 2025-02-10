using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDDDConsole.ORM;

namespace TestDDDConsole.Domain.Repository
{
    public interface IDatabaseSystem
    {
        Task<List<List_Table>> GetAllTableName();
    }
}
