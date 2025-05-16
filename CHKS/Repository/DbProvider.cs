using CHKS.Models.Interface;
using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MoreLinq;
using System.Data.Common;
using System.Reflection;

public class DbProvider<Context> : IDbProvider where Context : DbContext
{
    private readonly IDbContextFactory<Context> dbContextFactory;
    private readonly Context _context;
    private readonly ILogger<DbProvider<Context>> logger;
    public DbProvider(IDbContextFactory<Context> contextFactory, ILogger<DbProvider<Context>> logger){
        this.dbContextFactory = contextFactory;
        this._context = contextFactory.CreateDbContext();
        this.logger = logger;
    }

    public async Task CreateData<TEntity>(TEntity Object) where TEntity : class{
        try
        {
            var Field = await GetPropertyOfType<Context>(typeof(DbSet<TEntity>));
            var FieldValue = Field.GetValue(_context) as DbSet<TEntity>;
            FieldValue.Add(Object);

            await _context.SaveChangesAsync();
        }
        catch (Exception exc)
        {
            logger.LogError("Failed trying to create data for {Entity} DataSet.{error_message}",typeof(TEntity).Name, exc.Message);
        }
        
    }

    public async Task<IQueryable<TEntity>> GetData<TEntity>(List<string> ToExpand = null)
        where TEntity : class
    {
        IQueryable<TEntity> Data = Enumerable.Empty<TEntity>().AsQueryable();

        try
        {

            var Field = await GetPropertyOfType<Context>(typeof(DbSet<TEntity>));
            var FieldValue = Field.GetValue(_context) as DbSet<TEntity>;

            Data = FieldValue.AsNoTracking().AsQueryable();

            //To include the specify underlying field;
            foreach (string includeProperty in ToExpand ?? [])
            {
                Data = Data.Include(includeProperty);
            }

        }
        catch (Exception Exc)
        {
            logger.LogError("Failed trying to retrieve data from {error} DataSet.",typeof(TEntity).Name);
            Console.WriteLine(Exc.Message);
        }
        
        return Data;
    }

    public async Task UpdateData<TEntity, TKey>(TEntity Object,Func<TEntity, TKey> Key_Selector) 
        where TEntity : class
    {
        try
        {
            var entries = _context.Entry(Object);
            entries.CurrentValues.SetValues(Object);
            entries.State = EntityState.Modified;

            await _context.SaveChangesAsync();

        }
        catch (DbException exc)
        {
            logger.LogCritical("Fail to update {name}. Location: UpdateData. Error: {message} ",typeof(TEntity).Name, exc.Message);

        }
    }
    
    public async Task UpdateData<TEntity, TKey>(Action<TEntity> Object, TKey key)
        where TEntity : class
    {
        try
        {
            var Field = await GetPropertyOfType<Context>(typeof(DbSet<TEntity>));
            var FieldValue = Field.GetValue(_context) as DbSet<TEntity>;
            var item = await FieldValue.FindAsync(key);

            Object(item);

            await _context.SaveChangesAsync();

        }
        catch (DbException exc)
        {
            logger.LogCritical("Fail to update {name}. Location: UpdateData. Error: {message} ", typeof(TEntity).Name, exc.Message);

        }
    }

    public async Task DeleteData<TEntity, TKey>(Func<TEntity, TKey> Key_Selector, TKey key) where TEntity : class
    {
        try
        {
            var item = await GetData<TEntity>();
            var entries = _context.Remove(item.First(i => Key_Selector(i).Equals(key)));

            await _context.SaveChangesAsync();

        }
        catch (Exception exc)
        {
            logger.LogCritical($"Fail to update {typeof(TEntity).Name}", exc);

        }
    }
    
    

    public async Task Transaction(Func<Task> action, CancellationToken cancellationToken)
    {
        try
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                await _context.Database.BeginTransactionAsync(cancellationToken);
                await action();
                await _context.Database.CommitTransactionAsync(cancellationToken);
            }

        }
        catch
        {
            await _context.Database.RollbackTransactionAsync(cancellationToken);
            logger.LogError("Transaction Failed");
        }
    }

    public IDbProvider OnDifferentDbContext()
    {
        return new DbProvider<Context>(dbContextFactory, logger);
    }

    public async Task BulkInsert<TEntity>(List<TEntity> data) where TEntity : class
    {
        try
        {
            var Field = await GetPropertyOfType<Context>(typeof(DbSet<TEntity>));
            var FieldValue = Field.GetValue(_context) as DbSet<TEntity>;
            await FieldValue.AddRangeAsync(data);

            await _context.SaveChangesAsync();
        }
        catch (Exception exc)
        {
            logger.LogError($"Failed trying to create data for {typeof(TEntity).Name} DataSet.", exc);
        }
    }
    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
        GC.Collect();
    }

    
    
    //Return a property that matches the given type
    private static async Task<PropertyInfo> GetPropertyOfType<T>(Type type)
    {
        PropertyInfo[] CurrentContextProperty = typeof(T).GetProperties();
        return CurrentContextProperty.First(i => i.PropertyType == type);

    }

}