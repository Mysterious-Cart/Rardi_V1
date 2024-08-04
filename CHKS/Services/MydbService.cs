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
        partial void OnGetCarBrandByBrand(ref IQueryable<CHKS.Models.mydb.CarBrand> items);


        public async Task<CHKS.Models.mydb.CarBrand> GetCarBrandByBrand(string brand)
        {
            var items = Context.CarBrands
                              .AsNoTracking()
                              .Where(i => i.Brand == brand);

 
            OnGetCarBrandByBrand(ref items);

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
                              .Where(i => i.Brand == carbrand.Brand)
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

        public async Task<CHKS.Models.mydb.CarBrand> UpdateCarBrand(string brand, CHKS.Models.mydb.CarBrand carbrand)
        {
            OnCarBrandUpdated(carbrand);

            var itemToUpdate = Context.CarBrands
                              .Where(i => i.Brand == carbrand.Brand)
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

        public async Task<CHKS.Models.mydb.CarBrand> DeleteCarBrand(string brand)
        {
            var itemToDelete = Context.CarBrands
                              .Where(i => i.Brand == brand)
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
    
        public async Task ExportCashbacksToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/cashbacks/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/cashbacks/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCashbacksToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/cashbacks/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/cashbacks/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCashbacksRead(ref IQueryable<CHKS.Models.mydb.Cashback> items);

        public async Task<IQueryable<CHKS.Models.mydb.Cashback>> GetCashbacks(Query query = null)
        {
            var items = Context.Cashbacks.AsQueryable();


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

            OnCashbacksRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCashbackGet(CHKS.Models.mydb.Cashback item);
        partial void OnGetCashbackByKey(ref IQueryable<CHKS.Models.mydb.Cashback> items);


        public async Task<CHKS.Models.mydb.Cashback> GetCashbackByKey(string key)
        {
            var items = Context.Cashbacks
                              .AsNoTracking()
                              .Where(i => i.Key == key);

 
            OnGetCashbackByKey(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCashbackGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCashbackCreated(CHKS.Models.mydb.Cashback item);
        partial void OnAfterCashbackCreated(CHKS.Models.mydb.Cashback item);

        public async Task<CHKS.Models.mydb.Cashback> CreateCashback(CHKS.Models.mydb.Cashback cashback)
        {
            OnCashbackCreated(cashback);

            var existingItem = Context.Cashbacks
                              .Where(i => i.Key == cashback.Key)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Cashbacks.Add(cashback);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(cashback).State = EntityState.Detached;
                throw;
            }

            OnAfterCashbackCreated(cashback);

            return cashback;
        }

        public async Task<CHKS.Models.mydb.Cashback> CancelCashbackChanges(CHKS.Models.mydb.Cashback item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCashbackUpdated(CHKS.Models.mydb.Cashback item);
        partial void OnAfterCashbackUpdated(CHKS.Models.mydb.Cashback item);

        public async Task<CHKS.Models.mydb.Cashback> UpdateCashback(string key, CHKS.Models.mydb.Cashback cashback)
        {
            OnCashbackUpdated(cashback);

            var itemToUpdate = Context.Cashbacks
                              .Where(i => i.Key == cashback.Key)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(cashback);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCashbackUpdated(cashback);

            return cashback;
        }

        partial void OnCashbackDeleted(CHKS.Models.mydb.Cashback item);
        partial void OnAfterCashbackDeleted(CHKS.Models.mydb.Cashback item);

        public async Task<CHKS.Models.mydb.Cashback> DeleteCashback(string key)
        {
            var itemToDelete = Context.Cashbacks
                              .Where(i => i.Key == key)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnCashbackDeleted(itemToDelete);


            Context.Cashbacks.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCashbackDeleted(itemToDelete);

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

        partial void OnConnectorGet(CHKS.Models.mydb.Connector item);
        partial void OnGetConnectorByGeneratedKey(ref IQueryable<CHKS.Models.mydb.Connector> items);


        public async Task<CHKS.Models.mydb.Connector> GetConnectorByGeneratedKey(string generatedkey)
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

        partial void OnConnectorCreated(CHKS.Models.mydb.Connector item);
        partial void OnAfterConnectorCreated(CHKS.Models.mydb.Connector item);

        public async Task<CHKS.Models.mydb.Connector> CreateConnector(CHKS.Models.mydb.Connector connector)
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

        public async Task<CHKS.Models.mydb.Connector> UpdateConnector(string generatedkey, CHKS.Models.mydb.Connector connector)
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

        partial void OnConnectorDeleted(CHKS.Models.mydb.Connector item);
        partial void OnAfterConnectorDeleted(CHKS.Models.mydb.Connector item);

        public async Task<CHKS.Models.mydb.Connector> DeleteConnector(string generatedkey)
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


        public async Task<CHKS.Models.mydb.Dailyexpense> GetDailyexpenseByKey(string key)
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

        public async Task<CHKS.Models.mydb.Dailyexpense> UpdateDailyexpense(string key, CHKS.Models.mydb.Dailyexpense dailyexpense)
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

        public async Task<CHKS.Models.mydb.Dailyexpense> DeleteDailyexpense(string key)
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
        partial void OnGetHistoryByCashoutDate(ref IQueryable<CHKS.Models.mydb.History> items);


        public async Task<CHKS.Models.mydb.History> GetHistoryByCashoutDate(string cashoutdate)
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

        partial void OnHistoryCreated(CHKS.Models.mydb.History item);
        partial void OnAfterHistoryCreated(CHKS.Models.mydb.History item);

        public async Task<CHKS.Models.mydb.History> CreateHistory(CHKS.Models.mydb.History history)
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

        public async Task<CHKS.Models.mydb.History> UpdateHistory(string cashoutdate, CHKS.Models.mydb.History history)
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

        partial void OnHistoryDeleted(CHKS.Models.mydb.History item);
        partial void OnAfterHistoryDeleted(CHKS.Models.mydb.History item);

        public async Task<CHKS.Models.mydb.History> DeleteHistory(string cashoutdate)
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

        partial void OnHistoryconnectorGet(CHKS.Models.mydb.Historyconnector item);
        partial void OnGetHistoryconnectorById(ref IQueryable<CHKS.Models.mydb.Historyconnector> items);


        public async Task<CHKS.Models.mydb.Historyconnector> GetHistoryconnectorById(string id)
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

        public async Task<CHKS.Models.mydb.Historyconnector> UpdateHistoryconnector(string id, CHKS.Models.mydb.Historyconnector historyconnector)
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

        public async Task<CHKS.Models.mydb.Historyconnector> DeleteHistoryconnector(string id)
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
        partial void OnGetInventoryByName(ref IQueryable<CHKS.Models.mydb.Inventory> items);


        public async Task<CHKS.Models.mydb.Inventory> GetInventoryByName(string name)
        {
            var items = Context.Inventories
                              .AsNoTracking()
                              .Where(i => i.Name == name);

 
            OnGetInventoryByName(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnInventoryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnInventoryCreated(CHKS.Models.mydb.Inventory item);
        partial void OnAfterInventoryCreated(CHKS.Models.mydb.Inventory item);

        public async Task<CHKS.Models.mydb.Inventory> CreateInventory(CHKS.Models.mydb.Inventory inventory)
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

        public async Task<CHKS.Models.mydb.Inventory> UpdateInventory(string name, CHKS.Models.mydb.Inventory inventory)
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

        partial void OnInventoryDeleted(CHKS.Models.mydb.Inventory item);
        partial void OnAfterInventoryDeleted(CHKS.Models.mydb.Inventory item);

        public async Task<CHKS.Models.mydb.Inventory> DeleteInventory(string name)
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
    
        public async Task ExportInventoryCaroptionsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/inventorycaroptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/inventorycaroptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportInventoryCaroptionsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/inventorycaroptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/inventorycaroptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnInventoryCaroptionsRead(ref IQueryable<CHKS.Models.mydb.InventoryCaroption> items);

        public async Task<IQueryable<CHKS.Models.mydb.InventoryCaroption>> GetInventoryCaroptions(Query query = null)
        {
            var items = Context.InventoryCaroptions.AsQueryable();


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

            OnInventoryCaroptionsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnInventoryCaroptionGet(CHKS.Models.mydb.InventoryCaroption item);
        partial void OnGetInventoryCaroptionByKey(ref IQueryable<CHKS.Models.mydb.InventoryCaroption> items);


        public async Task<CHKS.Models.mydb.InventoryCaroption> GetInventoryCaroptionByKey(string key)
        {
            var items = Context.InventoryCaroptions
                              .AsNoTracking()
                              .Where(i => i.Key == key);

 
            OnGetInventoryCaroptionByKey(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnInventoryCaroptionGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnInventoryCaroptionCreated(CHKS.Models.mydb.InventoryCaroption item);
        partial void OnAfterInventoryCaroptionCreated(CHKS.Models.mydb.InventoryCaroption item);

        public async Task<CHKS.Models.mydb.InventoryCaroption> CreateInventoryCaroption(CHKS.Models.mydb.InventoryCaroption inventorycaroption)
        {
            OnInventoryCaroptionCreated(inventorycaroption);

            var existingItem = Context.InventoryCaroptions
                              .Where(i => i.Key == inventorycaroption.Key)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.InventoryCaroptions.Add(inventorycaroption);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(inventorycaroption).State = EntityState.Detached;
                throw;
            }

            OnAfterInventoryCaroptionCreated(inventorycaroption);

            return inventorycaroption;
        }

        public async Task<CHKS.Models.mydb.InventoryCaroption> CancelInventoryCaroptionChanges(CHKS.Models.mydb.InventoryCaroption item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnInventoryCaroptionUpdated(CHKS.Models.mydb.InventoryCaroption item);
        partial void OnAfterInventoryCaroptionUpdated(CHKS.Models.mydb.InventoryCaroption item);

        public async Task<CHKS.Models.mydb.InventoryCaroption> UpdateInventoryCaroption(string key, CHKS.Models.mydb.InventoryCaroption inventorycaroption)
        {
            OnInventoryCaroptionUpdated(inventorycaroption);

            var itemToUpdate = Context.InventoryCaroptions
                              .Where(i => i.Key == inventorycaroption.Key)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(inventorycaroption);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterInventoryCaroptionUpdated(inventorycaroption);

            return inventorycaroption;
        }

        partial void OnInventoryCaroptionDeleted(CHKS.Models.mydb.InventoryCaroption item);
        partial void OnAfterInventoryCaroptionDeleted(CHKS.Models.mydb.InventoryCaroption item);

        public async Task<CHKS.Models.mydb.InventoryCaroption> DeleteInventoryCaroption(string key)
        {
            var itemToDelete = Context.InventoryCaroptions
                              .Where(i => i.Key == key)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnInventoryCaroptionDeleted(itemToDelete);


            Context.InventoryCaroptions.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterInventoryCaroptionDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportInventoryOptionsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/inventoryoptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/inventoryoptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportInventoryOptionsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/inventoryoptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/inventoryoptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnInventoryOptionsRead(ref IQueryable<CHKS.Models.mydb.InventoryOption> items);

        public async Task<IQueryable<CHKS.Models.mydb.InventoryOption>> GetInventoryOptions(Query query = null)
        {
            var items = Context.InventoryOptions.AsQueryable();


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

            OnInventoryOptionsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnInventoryOptionGet(CHKS.Models.mydb.InventoryOption item);
        partial void OnGetInventoryOptionByKey(ref IQueryable<CHKS.Models.mydb.InventoryOption> items);


        public async Task<CHKS.Models.mydb.InventoryOption> GetInventoryOptionByKey(string key)
        {
            var items = Context.InventoryOptions
                              .AsNoTracking()
                              .Where(i => i.Key == key);

 
            OnGetInventoryOptionByKey(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnInventoryOptionGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnInventoryOptionCreated(CHKS.Models.mydb.InventoryOption item);
        partial void OnAfterInventoryOptionCreated(CHKS.Models.mydb.InventoryOption item);

        public async Task<CHKS.Models.mydb.InventoryOption> CreateInventoryOption(CHKS.Models.mydb.InventoryOption inventoryoption)
        {
            OnInventoryOptionCreated(inventoryoption);

            var existingItem = Context.InventoryOptions
                              .Where(i => i.Key == inventoryoption.Key)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.InventoryOptions.Add(inventoryoption);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(inventoryoption).State = EntityState.Detached;
                throw;
            }

            OnAfterInventoryOptionCreated(inventoryoption);

            return inventoryoption;
        }

        public async Task<CHKS.Models.mydb.InventoryOption> CancelInventoryOptionChanges(CHKS.Models.mydb.InventoryOption item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnInventoryOptionUpdated(CHKS.Models.mydb.InventoryOption item);
        partial void OnAfterInventoryOptionUpdated(CHKS.Models.mydb.InventoryOption item);

        public async Task<CHKS.Models.mydb.InventoryOption> UpdateInventoryOption(string key, CHKS.Models.mydb.InventoryOption inventoryoption)
        {
            OnInventoryOptionUpdated(inventoryoption);

            var itemToUpdate = Context.InventoryOptions
                              .Where(i => i.Key == inventoryoption.Key)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(inventoryoption);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterInventoryOptionUpdated(inventoryoption);

            return inventoryoption;
        }

        partial void OnInventoryOptionDeleted(CHKS.Models.mydb.InventoryOption item);
        partial void OnAfterInventoryOptionDeleted(CHKS.Models.mydb.InventoryOption item);

        public async Task<CHKS.Models.mydb.InventoryOption> DeleteInventoryOption(string key)
        {
            var itemToDelete = Context.InventoryOptions
                              .Where(i => i.Key == key)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnInventoryOptionDeleted(itemToDelete);


            Context.InventoryOptions.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterInventoryOptionDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportInventoryProductgroupsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/inventoryproductgroups/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/inventoryproductgroups/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportInventoryProductgroupsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/inventoryproductgroups/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/inventoryproductgroups/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnInventoryProductgroupsRead(ref IQueryable<CHKS.Models.mydb.InventoryProductgroup> items);

        public async Task<IQueryable<CHKS.Models.mydb.InventoryProductgroup>> GetInventoryProductgroups(Query query = null)
        {
            var items = Context.InventoryProductgroups.AsQueryable();


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

            OnInventoryProductgroupsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnInventoryProductgroupGet(CHKS.Models.mydb.InventoryProductgroup item);
        partial void OnGetInventoryProductgroupByKey(ref IQueryable<CHKS.Models.mydb.InventoryProductgroup> items);


        public async Task<CHKS.Models.mydb.InventoryProductgroup> GetInventoryProductgroupByKey(string key)
        {
            var items = Context.InventoryProductgroups
                              .AsNoTracking()
                              .Where(i => i.Key == key);

 
            OnGetInventoryProductgroupByKey(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnInventoryProductgroupGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnInventoryProductgroupCreated(CHKS.Models.mydb.InventoryProductgroup item);
        partial void OnAfterInventoryProductgroupCreated(CHKS.Models.mydb.InventoryProductgroup item);

        public async Task<CHKS.Models.mydb.InventoryProductgroup> CreateInventoryProductgroup(CHKS.Models.mydb.InventoryProductgroup inventoryproductgroup)
        {
            OnInventoryProductgroupCreated(inventoryproductgroup);

            var existingItem = Context.InventoryProductgroups
                              .Where(i => i.Key == inventoryproductgroup.Key)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.InventoryProductgroups.Add(inventoryproductgroup);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(inventoryproductgroup).State = EntityState.Detached;
                throw;
            }

            OnAfterInventoryProductgroupCreated(inventoryproductgroup);

            return inventoryproductgroup;
        }

        public async Task<CHKS.Models.mydb.InventoryProductgroup> CancelInventoryProductgroupChanges(CHKS.Models.mydb.InventoryProductgroup item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnInventoryProductgroupUpdated(CHKS.Models.mydb.InventoryProductgroup item);
        partial void OnAfterInventoryProductgroupUpdated(CHKS.Models.mydb.InventoryProductgroup item);

        public async Task<CHKS.Models.mydb.InventoryProductgroup> UpdateInventoryProductgroup(string key, CHKS.Models.mydb.InventoryProductgroup inventoryproductgroup)
        {
            OnInventoryProductgroupUpdated(inventoryproductgroup);

            var itemToUpdate = Context.InventoryProductgroups
                              .Where(i => i.Key == inventoryproductgroup.Key)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(inventoryproductgroup);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterInventoryProductgroupUpdated(inventoryproductgroup);

            return inventoryproductgroup;
        }

        partial void OnInventoryProductgroupDeleted(CHKS.Models.mydb.InventoryProductgroup item);
        partial void OnAfterInventoryProductgroupDeleted(CHKS.Models.mydb.InventoryProductgroup item);

        public async Task<CHKS.Models.mydb.InventoryProductgroup> DeleteInventoryProductgroup(string key)
        {
            var itemToDelete = Context.InventoryProductgroups
                              .Where(i => i.Key == key)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnInventoryProductgroupDeleted(itemToDelete);


            Context.InventoryProductgroups.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterInventoryProductgroupDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportInventoryTrashcansToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/inventorytrashcans/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/inventorytrashcans/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportInventoryTrashcansToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/inventorytrashcans/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/inventorytrashcans/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnInventoryTrashcansRead(ref IQueryable<CHKS.Models.mydb.InventoryTrashcan> items);

        public async Task<IQueryable<CHKS.Models.mydb.InventoryTrashcan>> GetInventoryTrashcans(Query query = null)
        {
            var items = Context.InventoryTrashcans.AsQueryable();


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

            OnInventoryTrashcansRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnInventoryTrashcanGet(CHKS.Models.mydb.InventoryTrashcan item);
        partial void OnGetInventoryTrashcanByDate(ref IQueryable<CHKS.Models.mydb.InventoryTrashcan> items);


        public async Task<CHKS.Models.mydb.InventoryTrashcan> GetInventoryTrashcanByDate(string date)
        {
            var items = Context.InventoryTrashcans
                              .AsNoTracking()
                              .Where(i => i.Date == date);

 
            OnGetInventoryTrashcanByDate(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnInventoryTrashcanGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnInventoryTrashcanCreated(CHKS.Models.mydb.InventoryTrashcan item);
        partial void OnAfterInventoryTrashcanCreated(CHKS.Models.mydb.InventoryTrashcan item);

        public async Task<CHKS.Models.mydb.InventoryTrashcan> CreateInventoryTrashcan(CHKS.Models.mydb.InventoryTrashcan inventorytrashcan)
        {
            OnInventoryTrashcanCreated(inventorytrashcan);

            var existingItem = Context.InventoryTrashcans
                              .Where(i => i.Date == inventorytrashcan.Date)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.InventoryTrashcans.Add(inventorytrashcan);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(inventorytrashcan).State = EntityState.Detached;
                throw;
            }

            OnAfterInventoryTrashcanCreated(inventorytrashcan);

            return inventorytrashcan;
        }

        public async Task<CHKS.Models.mydb.InventoryTrashcan> CancelInventoryTrashcanChanges(CHKS.Models.mydb.InventoryTrashcan item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnInventoryTrashcanUpdated(CHKS.Models.mydb.InventoryTrashcan item);
        partial void OnAfterInventoryTrashcanUpdated(CHKS.Models.mydb.InventoryTrashcan item);

        public async Task<CHKS.Models.mydb.InventoryTrashcan> UpdateInventoryTrashcan(string date, CHKS.Models.mydb.InventoryTrashcan inventorytrashcan)
        {
            OnInventoryTrashcanUpdated(inventorytrashcan);

            var itemToUpdate = Context.InventoryTrashcans
                              .Where(i => i.Date == inventorytrashcan.Date)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(inventorytrashcan);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterInventoryTrashcanUpdated(inventorytrashcan);

            return inventorytrashcan;
        }

        partial void OnInventoryTrashcanDeleted(CHKS.Models.mydb.InventoryTrashcan item);
        partial void OnAfterInventoryTrashcanDeleted(CHKS.Models.mydb.InventoryTrashcan item);

        public async Task<CHKS.Models.mydb.InventoryTrashcan> DeleteInventoryTrashcan(string date)
        {
            var itemToDelete = Context.InventoryTrashcans
                              .Where(i => i.Date == date)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnInventoryTrashcanDeleted(itemToDelete);


            Context.InventoryTrashcans.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterInventoryTrashcanDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportProductClassesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/productclasses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/productclasses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportProductClassesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/productclasses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/productclasses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnProductClassesRead(ref IQueryable<CHKS.Models.mydb.ProductClass> items);

        public async Task<IQueryable<CHKS.Models.mydb.ProductClass>> GetProductClasses(Query query = null)
        {
            var items = Context.ProductClasses.AsQueryable();


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

            OnProductClassesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnProductClassGet(CHKS.Models.mydb.ProductClass item);
        partial void OnGetProductClassById(ref IQueryable<CHKS.Models.mydb.ProductClass> items);


        public async Task<CHKS.Models.mydb.ProductClass> GetProductClassById(string id)
        {
            var items = Context.ProductClasses
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetProductClassById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnProductClassGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnProductClassCreated(CHKS.Models.mydb.ProductClass item);
        partial void OnAfterProductClassCreated(CHKS.Models.mydb.ProductClass item);

        public async Task<CHKS.Models.mydb.ProductClass> CreateProductClass(CHKS.Models.mydb.ProductClass productclass)
        {
            OnProductClassCreated(productclass);

            var existingItem = Context.ProductClasses
                              .Where(i => i.Id == productclass.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.ProductClasses.Add(productclass);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(productclass).State = EntityState.Detached;
                throw;
            }

            OnAfterProductClassCreated(productclass);

            return productclass;
        }

        public async Task<CHKS.Models.mydb.ProductClass> CancelProductClassChanges(CHKS.Models.mydb.ProductClass item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnProductClassUpdated(CHKS.Models.mydb.ProductClass item);
        partial void OnAfterProductClassUpdated(CHKS.Models.mydb.ProductClass item);

        public async Task<CHKS.Models.mydb.ProductClass> UpdateProductClass(string id, CHKS.Models.mydb.ProductClass productclass)
        {
            OnProductClassUpdated(productclass);

            var itemToUpdate = Context.ProductClasses
                              .Where(i => i.Id == productclass.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(productclass);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterProductClassUpdated(productclass);

            return productclass;
        }

        partial void OnProductClassDeleted(CHKS.Models.mydb.ProductClass item);
        partial void OnAfterProductClassDeleted(CHKS.Models.mydb.ProductClass item);

        public async Task<CHKS.Models.mydb.ProductClass> DeleteProductClass(string id)
        {
            var itemToDelete = Context.ProductClasses
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnProductClassDeleted(itemToDelete);


            Context.ProductClasses.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterProductClassDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}