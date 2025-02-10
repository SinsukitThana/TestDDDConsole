using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDDDConsole.Domain.Repository;
using TestDDDConsole.ORM;

namespace TestDDDConsole.Infrastructure.Database.Repository
{
    public class DatabaseSystem: IDatabaseSystem
    {
        private DataContext _context;
        public DatabaseSystem(DataContext context)
        {
            _context = context;
        }

        public async Task<List<List_Table>> GetAllTableName()
        {
            try
            {
                var tableList = _context.List_Table.FromSqlRaw(@"SELECT tb.TABLE_NAME as 'TableName',col.COLUMN_NAME as 'ColumnName',col.DATA_TYPE as 'Datatype',
                                                                        CASE WHEN pk.is_identity = 1 THEN 'YES' ELSE 'NO' END AS IsIdentity,
                                                                        CASE WHEN pk.is_primary_key = 1 THEN 'YES' ELSE 'NO' END AS IsPrimaryKey
                                                                 FROM
                                                                    (SELECT TABLE_NAME
                                                                     FROM INFORMATION_SCHEMA.TABLES
                                                                     WHERE TABLE_TYPE = 'BASE TABLE') tb
                                                                 LEFT JOIN
                                                                    (SELECT TABLE_NAME,COLUMN_NAME, DATA_TYPE
                                                                    FROM INFORMATION_SCHEMA.COLUMNS) col
                                                                 ON tb.TABLE_NAME = col.TABLE_NAME
                                                                 LEFT JOIN
                                                                    (SELECT 
                                                                        t.name AS TableName,
                                                                        c.name AS ColumnName,
                                                                 	    i.is_primary_key ,
                                                                        c.is_identity 
                                                                     FROM sys.tables t
                                                                     JOIN sys.columns c ON t.object_id = c.object_id
                                                                     JOIN sys.index_columns ic ON c.object_id = ic.object_id AND c.column_id = ic.column_id
                                                                     JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id
                                                                    ) pk
                                                                 ON pk.TableName = tb.TABLE_NAME AND pk.ColumnName = col.COLUMN_NAME ").ToList();

                return tableList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
