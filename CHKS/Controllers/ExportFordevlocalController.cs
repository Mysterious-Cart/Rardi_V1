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

        [HttpGet("/export/fordevlocal/efmigrationshistories/csv")]
        [HttpGet("/export/fordevlocal/efmigrationshistories/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEfmigrationshistoriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetEfmigrationshistories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/efmigrationshistories/excel")]
        [HttpGet("/export/fordevlocal/efmigrationshistories/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEfmigrationshistoriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetEfmigrationshistories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/aspnetroleclaims/csv")]
        [HttpGet("/export/fordevlocal/aspnetroleclaims/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetroleclaimsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspnetroleclaims(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/aspnetroleclaims/excel")]
        [HttpGet("/export/fordevlocal/aspnetroleclaims/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetroleclaimsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspnetroleclaims(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/aspnetroles/csv")]
        [HttpGet("/export/fordevlocal/aspnetroles/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetrolesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspnetroles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/aspnetroles/excel")]
        [HttpGet("/export/fordevlocal/aspnetroles/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetrolesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspnetroles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/aspnetuserclaims/csv")]
        [HttpGet("/export/fordevlocal/aspnetuserclaims/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetuserclaimsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspnetuserclaims(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/aspnetuserclaims/excel")]
        [HttpGet("/export/fordevlocal/aspnetuserclaims/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetuserclaimsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspnetuserclaims(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/aspnetuserlogins/csv")]
        [HttpGet("/export/fordevlocal/aspnetuserlogins/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetuserloginsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspnetuserlogins(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/aspnetuserlogins/excel")]
        [HttpGet("/export/fordevlocal/aspnetuserlogins/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetuserloginsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspnetuserlogins(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/aspnetuserroles/csv")]
        [HttpGet("/export/fordevlocal/aspnetuserroles/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetuserrolesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspnetuserroles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/aspnetuserroles/excel")]
        [HttpGet("/export/fordevlocal/aspnetuserroles/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetuserrolesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspnetuserroles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/aspnetusers/csv")]
        [HttpGet("/export/fordevlocal/aspnetusers/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetusersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspnetusers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/aspnetusers/excel")]
        [HttpGet("/export/fordevlocal/aspnetusers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetusersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspnetusers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/aspnetusertokens/csv")]
        [HttpGet("/export/fordevlocal/aspnetusertokens/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetusertokensToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspnetusertokens(), Request.Query, false), fileName);
        }

        [HttpGet("/export/fordevlocal/aspnetusertokens/excel")]
        [HttpGet("/export/fordevlocal/aspnetusertokens/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspnetusertokensToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspnetusertokens(), Request.Query, false), fileName);
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
