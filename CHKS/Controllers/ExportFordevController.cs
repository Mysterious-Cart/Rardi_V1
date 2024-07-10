using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using CHKS.Data;

namespace CHKS.Controllers
{
    public partial class ExportfordevController : ExportController
    {
        private readonly fordevContext context;
        private readonly fordevService service;

        public ExportfordevController(fordevContext context, fordevService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/fordev/cars/csv")]
        [HttpGet("/export/fordev/cars/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCarsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCars(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordev/cars/excel")]
        [HttpGet("/export/fordev/cars/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCarsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCars(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordev/carts/csv")]
        [HttpGet("/export/fordev/carts/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCartsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCarts(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordev/carts/excel")]
        [HttpGet("/export/fordev/carts/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCartsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCarts(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordev/connectors/csv")]
        [HttpGet("/export/fordev/connectors/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportConnectorsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetConnectors(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordev/connectors/excel")]
        [HttpGet("/export/fordev/connectors/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportConnectorsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetConnectors(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordev/dailyexpenses/csv")]
        [HttpGet("/export/fordev/dailyexpenses/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDailyexpensesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetDailyexpenses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordev/dailyexpenses/excel")]
        [HttpGet("/export/fordev/dailyexpenses/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDailyexpensesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetDailyexpenses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordev/expensehistoryconnectors/csv")]
        [HttpGet("/export/fordev/expensehistoryconnectors/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportExpensehistoryconnectorsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetExpensehistoryconnectors(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordev/expensehistoryconnectors/excel")]
        [HttpGet("/export/fordev/expensehistoryconnectors/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportExpensehistoryconnectorsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetExpensehistoryconnectors(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordev/histories/csv")]
        [HttpGet("/export/fordev/histories/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHistoriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHistories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordev/histories/excel")]
        [HttpGet("/export/fordev/histories/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHistoriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHistories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordev/historyconnectors/csv")]
        [HttpGet("/export/fordev/historyconnectors/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHistoryconnectorsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHistoryconnectors(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordev/historyconnectors/excel")]
        [HttpGet("/export/fordev/historyconnectors/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHistoryconnectorsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHistoryconnectors(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordev/inventories/csv")]
        [HttpGet("/export/fordev/inventories/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportInventoriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetInventories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordev/inventories/excel")]
        [HttpGet("/export/fordev/inventories/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportInventoriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetInventories(), Request.Query, false), fileName);
        }
    }
}
