using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDDDConsole.Domain.Entity;
using TestDDDConsole.Domain.Repository;
using TestDDDConsole.ORM;

namespace TestDDDConsole.Application.Usecase
{
    public interface IImportData
    {
        Task<bool> InsertUpdateDeleteData(TableObject data);
    }
    public class ImportData : IImportData
    {
        private IExcelToDatabase repository;
        public ImportData(IExcelToDatabase repository)
        {
            this.repository = repository;
        }

        public async Task<bool> InsertUpdateDeleteData(TableObject data)
        {
            try
            {
                List<ActionMode> lstMode = new List<ActionMode> { ActionMode.INSERT, ActionMode.UPDATE, ActionMode.DELETE };

                foreach (ActionMode mode in lstMode)
                {
                    List<RowObject>? rows = data.Rows?.Where(x => x.Mode == mode).ToList();
                    List<string> query = repository.GenerateQuery(mode, rows, data.TableName);
                    await repository.ExcuteQuery(query);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
