namespace CHKS.Mappers;
using Models;
using Entity;
using Enum;
using System.Linq.Expressions;
public class StockLogsExpressionMapper
{
    public static Expression<Func<StockLogsModel, StockLogs>> ToStockLogs()
    {
        return stockLogsModel => new StockLogs
        {
            Id = stockLogsModel.Id,
            EmployeeId = stockLogsModel.EmployeeId,
            Type = stockLogsModel.Type, // Assuming Type is an enum in StockLogsModel
            Date = stockLogsModel.Date
        };
    }
}
public class StockLogsMapper
{
    public static StockLogs ToStockLogs(StockLogsModel stockLogsModel)
    {
        return new StockLogs
        {
            Id = stockLogsModel.Id,
            EmployeeId = stockLogsModel.EmployeeId,
            Type = stockLogsModel.Type, // Assuming Type is an enum in StockLogsModel
            Date = stockLogsModel.Date
        };
    }

    public static IEnumerable<StockLogs> ToStockLogs(IEnumerable<StockLogsModel> stockLogsModels)
    {
        return stockLogsModels.Select(ToStockLogs);
    }
}