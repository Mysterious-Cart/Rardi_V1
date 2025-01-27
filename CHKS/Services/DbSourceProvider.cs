using CHKS.Models.Enum;
using CHKS.Models.Interface;

public class DbSourceProvider : IDataSourceProvider
{

    private readonly IConfiguration _configuration;
    public DataSource CurrentDataSource { get; set; }
    public DbSourceProvider(IConfiguration configuration){
        _configuration = configuration;
        CurrentDataSource = DataSource.Client;
    }

    public string GetConnectionString()
    {
        return CurrentDataSource switch
        {
            DataSource.Client => _configuration.GetConnectionString("localConnection")!,
            DataSource.Server => _configuration.GetConnectionString("serverConnection")!,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    public DataSource GetCurrentSource()
    {
        return CurrentDataSource;
    }
}

