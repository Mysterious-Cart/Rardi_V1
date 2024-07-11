using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using CHKS.Data;

namespace CHKS.Controllers
{
    public partial class ExportfordevlocalController : ExportController
    {
        private readonly fordevlocalContext context;
        private readonly fordevlocalService service;

        public ExportfordevlocalController(fordevlocalContext context, fordevlocalService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/fordevlocal/cars/csv")]
        [HttpGet("/export/fordevlocal/cars/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCarsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCars(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/cars/excel")]
        [HttpGet("/export/fordevlocal/cars/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCarsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCars(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/carts/csv")]
        [HttpGet("/export/fordevlocal/carts/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCartsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCarts(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/carts/excel")]
        [HttpGet("/export/fordevlocal/carts/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCartsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCarts(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/connectors/csv")]
        [HttpGet("/export/fordevlocal/connectors/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportConnectorsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetConnectors(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/connectors/excel")]
        [HttpGet("/export/fordevlocal/connectors/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportConnectorsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetConnectors(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/dailyexpenses/csv")]
        [HttpGet("/export/fordevlocal/dailyexpenses/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDailyexpensesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetDailyexpenses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/dailyexpenses/excel")]
        [HttpGet("/export/fordevlocal/dailyexpenses/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDailyexpensesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetDailyexpenses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/histories/csv")]
        [HttpGet("/export/fordevlocal/histories/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHistoriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHistories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/histories/excel")]
        [HttpGet("/export/fordevlocal/histories/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHistoriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHistories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/historyconnectors/csv")]
        [HttpGet("/export/fordevlocal/historyconnectors/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHistoryconnectorsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHistoryconnectors(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/historyconnectors/excel")]
        [HttpGet("/export/fordevlocal/historyconnectors/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHistoryconnectorsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHistoryconnectors(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/inventories/csv")]
        [HttpGet("/export/fordevlocal/inventories/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportInventoriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetInventories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/inventories/excel")]
        [HttpGet("/export/fordevlocal/inventories/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportInventoriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetInventories(), Request.Query, false), fileName);
        }
    }
}
