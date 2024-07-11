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
    public partial class fordevlocalService
    {
        fordevlocalContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly fordevlocalContext context;
        private readonly NavigationManager navigationManager;

        public fordevlocalService(fordevlocalContext context, NavigationManager navigationManager)
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
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/fordevlocal/cars/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/fordevlocal/cars/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCarsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/fordevlocal/cars/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/fordevlocal/cars/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCarsRead(ref IQueryable<CHKS.Models.fordevlocal.Car> items);

        public async Task<IQueryable<CHKS.Models.fordevlocal.Car>> GetCars(Query query = null)
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

        partial void OnCarGet(CHKS.Models.fordevlocal.Car item);
        partial void OnGetCarByPlate(ref IQueryable<CHKS.Models.fordevlocal.Car> items);


        public async Task<CHKS.Models.fordevlocal.Car> GetCarByPlate(string plate)
        {
            var items = Context.Cars
                              .AsNoTracking()
                              .Where(i => i.Plate == plate);

 
            OnGetCarByPlate(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCarGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCarCreated(CHKS.Models.fordevlocal.Car item);
        partial void OnAfterCarCreated(CHKS.Models.fordevlocal.Car item);

        public async Task<CHKS.Models.fordevlocal.Car> CreateCar(CHKS.Models.fordevlocal.Car car)
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

        public async Task<CHKS.Models.fordevlocal.Car> CancelCarChanges(CHKS.Models.fordevlocal.Car item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCarUpdated(CHKS.Models.fordevlocal.Car item);
        partial void OnAfterCarUpdated(CHKS.Models.fordevlocal.Car item);

        public async Task<CHKS.Models.fordevlocal.Car> UpdateCar(string plate, CHKS.Models.fordevlocal.Car car)
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

        partial void OnCarDeleted(CHKS.Models.fordevlocal.Car item);
        partial void OnAfterCarDeleted(CHKS.Models.fordevlocal.Car item);

        public async Task<CHKS.Models.fordevlocal.Car> DeleteCar(string plate)
        {
            var itemToDelete = Context.Cars
                              .Where(i => i.Plate == plate)
                              .Include(i => i.Carts)
                              .Include(i => i.Histories)
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
    
        public async Task ExportCartsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/fordevlocal/carts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/fordevlocal/carts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCartsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/fordevlocal/carts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/fordevlocal/carts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCartsRead(ref IQueryable<CHKS.Models.fordevlocal.Cart> items);

        public async Task<IQueryable<CHKS.Models.fordevlocal.Cart>> GetCarts(Query query = null)
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

        partial void OnCartGet(CHKS.Models.fordevlocal.Cart item);
        partial void OnGetCartByCartId(ref IQueryable<CHKS.Models.fordevlocal.Cart> items);


        public async Task<CHKS.Models.fordevlocal.Cart> GetCartByCartId(int cartid)
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

        partial void OnCartCreated(CHKS.Models.fordevlocal.Cart item);
        partial void OnAfterCartCreated(CHKS.Models.fordevlocal.Cart item);

        public async Task<CHKS.Models.fordevlocal.Cart> CreateCart(CHKS.Models.fordevlocal.Cart cart)
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

        public async Task<CHKS.Models.fordevlocal.Cart> CancelCartChanges(CHKS.Models.fordevlocal.Cart item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCartUpdated(CHKS.Models.fordevlocal.Cart item);
        partial void OnAfterCartUpdated(CHKS.Models.fordevlocal.Cart item);

        public async Task<CHKS.Models.fordevlocal.Cart> UpdateCart(int cartid, CHKS.Models.fordevlocal.Cart cart)
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

        partial void OnCartDeleted(CHKS.Models.fordevlocal.Cart item);
        partial void OnAfterCartDeleted(CHKS.Models.fordevlocal.Cart item);

        public async Task<CHKS.Models.fordevlocal.Cart> DeleteCart(int cartid)
        {
            var itemToDelete = Context.Carts
                              .Where(i => i.CartId == cartid)
                              .Include(i => i.Connectors)
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
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/fordevlocal/connectors/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/fordevlocal/connectors/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportConnectorsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/fordevlocal/connectors/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/fordevlocal/connectors/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnConnectorsRead(ref IQueryable<CHKS.Models.fordevlocal.Connector> items);

        public async Task<IQueryable<CHKS.Models.fordevlocal.Connector>> GetConnectors(Query query = null)
        {
            var items = Context.Connectors.AsQueryable();

            items = items.Include(i => i.Cart);
            items = items.Include(i => i.Inventory);

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

        partial void OnConnectorGet(CHKS.Models.fordevlocal.Connector item);
        partial void OnGetConnectorByGeneratedKey(ref IQueryable<CHKS.Models.fordevlocal.Connector> items);


        public async Task<CHKS.Models.fordevlocal.Connector> GetConnectorByGeneratedKey(string generatedkey)
        {
            var items = Context.Connectors
                              .AsNoTracking()
                              .Where(i => i.GeneratedKey == generatedkey);

            items = items.Include(i => i.Cart);
            items = items.Include(i => i.Inventory);
 
            OnGetConnectorByGeneratedKey(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnConnectorGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnConnectorCreated(CHKS.Models.fordevlocal.Connector item);
        partial void OnAfterConnectorCreated(CHKS.Models.fordevlocal.Connector item);

        public async Task<CHKS.Models.fordevlocal.Connector> CreateConnector(CHKS.Models.fordevlocal.Connector connector)
        {
            OnConnectorCreated(connector);

            var existingItem = Context.Connectors
                              .Where(i => i.GeneratedKey == connector.GeneratedKey)
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

        public async Task<CHKS.Models.fordevlocal.Connector> CancelConnectorChanges(CHKS.Models.fordevlocal.Connector item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnConnectorUpdated(CHKS.Models.fordevlocal.Connector item);
        partial void OnAfterConnectorUpdated(CHKS.Models.fordevlocal.Connector item);

        public async Task<CHKS.Models.fordevlocal.Connector> UpdateConnector(string generatedkey, CHKS.Models.fordevlocal.Connector connector)
        {
            OnConnectorUpdated(connector);

            var itemToUpdate = Context.Connectors
                              .Where(i => i.GeneratedKey == connector.GeneratedKey)
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

        partial void OnConnectorDeleted(CHKS.Models.fordevlocal.Connector item);
        partial void OnAfterConnectorDeleted(CHKS.Models.fordevlocal.Connector item);

        public async Task<CHKS.Models.fordevlocal.Connector> DeleteConnector(string generatedkey)
        {
            var itemToDelete = Context.Connectors
                              .Where(i => i.GeneratedKey == generatedkey)
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
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/fordevlocal/dailyexpenses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/fordevlocal/dailyexpenses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportDailyexpensesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/fordevlocal/dailyexpenses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/fordevlocal/dailyexpenses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnDailyexpensesRead(ref IQueryable<CHKS.Models.fordevlocal.Dailyexpense> items);

        public async Task<IQueryable<CHKS.Models.fordevlocal.Dailyexpense>> GetDailyexpenses(Query query = null)
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

        partial void OnDailyexpenseGet(CHKS.Models.fordevlocal.Dailyexpense item);
        partial void OnGetDailyexpenseByKey(ref IQueryable<CHKS.Models.fordevlocal.Dailyexpense> items);


        public async Task<CHKS.Models.fordevlocal.Dailyexpense> GetDailyexpenseByKey(string key)
        {
            var items = Context.Dailyexpenses
                              .AsNoTracking()
                              .Where(i => i.Key == key);

 
            OnGetDailyexpenseByKey(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnDailyexpenseGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnDailyexpenseCreated(CHKS.Models.fordevlocal.Dailyexpense item);
        partial void OnAfterDailyexpenseCreated(CHKS.Models.fordevlocal.Dailyexpense item);

        public async Task<CHKS.Models.fordevlocal.Dailyexpense> CreateDailyexpense(CHKS.Models.fordevlocal.Dailyexpense dailyexpense)
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

        public async Task<CHKS.Models.fordevlocal.Dailyexpense> CancelDailyexpenseChanges(CHKS.Models.fordevlocal.Dailyexpense item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnDailyexpenseUpdated(CHKS.Models.fordevlocal.Dailyexpense item);
        partial void OnAfterDailyexpenseUpdated(CHKS.Models.fordevlocal.Dailyexpense item);

        public async Task<CHKS.Models.fordevlocal.Dailyexpense> UpdateDailyexpense(string key, CHKS.Models.fordevlocal.Dailyexpense dailyexpense)
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

        partial void OnDailyexpenseDeleted(CHKS.Models.fordevlocal.Dailyexpense item);
        partial void OnAfterDailyexpenseDeleted(CHKS.Models.fordevlocal.Dailyexpense item);

        public async Task<CHKS.Models.fordevlocal.Dailyexpense> DeleteDailyexpense(string key)
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
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/fordevlocal/histories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/fordevlocal/histories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHistoriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/fordevlocal/histories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/fordevlocal/histories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHistoriesRead(ref IQueryable<CHKS.Models.fordevlocal.History> items);

        public async Task<IQueryable<CHKS.Models.fordevlocal.History>> GetHistories(Query query = null)
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

        partial void OnHistoryGet(CHKS.Models.fordevlocal.History item);
        partial void OnGetHistoryByCashoutDate(ref IQueryable<CHKS.Models.fordevlocal.History> items);


        public async Task<CHKS.Models.fordevlocal.History> GetHistoryByCashoutDate(string cashoutdate)
        {
            var items = Context.Histories
                              .AsNoTracking()
                              .Where(i => i.CashoutDate == cashoutdate);

            items = items.Include(i => i.Car);
 
            OnGetHistoryByCashoutDate(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHistoryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHistoryCreated(CHKS.Models.fordevlocal.History item);
        partial void OnAfterHistoryCreated(CHKS.Models.fordevlocal.History item);

        public async Task<CHKS.Models.fordevlocal.History> CreateHistory(CHKS.Models.fordevlocal.History history)
        {
            OnHistoryCreated(history);

            var existingItem = Context.Histories
                              .Where(i => i.CashoutDate == history.CashoutDate)
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

        public async Task<CHKS.Models.fordevlocal.History> CancelHistoryChanges(CHKS.Models.fordevlocal.History item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHistoryUpdated(CHKS.Models.fordevlocal.History item);
        partial void OnAfterHistoryUpdated(CHKS.Models.fordevlocal.History item);

        public async Task<CHKS.Models.fordevlocal.History> UpdateHistory(string cashoutdate, CHKS.Models.fordevlocal.History history)
        {
            OnHistoryUpdated(history);

            var itemToUpdate = Context.Histories
                              .Where(i => i.CashoutDate == history.CashoutDate)
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

        partial void OnHistoryDeleted(CHKS.Models.fordevlocal.History item);
        partial void OnAfterHistoryDeleted(CHKS.Models.fordevlocal.History item);

        public async Task<CHKS.Models.fordevlocal.History> DeleteHistory(string cashoutdate)
        {
            var itemToDelete = Context.Histories
                              .Where(i => i.CashoutDate == cashoutdate)
                              .Include(i => i.Historyconnectors)
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
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/fordevlocal/historyconnectors/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/fordevlocal/historyconnectors/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportHistoryconnectorsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/fordevlocal/historyconnectors/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/fordevlocal/historyconnectors/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnHistoryconnectorsRead(ref IQueryable<CHKS.Models.fordevlocal.Historyconnector> items);

        public async Task<IQueryable<CHKS.Models.fordevlocal.Historyconnector>> GetHistoryconnectors(Query query = null)
        {
            var items = Context.Historyconnectors.AsQueryable();

            items = items.Include(i => i.History);
            items = items.Include(i => i.Inventory);

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

        partial void OnHistoryconnectorGet(CHKS.Models.fordevlocal.Historyconnector item);
        partial void OnGetHistoryconnectorById(ref IQueryable<CHKS.Models.fordevlocal.Historyconnector> items);


        public async Task<CHKS.Models.fordevlocal.Historyconnector> GetHistoryconnectorById(string id)
        {
            var items = Context.Historyconnectors
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.History);
            items = items.Include(i => i.Inventory);
 
            OnGetHistoryconnectorById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnHistoryconnectorGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnHistoryconnectorCreated(CHKS.Models.fordevlocal.Historyconnector item);
        partial void OnAfterHistoryconnectorCreated(CHKS.Models.fordevlocal.Historyconnector item);

        public async Task<CHKS.Models.fordevlocal.Historyconnector> CreateHistoryconnector(CHKS.Models.fordevlocal.Historyconnector historyconnector)
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

        public async Task<CHKS.Models.fordevlocal.Historyconnector> CancelHistoryconnectorChanges(CHKS.Models.fordevlocal.Historyconnector item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnHistoryconnectorUpdated(CHKS.Models.fordevlocal.Historyconnector item);
        partial void OnAfterHistoryconnectorUpdated(CHKS.Models.fordevlocal.Historyconnector item);

        public async Task<CHKS.Models.fordevlocal.Historyconnector> UpdateHistoryconnector(string id, CHKS.Models.fordevlocal.Historyconnector historyconnector)
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

        partial void OnHistoryconnectorDeleted(CHKS.Models.fordevlocal.Historyconnector item);
        partial void OnAfterHistoryconnectorDeleted(CHKS.Models.fordevlocal.Historyconnector item);

        public async Task<CHKS.Models.fordevlocal.Historyconnector> DeleteHistoryconnector(string id)
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
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/fordevlocal/inventories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/fordevlocal/inventories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportInventoriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/fordevlocal/inventories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/fordevlocal/inventories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnInventoriesRead(ref IQueryable<CHKS.Models.fordevlocal.Inventory> items);

        public async Task<IQueryable<CHKS.Models.fordevlocal.Inventory>> GetInventories(Query query = null)
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

        partial void OnInventoryGet(CHKS.Models.fordevlocal.Inventory item);
        partial void OnGetInventoryByName(ref IQueryable<CHKS.Models.fordevlocal.Inventory> items);


        public async Task<CHKS.Models.fordevlocal.Inventory> GetInventoryByName(string name)
        {
            var items = Context.Inventories
                              .AsNoTracking()
                              .Where(i => i.Name == name);

 
            OnGetInventoryByName(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnInventoryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnInventoryCreated(CHKS.Models.fordevlocal.Inventory item);
        partial void OnAfterInventoryCreated(CHKS.Models.fordevlocal.Inventory item);

        public async Task<CHKS.Models.fordevlocal.Inventory> CreateInventory(CHKS.Models.fordevlocal.Inventory inventory)
        {
            OnInventoryCreated(inventory);

            var existingItem = Context.Inventories
                              .Where(i => i.Name == inventory.Name)
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

        public async Task<CHKS.Models.fordevlocal.Inventory> CancelInventoryChanges(CHKS.Models.fordevlocal.Inventory item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnInventoryUpdated(CHKS.Models.fordevlocal.Inventory item);
        partial void OnAfterInventoryUpdated(CHKS.Models.fordevlocal.Inventory item);

        public async Task<CHKS.Models.fordevlocal.Inventory> UpdateInventory(string name, CHKS.Models.fordevlocal.Inventory inventory)
        {
            OnInventoryUpdated(inventory);

            var itemToUpdate = Context.Inventories
                              .Where(i => i.Name == inventory.Name)
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

        partial void OnInventoryDeleted(CHKS.Models.fordevlocal.Inventory item);
        partial void OnAfterInventoryDeleted(CHKS.Models.fordevlocal.Inventory item);

        public async Task<CHKS.Models.fordevlocal.Inventory> DeleteInventory(string name)
        {
            var itemToDelete = Context.Inventories
                              .Where(i => i.Name == name)
                              .Include(i => i.Connectors)
                              .Include(i => i.Historyconnectors)
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