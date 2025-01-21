using CHKS.Data;
using CHKS.Models;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

public class DbProvider : IDbProvider
{
    private readonly mydbContext context;
    public DbProvider(mydbContext context){
        this.context = context;
    }

    public Task<IQueryable<T>> GetData<T>() 
        where T : class, IModelClass
    {
        Type GivenModel = typeof(DbSet<T>);
        Type CurrentContext = typeof(mydbContext);
        
        FieldInfo[] CurrentContextField = CurrentContext.GetFields();
        var Field = CurrentContextField.First(i => i.FieldType == GivenModel);
        var FieldValue = Field.GetValue(context) as DbSet<T>;
        var Data = FieldValue.AsQueryable();

        return Task.FromResult(Data) ;
    }
}