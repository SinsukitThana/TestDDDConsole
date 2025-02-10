using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDDDConsole.Domain.Entity;
using TestDDDConsole.Domain.Repository;
using TestDDDConsole.Domain.Value_Object;
using TestDDDConsole.ORM;

namespace TestDDDConsole.Infrastructure.Database.Repository
{
    public class ExcelToDatabase : IExcelToDatabase
    {
        private DataContext _context;

        public class ExcelToDatabaseInfrastrueException : Exception
        {
            public ExcelToDatabaseInfrastrueException(string? message) : base(message)
            { }

        }
        public ExcelToDatabase(DataContext context)
        {
            _context = context;
        }
        public async Task ExcuteQuery(List<string> lstQuery)
        {
            //string.Join(Environment.NewLine, lstResult)
            try
            {

                foreach (var query in lstQuery)
                {
                    int affectedRows = await _context.Database.ExecuteSqlRawAsync(query);
                    Console.WriteLine(query);
                    //if (affectedRows == 0)
                    //{
                        //throw new ExcelToDatabaseInfrastrueException("affectedRows 0");
                    //}
                }
            }
            catch (Exception ex)
            {
                throw new ExcelToDatabaseInfrastrueException(ex.Message);
            }
        }
        public List<string> GenerateQuery(ActionMode mode, List<RowObject> rows, string tableName)
        {
            LstTypeWithoutSingleQuot getLstTypeWithoutSingleQuot = new LstTypeWithoutSingleQuot();
            List<string> lstTypeWithoutSingleQuot = getLstTypeWithoutSingleQuot.DataType;
            List<string> lstResult = new List<string>();
            //สร้าง Create
            foreach (RowObject row in rows)
            {
                if (mode == ActionMode.INSERT)
                {
                    lstResult.Add(CreateQuery(row, tableName));
                }
                else if (mode == ActionMode.UPDATE)
                {
                    lstResult.Add(UpdateQuery(row, tableName));
                }
                else if (mode == ActionMode.DELETE)
                {
                    lstResult.Add(DeleteQuery(row, tableName));
                }
            }
            return lstResult;
        }
        public string PhaseValue(string data, ColumnObject columns)
        {
            LstTypeWithoutSingleQuot getLstTypeWithoutSingleQuot = new LstTypeWithoutSingleQuot();
            List<string> lstTypeWithoutSingleQuot = getLstTypeWithoutSingleQuot.DataType;
            if (lstTypeWithoutSingleQuot.Any(x => x == columns.DbType))
            {
                data = columns.Value;
            }
            else
            {
                data = "'" + columns.Value + "'";
            }
            return data;
        }
        public string CreateQuery(RowObject row, string tableName)
        {
            LstTypeWithoutSingleQuot getLstTypeWithoutSingleQuot = new LstTypeWithoutSingleQuot();
            List<string> lstTypeWithoutSingleQuot = getLstTypeWithoutSingleQuot.DataType;
            List<string> lstColumnInsert = new List<string>();
            List<string> lstParameterInsert = new List<string>();

            //วนข้อมูลเพื่อสร้าง Query Header และ Data
            foreach (ColumnObject columns in row.Columns.Where(x => !x.IsIdentity).ToList())
            {
                lstColumnInsert.Add(columns.ColumnName);
                lstParameterInsert.Add(PhaseValue(columns.Value, columns));
            }

            //จับรวม สร้าง Query
            string query = "INSERT INTO " + tableName + " ";
            string columnsQuery = "(" + string.Join(",", lstColumnInsert) + ")";
            string parameterInsert = "(" + string.Join(",", lstParameterInsert) + ")";

            query += columnsQuery + " VALUES " + parameterInsert + ";";

            return query;
        }
        public string UpdateQuery(RowObject row, string tableName)
        {
            LstTypeWithoutSingleQuot getLstTypeWithoutSingleQuot = new LstTypeWithoutSingleQuot();
            List<string> lstTypeWithoutSingleQuot = getLstTypeWithoutSingleQuot.DataType;
            List<string> lstUpdate = new List<string>();
            List<string> lstWhere = new List<string>();

            //วนข้อมูลเพื่อสร้าง Update Query
            foreach (ColumnObject columns in row.Columns.Where(x => !x.IsPrimaryKey).ToList())
            {
                string updateQuery = "";
                string valueUpdate = PhaseValue(columns.Value, columns);

                updateQuery = columns.ColumnName + " = " + valueUpdate;
                lstUpdate.Add(updateQuery);
            }
            //วนข้อมูลเพื่อสร้าง Where Query
            foreach (ColumnObject columns in row.Columns.Where(x => x.IsPrimaryKey).ToList())
            {
                string whereQuery = "";
                string valueUpdate = PhaseValue(columns.Value, columns);
                whereQuery = columns.ColumnName + " = " + valueUpdate;
                lstWhere.Add(whereQuery);
            }

            //จับรวม สร้าง Query
            string query = "UPDATE " + tableName + " SET ";
            string valueUpdateQuery = string.Join(",", lstUpdate);
            string whereUpdateQuery = string.Join(" AND ", lstWhere);
            query += valueUpdateQuery + " WHERE " + whereUpdateQuery + ";";

            return query;
        }
        public string DeleteQuery(RowObject row, string tableName)
        {
            LstTypeWithoutSingleQuot getLstTypeWithoutSingleQuot = new LstTypeWithoutSingleQuot();
            List<string> lstTypeWithoutSingleQuot = getLstTypeWithoutSingleQuot.DataType;

            List<string> lstWhere = new List<string>();

            //วนข้อมูลเพื่อสร้าง Where Query
            foreach (ColumnObject columns in row.Columns.Where(x => x.IsPrimaryKey).ToList())
            {
                string whereQuery = "";
                string valueDelete = PhaseValue(columns.Value, columns);
                whereQuery = columns.ColumnName + " = " + valueDelete;
                lstWhere.Add(whereQuery);
            }

            //จับรวม สร้าง Query
            string query = "DELETE FROM " + tableName;
            string whereDeleteQuery = string.Join(" AND ", lstWhere);
            query += " WHERE " + whereDeleteQuery + ";";

            return query;
        }

    }
}
