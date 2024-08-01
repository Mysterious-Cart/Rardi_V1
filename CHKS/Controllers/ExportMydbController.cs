using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using CHKS.Data;

namespace CHKS.Controllers
{
    public partial class ExportmydbController : ExportController
    {
        private readonly mydbContext context;
        private readonly mydbService service;

        public ExportmydbController(mydbContext context, mydbService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/mydb/cars/csv")]
        [HttpGet("/export/mydb/cars/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCarsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCars(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/cars/excel")]
        [HttpGet("/export/mydb/cars/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCarsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCars(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/carbrands/csv")]
        [HttpGet("/export/mydb/carbrands/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCarBrandsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCarBrands(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/carbrands/excel")]
        [HttpGet("/export/mydb/carbrands/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCarBrandsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCarBrands(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/carts/csv")]
        [HttpGet("/export/mydb/carts/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCartsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCarts(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/carts/excel")]
        [HttpGet("/export/mydb/carts/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCartsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCarts(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/cashbacks/csv")]
        [HttpGet("/export/mydb/cashbacks/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCashbacksToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCashbacks(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/cashbacks/excel")]
        [HttpGet("/export/mydb/cashbacks/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCashbacksToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCashbacks(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/connectors/csv")]
        [HttpGet("/export/mydb/connectors/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportConnectorsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetConnectors(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/connectors/excel")]
        [HttpGet("/export/mydb/connectors/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportConnectorsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetConnectors(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/dailyexpenses/csv")]
        [HttpGet("/export/mydb/dailyexpenses/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDailyexpensesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetDailyexpenses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/dailyexpenses/excel")]
        [HttpGet("/export/mydb/dailyexpenses/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDailyexpensesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetDailyexpenses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/histories/csv")]
        [HttpGet("/export/mydb/histories/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHistoriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHistories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/histories/excel")]
        [HttpGet("/export/mydb/histories/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHistoriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHistories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/historyconnectors/csv")]
        [HttpGet("/export/mydb/historyconnectors/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHistoryconnectorsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetHistoryconnectors(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/historyconnectors/excel")]
        [HttpGet("/export/mydb/historyconnectors/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportHistoryconnectorsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetHistoryconnectors(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/inventories/csv")]
        [HttpGet("/export/mydb/inventories/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportInventoriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetInventories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/inventories/excel")]
        [HttpGet("/export/mydb/inventories/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportInventoriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetInventories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/inventorycaroptions/csv")]
        [HttpGet("/export/mydb/inventorycaroptions/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportInventoryCaroptionsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetInventoryCaroptions(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/inventorycaroptions/excel")]
        [HttpGet("/export/mydb/inventorycaroptions/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportInventoryCaroptionsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetInventoryCaroptions(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/inventoryoptions/csv")]
        [HttpGet("/export/mydb/inventoryoptions/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportInventoryOptionsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetInventoryOptions(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/inventoryoptions/excel")]
        [HttpGet("/export/mydb/inventoryoptions/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportInventoryOptionsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetInventoryOptions(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/inventoryproductgroups/csv")]
        [HttpGet("/export/mydb/inventoryproductgroups/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportInventoryProductgroupsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetInventoryProductgroups(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/inventoryproductgroups/excel")]
        [HttpGet("/export/mydb/inventoryproductgroups/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportInventoryProductgroupsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetInventoryProductgroups(), Request.Query, false), fileName);
        }
    }
}
