namespace CHKS.Models.Interface;
using CHKS.Models.Enum;
public interface IDataSourceProvider
{
    DataSource CurrentDataSource { set; }
    string GetConnectionString();

    DataSource GetCurrentSource();
}