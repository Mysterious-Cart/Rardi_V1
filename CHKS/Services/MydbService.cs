using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;

using CHKS.Data;

namespace CHKS
{
    public partial class mydbService
    {
        mydbContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly mydbContext context;
        private readonly NavigationManager navigationManager;

        public mydbService(mydbContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportCarsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/cars/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/cars/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCarsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/cars/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/cars/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCarsRead(ref IQueryable<CHKS.Models.mydb.Car> items);

        public async Task<IQueryable<CHKS.Models.mydb.Car>> GetCars(Query query = null)
        {
            var items = Context.Cars.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCarsRead(ref items);

            return await Task.FromResult(items);
        }

        public async Task<IQueryable<Models.mydb.Tags>> GetTags(Query query = null){
            var items = Context.Tags.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            return await Task.FromResult(items);
        }

        public async Task<Models.mydb.Tags> CreateTag(CHKS.Models.mydb.Tags Tags)
        {         

            try
            {
                Context.Tags.Add(Tags);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(Tags).State = EntityState.Detached;
                throw;
            }

            return Tags;
        }

         public async Task<CHKS.Models.mydb.Tags> UpdateTags(Models.mydb.Tags Tags)
        {

            var entryToUpdate = Context.Entry(Tags);
            entryToUpdate.CurrentValues.SetValues(Tags);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            return Tags;
        }

        public async Task<CHKS.Models.mydb.Tags> DeleteTags(Guid Id)
        {
            var itemToDelete = Context.Tags
                              .Where(i => i.Id == Id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            Context.Tags.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            return itemToDelete;
        }

        partial void OnCarGet(CHKS.Models.mydb.Car item);
        partial void OnGetCarByPlate(ref IQueryable<CHKS.Models.mydb.Car> items);


        public async Task<CHKS.Models.mydb.Car> GetCarByPlate(string plate)
        {
            var items = Context.Cars
                              .AsNoTracking()
                              .Where(i => i.Plate == plate);

 
            OnGetCarByPlate(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCarGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCarCreated(CHKS.Models.mydb.Car item);
        partial void OnAfterCarCreated(CHKS.Models.mydb.Car item);

        public async Task<CHKS.Models.mydb.Car> CreateCar(CHKS.Models.mydb.Car car)
        {
            OnCarCreated(car);

            var existingItem = Context.Cars
                              .Where(i => i.Plate == car.Plate)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Cars.Add(car);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(car).State = EntityState.Detached;
                throw;
            }

            OnAfterCarCreated(car);

            return car;
        }

        public async Task<CHKS.Models.mydb.Car> CancelCarChanges(CHKS.Models.mydb.Car item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCarUpdated(CHKS.Models.mydb.Car item);
        partial void OnAfterCarUpdated(CHKS.Models.mydb.Car item);

        public async Task<CHKS.Models.mydb.Car> UpdateCar(string plate, CHKS.Models.mydb.Car car)
        {
            OnCarUpdated(car);

            var itemToUpdate = Context.Cars
                              .Where(i => i.Plate == car.Plate)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(car);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCarUpdated(car);

            return car;
        }

        partial void OnCarDeleted(CHKS.Models.mydb.Car item);
        partial void OnAfterCarDeleted(CHKS.Models.mydb.Car item);

        public async Task<CHKS.Models.mydb.Car> DeleteCar(string plate)
        {
            var itemToDelete = Context.Cars
                              .Where(i => i.Plate == plate)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnCarDeleted(itemToDelete);


            Context.Cars.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCarDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportCarBrandsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/carbrands/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/carbrands/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCarBrandsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/carbrands/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/carbrands/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCarBrandsRead(ref IQueryable<CHKS.Models.mydb.CarBrand> items);

        public async Task<IQueryable<CHKS.Models.mydb.CarBrand>> GetCarBrands(Query query = null)
        {
            var items = Context.CarBrands.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCarBrandsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCarBrandGet(CHKS.Models.mydb.CarBrand item);
        partial void OnGetCarBrandByKey(ref IQueryable<CHKS.Models.mydb.CarBrand> items);


        public async Task<CHKS.Models.mydb.CarBrand> GetCarBrandByKey(string key)
        {
            var items = Context.CarBrands
                              .AsNoTracking()
                              .Where(i => i.Key == key);

 
            OnGetCarBrandByKey(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCarBrandGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCarBrandCreated(CHKS.Models.mydb.CarBrand item);
        partial void OnAfterCarBrandCreated(CHKS.Models.mydb.CarBrand item);

        public async Task<CHKS.Models.mydb.CarBrand> CreateCarBrand(CHKS.Models.mydb.CarBrand carbrand)
        {
            OnCarBrandCreated(carbrand);

            var existingItem = Context.CarBrands
                              .Where(i => i.Key == carbrand.Key)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.CarBrands.Add(carbrand);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(carbrand).State = EntityState.Detached;
                throw;
            }

            OnAfterCarBrandCreated(carbrand);

            return carbrand;
        }

        public async Task<CHKS.Models.mydb.CarBrand> CancelCarBrandChanges(CHKS.Models.mydb.CarBrand item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCarBrandUpdated(CHKS.Models.mydb.CarBrand item);
        partial void OnAfterCarBrandUpdated(CHKS.Models.mydb.CarBrand item);

        public async Task<CHKS.Models.mydb.CarBrand> UpdateCarBrand(string key, CHKS.Models.mydb.CarBrand carbrand)
        {
            OnCarBrandUpdated(carbrand);

            var itemToUpdate = Context.CarBrands
                              .Where(i => i.Key == carbrand.Key)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(carbrand);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCarBrandUpdated(carbrand);

            return carbrand;
        }

        partial void OnCarBrandDeleted(CHKS.Models.mydb.CarBrand item);
        partial void OnAfterCarBrandDeleted(CHKS.Models.mydb.CarBrand item);

        public async Task<CHKS.Models.mydb.CarBrand> DeleteCarBrand(string key)
        {
            var itemToDelete = Context.CarBrands
                              .Where(i => i.Key == key)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnCarBrandDeleted(itemToDelete);


            Context.CarBrands.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCarBrandDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportCartsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/carts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/carts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCartsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/carts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/carts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCartsRead(ref IQueryable<CHKS.Models.mydb.Cart> items);

        public async Task<IQueryable<CHKS.Models.mydb.Cart>> GetCarts(Query query = null)
        {
            var items = Context.Carts.AsQueryable();

            items = items.Include(i => i.Car);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCartsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCartGet(CHKS.Models.mydb.Cart item);
        partial void OnGetCartByCartId(ref IQueryable<CHKS.Models.mydb.Cart> items);


        public async Task<CHKS.Models.mydb.Cart> GetCartByCartId(int cartid)
        {
            var items = Context.Carts
                              .AsNoTracking()
                              .Where(i => i.CartId == cartid);

            items = items.Include(i => i.Car);
 
            OnGetCartByCartId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCartGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCartCreated(CHKS.Models.mydb.Cart item);
        partial void OnAfterCartCreated(CHKS.Models.mydb.Cart item);

        public async Task<CHKS.Models.mydb.Cart> CreateCart(CHKS.Models.mydb.Cart cart)
        {
            OnCartCreated(cart);

            var existingItem = Context.Carts
                              .Where(i => i.CartId == cart.CartId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Carts.Add(cart);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(cart).State = EntityState.Detached;
                throw;
            }

            OnAfterCartCreated(cart);

            return cart;
        }

        public async Task<CHKS.Models.mydb.Cart> CancelCartChanges(CHKS.Models.mydb.Cart item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCartUpdated(CHKS.Models.mydb.Cart item);
        partial void OnAfterCartUpdated(CHKS.Models.mydb.Cart item);

        public async Task<CHKS.Models.mydb.Cart> UpdateCart(int cartid, CHKS.Models.mydb.Cart cart)
        {
            OnCartUpdated(cart);

            var itemToUpdate = Context.Carts
                              .Where(i => i.CartId == cart.CartId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(cart);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCartUpdated(cart);

            return cart;
        }

        partial void OnCartDeleted(CHKS.Models.mydb.Cart item);
        partial void OnAfterCartDeleted(CHKS.Models.mydb.Cart item);

        public async Task<CHKS.Models.mydb.Cart> DeleteCart(int cartid)
        {
            var itemToDelete = Context.Carts
                              .Where(i => i.CartId == cartid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnCartDeleted(itemToDelete);


            Context.Carts.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCartDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportConnectorsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/connectors/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/connectors/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportConnectorsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/connectors/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/connectors/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnConnectorsRead(ref IQueryable<CHKS.Models.mydb.Connector> items);

        public async Task<IQueryable<CHKS.Models.mydb.Connector>> GetConnectors(Query query = null)
        {
            var items = Context.Connectors.AsQueryable();

            items = items.Include(i => i.Cart);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnConnectorsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnConnectorGet(CHKS.Models.mydb.Connector item);
        partial void OnGetConnectorByGeneratedKey(ref IQueryable<CHKS.Models.mydb.Connector> items);


        public async Task<CHKS.Models.mydb.Connector> GetConnectorById(Guid Id)
        {
            var items = Context.Connectors
                              .AsNoTracking()
                              .Where(i => i.Id == Id);

            items = items.Include(i => i.Cart);
 
            OnGetConnectorByGeneratedKey(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnConnectorGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnConnectorCreated(CHKS.Models.mydb.Connector item);
        partial void OnAfterConnectorCreated(CHKS.Models.mydb.Connector item);

        public async Task<CHKS.Models.mydb.Connector> CreateConnector(CHKS.Models.mydb.Connector connector)
        {
            OnConnectorCreated(connector);

            var existingItem = Context.Connectors
                              .Where(i => i.Id == connector.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Connectors.Add(connector);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(connector).State = EntityState.Detached;
                throw;
            }

            OnAfterConnectorCreated(connector);

            return connector;
        }

        public async Task<CHKS.Models.mydb.Connector> CancelConnectorChanges(CHKS.Models.mydb.Connector item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnConnectorUpdated(CHKS.Models.mydb.Connector item);
        partial void OnAfterConnectorUpdated(CHKS.Models.mydb.Connector item);

        public async Task<CHKS.Models.mydb.Connector> UpdateConnector(Guid Id, CHKS.Models.mydb.Connector connector)
        {
            OnConnectorUpdated(connector);

            var itemToUpdate = Context.Connectors
                              .Where(i => i.Id == connector.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(connector);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterConnectorUpdated(connector);

            return connector;
        }

        partial void OnConnectorDeleted(CHKS.Models.mydb.Connector item);
        partial void OnAfterConnectorDeleted(CHKS.Models.mydb.Connector item);

        public async Task<CHKS.Models.mydb.Connector> DeleteConnector(Guid Id)
        {
            var itemToDelete = Context.Connectors
                              .Where(i => i.Id == Id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnConnectorDeleted(itemToDelete);


            Context.Connectors.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterConnectorDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportDailyexpensesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/dailyexpenses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/dailyexpenses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportDailyexpensesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/dailyexpenses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/dailyexpenses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnDailyexpensesRead(ref IQueryable<CHKS.Models.mydb.Dailyexpense> items);

        public async Task<IQueryable<CHKS.Models.mydb.Dailyexpense>> GetDailyexpenses(Query query = null)
        {
            var items = Context.Dailyexpenses.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnDailyexpensesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnDailyexpenseGet(CHKS.Models.mydb.Dailyexpense item);
        partial void OnGetDailyexpenseByKey(ref IQueryable<CHKS.Models.mydb.Dailyexpense> items);


        public async Task<CHKS.Models.mydb.Dailyexpense> GetDailyexpenseByKey(Guid key)
        {
            var items = Context.Dailyexpenses
                              .AsNoTracking()
                              .Where(i => i.Key == key);

 
            OnGetDailyexpenseByKey(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnDailyexpenseGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnDailyexpenseCreated(CHKS.Models.mydb.Dailyexpense item);
        partial void OnAfterDailyexpenseCreated(CHKS.Models.mydb.Dailyexpense item);

        public async Task<CHKS.Models.mydb.Dailyexpense> CreateDailyexpense(CHKS.Models.mydb.Dailyexpense dailyexpense)
        {
            OnDailyexpenseCreated(dailyexpense);

            var existingItem = Context.Dailyexpenses
                              .Where(i => i.Key == dailyexpense.Key)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Dailyexpenses.Add(dailyexpense);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(dailyexpense).State = EntityState.Detached;
                throw;
            }

            OnAfterDailyexpenseCreated(dailyexpense);

            return dailyexpense;
        }

        public async Task<CHKS.Models.mydb.Dailyexpense> CancelDailyexpenseChanges(CHKS.Models.mydb.Dailyexpense item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnDailyexpenseUpdated(CHKS.Models.mydb.Dailyexpense item);
        partial void OnAfterDailyexpenseUpdated(CHKS.Models.mydb.Dailyexpense item);

        public async Task<CHKS.Models.mydb.Dailyexpense> UpdateDailyexpense(Guid key, CHKS.Models.mydb.Dailyexpense dailyexpense)
        {
            OnDailyexpenseUpdated(dailyexpense);

            var itemToUpdate = Context.Dailyexpenses
                              .Where(i => i.Key == dailyexpense.Key)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(dailyexpense);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterDailyexpenseUpdated(dailyexpense);

            return dailyexpense;
        }

        partial void OnDailyexpenseDeleted(CHKS.Models.mydb.Dailyexpense item);
        partial void OnAfterDailyexpenseDeleted(CHKS.Models.mydb.Dailyexpense item);

        public async Task<CHKS.Models.mydb.Dailyexpense> DeleteDailyexpense(Guid key)
        {
            var itemToDelete = Context.Dailyexpenses
                              .Where(i => i.Key == key)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnDailyexpenseDeleted(itemToDelete);


            Context.Dailyexpenses.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterDailyexpenseDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportHistoriesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/histories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/histories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHistoriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/histories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/histories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHistoriesRead(ref IQueryable<CHKS.Models.mydb.History> items);

        public async Task<IQueryable<CHKS.Models.mydb.History>> GetHistories(Query query = null)
        {
            var items = Context.Histories.AsQueryable();

            items = items.Include(i => i.Car);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnHistoriesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHistoryGet(CHKS.Models.mydb.History item);
        partial void OnGetHistoryById(ref IQueryable<CHKS.Models.mydb.History> items);


        public async Task<CHKS.Models.mydb.History> GetHistoryById(Guid id)
        {
            var items = Context.Histories
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Car);
 
            OnGetHistoryById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHistoryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHistoryCreated(CHKS.Models.mydb.History item);
        partial void OnAfterHistoryCreated(CHKS.Models.mydb.History item);

        public async Task<CHKS.Models.mydb.History> CreateHistory(CHKS.Models.mydb.History history)
        {
            OnHistoryCreated(history);

            var existingItem = Context.Histories
                              .Where(i => i.Id == history.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Histories.Add(history);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(history).State = EntityState.Detached;
                throw;
            }

            OnAfterHistoryCreated(history);

            return history;
        }

        public async Task<CHKS.Models.mydb.History> CancelHistoryChanges(CHKS.Models.mydb.History item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHistoryUpdated(CHKS.Models.mydb.History item);
        partial void OnAfterHistoryUpdated(CHKS.Models.mydb.History item);

        public async Task<CHKS.Models.mydb.History> UpdateHistory(Guid id, CHKS.Models.mydb.History history)
        {
            OnHistoryUpdated(history);

            var itemToUpdate = Context.Histories
                              .Where(i => i.Id == history.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(history);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHistoryUpdated(history);

            return history;
        }

        partial void OnHistoryDeleted(CHKS.Models.mydb.History item);
        partial void OnAfterHistoryDeleted(CHKS.Models.mydb.History item);

        public async Task<CHKS.Models.mydb.History> DeleteHistory(Guid id)
        {
            var itemToDelete = Context.Histories
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnHistoryDeleted(itemToDelete);


            Context.Histories.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHistoryDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportHistoryconnectorsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/historyconnectors/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/historyconnectors/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHistoryconnectorsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/historyconnectors/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/historyconnectors/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHistoryconnectorsRead(ref IQueryable<CHKS.Models.mydb.Historyconnector> items);

        public async Task<IQueryable<CHKS.Models.mydb.Historyconnector>> GetHistoryconnectors(Query query = null)
        {
            var items = Context.Historyconnectors.AsQueryable();

            items = items.Include(i => i.History);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnHistoryconnectorsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnHistoryconnectorGet(CHKS.Models.mydb.Historyconnector item);
        partial void OnGetHistoryconnectorById(ref IQueryable<CHKS.Models.mydb.Historyconnector> items);


        public async Task<CHKS.Models.mydb.Historyconnector> GetHistoryconnectorById(Guid id)
        {
            var items = Context.Historyconnectors
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.History);
 
            OnGetHistoryconnectorById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHistoryconnectorGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHistoryconnectorCreated(CHKS.Models.mydb.Historyconnector item);
        partial void OnAfterHistoryconnectorCreated(CHKS.Models.mydb.Historyconnector item);

        public async Task<CHKS.Models.mydb.Historyconnector> CreateHistoryconnector(CHKS.Models.mydb.Historyconnector historyconnector)
        {
            OnHistoryconnectorCreated(historyconnector);

            var existingItem = Context.Historyconnectors
                              .Where(i => i.Id == historyconnector.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Historyconnectors.Add(historyconnector);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(historyconnector).State = EntityState.Detached;
                throw;
            }

            OnAfterHistoryconnectorCreated(historyconnector);

            return historyconnector;
        }

        public async Task<CHKS.Models.mydb.Historyconnector> CancelHistoryconnectorChanges(CHKS.Models.mydb.Historyconnector item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHistoryconnectorUpdated(CHKS.Models.mydb.Historyconnector item);
        partial void OnAfterHistoryconnectorUpdated(CHKS.Models.mydb.Historyconnector item);

        public async Task<CHKS.Models.mydb.Historyconnector> UpdateHistoryconnector(Guid id, CHKS.Models.mydb.Historyconnector historyconnector)
        {
            OnHistoryconnectorUpdated(historyconnector);

            var itemToUpdate = Context.Historyconnectors
                              .Where(i => i.Id == historyconnector.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(historyconnector);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterHistoryconnectorUpdated(historyconnector);

            return historyconnector;
        }

        partial void OnHistoryconnectorDeleted(CHKS.Models.mydb.Historyconnector item);
        partial void OnAfterHistoryconnectorDeleted(CHKS.Models.mydb.Historyconnector item);

        public async Task<CHKS.Models.mydb.Historyconnector> DeleteHistoryconnector(Guid id)
        {
            var itemToDelete = Context.Historyconnectors
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnHistoryconnectorDeleted(itemToDelete);


            Context.Historyconnectors.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterHistoryconnectorDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportInventoriesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/inventories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/inventories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportInventoriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/inventories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/inventories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnInventoriesRead(ref IQueryable<CHKS.Models.mydb.Inventory> items);

        public async Task<IQueryable<CHKS.Models.mydb.Inventory>> GetInventories(Query query = null)
        {
            var items = Context.Inventories.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnInventoriesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnInventoryGet(CHKS.Models.mydb.Inventory item);
        partial void OnGetInventoryByCode(ref IQueryable<CHKS.Models.mydb.Inventory> items);


        public async Task<CHKS.Models.mydb.Inventory> GetInventoryById(Guid Id)
        {
            var items = Context.Inventories
                              .AsNoTracking()
                              .Where(i => i.Id == Id);
 
            OnGetInventoryByCode(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnInventoryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        public async Task<IEnumerable<Models.mydb.Tags>> InventoryAddTag(Guid Id, Models.mydb.Tags Tags)
        {
            var item = Context.Inventories
                              .Where(i => i.Id == Id).FirstOrDefault();
            item.Tags ??= [];   
            item.Tags.Add(Tags);

            Context.SaveChanges();

            return await Task.FromResult(item.Tags);
        }

        public async Task<IEnumerable<Models.mydb.Tags>> InventoryRemoveTag(Guid Id, Models.mydb.Tags Tags)
        {
            var item = Context.Inventories
                              .Where(i => i.Id == Id).FirstOrDefault();
            item?.Tags?.Remove(Tags);

            Context.SaveChanges();

            return await Task.FromResult(item.Tags);
        }

        partial void OnInventoryCreated(CHKS.Models.mydb.Inventory item);
        partial void OnAfterInventoryCreated(CHKS.Models.mydb.Inventory item);

        public async Task<CHKS.Models.mydb.Inventory> CreateInventory(CHKS.Models.mydb.Inventory inventory)
        {
            OnInventoryCreated(inventory);

            var existingItem = Context.Inventories
                              .Where(i => i.Id == inventory.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Inventories.Add(inventory);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(inventory).State = EntityState.Detached;
                throw;
            }

            OnAfterInventoryCreated(inventory);

            return inventory;
        }

        public async Task<CHKS.Models.mydb.Inventory> CancelInventoryChanges(CHKS.Models.mydb.Inventory item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnInventoryUpdated(CHKS.Models.mydb.Inventory item);
        partial void OnAfterInventoryUpdated(CHKS.Models.mydb.Inventory item);

        public async Task<CHKS.Models.mydb.Inventory> UpdateInventory(string code, CHKS.Models.mydb.Inventory inventory)
        {
            OnInventoryUpdated(inventory);

            var itemToUpdate = Context.Inventories
                              .Where(i => i.Id == inventory.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(inventory);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterInventoryUpdated(inventory);

            return inventory;
        }
        public async Task<CHKS.Models.mydb.Inventory> UpdateInventory(CHKS.Models.mydb.Inventory inventory)
        {
            OnInventoryUpdated(inventory);

            var itemToUpdate = Context.Inventories
                              .Where(i => i.Id == inventory.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(inventory);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterInventoryUpdated(inventory);

            return inventory;
        }

        partial void OnInventoryDeleted(CHKS.Models.mydb.Inventory item);
        partial void OnAfterInventoryDeleted(CHKS.Models.mydb.Inventory item);

        public async Task<CHKS.Models.mydb.Inventory> DeleteInventory(Guid Id)
        {
            var itemToDelete = Context.Inventories
                              .Where(i => i.Id == Id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnInventoryDeleted(itemToDelete);


            Context.Inventories.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterInventoryDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}