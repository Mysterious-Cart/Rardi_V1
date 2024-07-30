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

        [HttpGet("/export/mydb/efmigrationshistories/csv")]
        [HttpGet("/export/mydb/efmigrationshistories/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEfmigrationshistoriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetEfmigrationshistories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/efmigrationshistories/excel")]
        [HttpGet("/export/mydb/efmigrationshistories/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEfmigrationshistoriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetEfmigrationshistories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/aspnetroleclaims/csv")]
        [HttpGet("/export/mydb/aspnetroleclaims/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetroleclaimsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspnetroleclaims(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/aspnetroleclaims/excel")]
        [HttpGet("/export/mydb/aspnetroleclaims/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetroleclaimsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspnetroleclaims(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/aspnetroles/csv")]
        [HttpGet("/export/mydb/aspnetroles/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetrolesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspnetroles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/aspnetroles/excel")]
        [HttpGet("/export/mydb/aspnetroles/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetrolesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspnetroles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/aspnetuserclaims/csv")]
        [HttpGet("/export/mydb/aspnetuserclaims/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetuserclaimsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspnetuserclaims(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/aspnetuserclaims/excel")]
        [HttpGet("/export/mydb/aspnetuserclaims/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetuserclaimsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspnetuserclaims(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/aspnetuserlogins/csv")]
        [HttpGet("/export/mydb/aspnetuserlogins/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetuserloginsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspnetuserlogins(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/aspnetuserlogins/excel")]
        [HttpGet("/export/mydb/aspnetuserlogins/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetuserloginsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspnetuserlogins(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/aspnetuserroles/csv")]
        [HttpGet("/export/mydb/aspnetuserroles/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetuserrolesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspnetuserroles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/aspnetuserroles/excel")]
        [HttpGet("/export/mydb/aspnetuserroles/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetuserrolesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspnetuserroles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/aspnetusers/csv")]
        [HttpGet("/export/mydb/aspnetusers/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetusersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspnetusers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/aspnetusers/excel")]
        [HttpGet("/export/mydb/aspnetusers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetusersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspnetusers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/aspnetusertokens/csv")]
        [HttpGet("/export/mydb/aspnetusertokens/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetusertokensToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspnetusertokens(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/aspnetusertokens/excel")]
        [HttpGet("/export/mydb/aspnetusertokens/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetusertokensToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspnetusertokens(), Request.Query, false), fileName);
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

        [HttpGet("/export/mydb/inventorytrashcans/csv")]
        [HttpGet("/export/mydb/inventorytrashcans/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportInventoryTrashcansToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetInventoryTrashcans(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/inventorytrashcans/excel")]
        [HttpGet("/export/mydb/inventorytrashcans/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportInventoryTrashcansToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetInventoryTrashcans(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/productclasses/csv")]
        [HttpGet("/export/mydb/productclasses/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportProductClassesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetProductClasses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/mydb/productclasses/excel")]
        [HttpGet("/export/mydb/productclasses/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportProductClassesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetProductClasses(), Request.Query, false), fileName);
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
