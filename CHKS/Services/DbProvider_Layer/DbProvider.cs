using CHKS.Data;
using CHKS.Models.Interface;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Connections;
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

    public async Task CreateData<T>(T Object){
        try{
            var entries = _context.Entry(Object);
            entries.State = EntityState.Added;
            await _context.SaveChangesAsync();
        }catch{
            
        }
        
    }

    public async Task<IQueryable<TEntity>> GetData<TEntity>(List<string> ToExpand = null) 
        where TEntity : class
    {
        IQueryable<TEntity> Data = null;

        try{
                
            var Field = await GetPropertyOfType<Context>(typeof(DbSet<TEntity>));
            var FieldValue = Field.GetValue(_context) as DbSet<TEntity>;

            Data = FieldValue.AsNoTracking().AsQueryable();
            
            //To include the specify underlying field;
            foreach(string i in ToExpand??[]){
                Data = Data.Include(i);
            }
            
            
        }catch(Exception Exc){
            logger.LogError($"Failed trying to retrieve data from {typeof(TEntity).Name} DataSet.", Exc);
        }finally{
            
        }

        return Data;
    }

    public async Task UpdateData<T, TKey>(T Object,Func<T, TKey> Key_Selector, bool ConfirmExistance = true) 
        where T : class
    {
        try{
            
            if(ConfirmExistance) await Confirm(Key_Selector, Key_Selector(Object));
            
            var entries = _context.Entry(Object);
            entries.CurrentValues.SetValues(Object);
            entries.State = EntityState.Modified;
            _context.SaveChanges();
            
        }catch(Exception exc){
            logger.LogCritical("Item doesn't exist");
        }
        
    }
    public async Task DeleteData<T, TKey>(Func<T, TKey> Key_Selector, TKey key, bool ConfirmExistance = true) where T : class
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

    private async Task<T> Confirm<T, Tkey>(Func<T, Tkey> keyselector, Tkey key) where T: class{
        var data = await GetData<T>();
        var item = data.ToList().First(i => keyselector(i).Equals(key));
        if(item is null){
            throw new Exception("Item doesn't Exist");
        }
        return item;
    }
    
}