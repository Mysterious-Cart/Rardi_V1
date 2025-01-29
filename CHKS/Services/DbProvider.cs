using CHKS.Data;
using CHKS.Models.Interface;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MudBlazor.Extensions;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

public class DbProvider<Context> : IDbProvider where Context : DbContext
{
    private readonly Context _context;
    private readonly ILogger<DbProvider<Context>> logger;
    public DbProvider(Context context, ILogger<DbProvider<Context>> logger){
        this._context = context;
        this.logger = logger;
    }

    public async Task<IQueryable<TEntity>> GetData<TEntity>(List<string> ToExpand = null) 
        where TEntity : class, IModelClass
    {
        IQueryable<TEntity> Data = null;

        try{
            //To Check if have connection to db
            if(await _context.Database.CanConnectAsync()){
                
                var Field = await GetPropertyOfType<Context>(typeof(DbSet<TEntity>));
                var FieldValue = Field.GetValue(_context) as DbSet<TEntity>;

                Data = FieldValue.AsNoTracking().AsQueryable();
                
                //To include the specify underlying field;
                foreach(string i in ToExpand??[]){
                    Data = Data.Include(i);
                }
                
            }else{
                logger.LogCritical("Not connected to any database");
            }
            
        }catch(Exception Exc){
            logger.LogError($"Failed trying to retrieve data from {typeof(TEntity).Name} DataSet.", Exc);
        }finally{
            
        }
        
        if(Data is not null)
        {
            logger.LogInformation($"Successfully retrieve from {typeof(TEntity).Name} DataSet. ");
        }
        return Data;
    }

    public async Task UpdateData<T, TKey>(T Object,Func<T, TKey> Key_Selector, bool ConfirmExistance = true) 
        where T : class, IModelClass
    {
        try{
            
            if(ConfirmExistance) {await Confirm(Key_Selector, Key_Selector(Object));}
            
            var entries = _context.Entry(Object);
            entries.CurrentValues.SetValues(Object);
            entries.State = EntityState.Modified;
            _context.SaveChanges();
            
        }catch(Exception exc){
            logger.LogCritical("Item doesn't exist");
        }
        
        
    }

    public async Task DeleteData<T, TKey>(Func<T, TKey> Key_Selector, TKey key, bool ConfirmExistance = true) where T : class, IModelClass
    {  
        try{
            
            if(ConfirmExistance)
            {
                var itemFound = Confirm(Key_Selector, key);
                var entries = _context.Remove(itemFound);
            }else{
                var item = await GetData<T>();
                var entries = _context.Remove(item.First(i => Key_Selector(i).Equals(key)));
            }
            
            _context.SaveChanges();
            
        }catch(Exception exc){
            logger.LogCritical("Item doesn't exist");
        }
    }

    //Return a property that matches the given type
    private static async Task<PropertyInfo> GetPropertyOfType<T>( Type type) 
    {
        PropertyInfo[] CurrentContextProperty = typeof(T).GetProperties();
        return CurrentContextProperty.First(i => i.PropertyType == type);
        
    }

    private async Task<T> Confirm<T, Tkey>(Func<T, Tkey> keyselector, Tkey key) where T: class, IModelClass{
        var data = await GetData<T>();
        var item = data.ToList().First(i => keyselector(i).Equals(key));
        if(item is null){
            throw new Exception("Item doesn't Exist");
        }
        return item;
    }
    
}

public static class providerExtension { 
    public static async void Update<T>(this T item, IDbProvider provider) where T : class, IModelClass
    {
        var properties = typeof(Item).GetProperties();
        PropertyInfo keyprop = null;
        foreach(var i in properties)
        {
            if(i.GetCustomAttribute(typeof(KeyAttribute), false) is not null)
            {
                keyprop = i;
                break;
            }
        }
        if (keyprop is not null)
        {
           
        }
        
    }

}
