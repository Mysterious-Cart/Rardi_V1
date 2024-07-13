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


        public async Task ExportEfmigrationshistoriesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/efmigrationshistories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/efmigrationshistories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportEfmigrationshistoriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/efmigrationshistories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/efmigrationshistories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnEfmigrationshistoriesRead(ref IQueryable<CHKS.Models.mydb.Efmigrationshistory> items);

        public async Task<IQueryable<CHKS.Models.mydb.Efmigrationshistory>> GetEfmigrationshistories(Query query = null)
        {
            var items = Context.Efmigrationshistories.AsQueryable();


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

            OnEfmigrationshistoriesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnEfmigrationshistoryGet(CHKS.Models.mydb.Efmigrationshistory item);
        partial void OnGetEfmigrationshistoryByMigrationId(ref IQueryable<CHKS.Models.mydb.Efmigrationshistory> items);


        public async Task<CHKS.Models.mydb.Efmigrationshistory> GetEfmigrationshistoryByMigrationId(string migrationid)
        {
            var items = Context.Efmigrationshistories
                              .AsNoTracking()
                              .Where(i => i.MigrationId == migrationid);

 
            OnGetEfmigrationshistoryByMigrationId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnEfmigrationshistoryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnEfmigrationshistoryCreated(CHKS.Models.mydb.Efmigrationshistory item);
        partial void OnAfterEfmigrationshistoryCreated(CHKS.Models.mydb.Efmigrationshistory item);

        public async Task<CHKS.Models.mydb.Efmigrationshistory> CreateEfmigrationshistory(CHKS.Models.mydb.Efmigrationshistory efmigrationshistory)
        {
            OnEfmigrationshistoryCreated(efmigrationshistory);

            var existingItem = Context.Efmigrationshistories
                              .Where(i => i.MigrationId == efmigrationshistory.MigrationId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Efmigrationshistories.Add(efmigrationshistory);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(efmigrationshistory).State = EntityState.Detached;
                throw;
            }

            OnAfterEfmigrationshistoryCreated(efmigrationshistory);

            return efmigrationshistory;
        }

        public async Task<CHKS.Models.mydb.Efmigrationshistory> CancelEfmigrationshistoryChanges(CHKS.Models.mydb.Efmigrationshistory item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnEfmigrationshistoryUpdated(CHKS.Models.mydb.Efmigrationshistory item);
        partial void OnAfterEfmigrationshistoryUpdated(CHKS.Models.mydb.Efmigrationshistory item);

        public async Task<CHKS.Models.mydb.Efmigrationshistory> UpdateEfmigrationshistory(string migrationid, CHKS.Models.mydb.Efmigrationshistory efmigrationshistory)
        {
            OnEfmigrationshistoryUpdated(efmigrationshistory);

            var itemToUpdate = Context.Efmigrationshistories
                              .Where(i => i.MigrationId == efmigrationshistory.MigrationId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(efmigrationshistory);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterEfmigrationshistoryUpdated(efmigrationshistory);

            return efmigrationshistory;
        }

        partial void OnEfmigrationshistoryDeleted(CHKS.Models.mydb.Efmigrationshistory item);
        partial void OnAfterEfmigrationshistoryDeleted(CHKS.Models.mydb.Efmigrationshistory item);

        public async Task<CHKS.Models.mydb.Efmigrationshistory> DeleteEfmigrationshistory(string migrationid)
        {
            var itemToDelete = Context.Efmigrationshistories
                              .Where(i => i.MigrationId == migrationid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnEfmigrationshistoryDeleted(itemToDelete);


            Context.Efmigrationshistories.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterEfmigrationshistoryDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspnetroleclaimsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/aspnetroleclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/aspnetroleclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspnetroleclaimsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/aspnetroleclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/aspnetroleclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspnetroleclaimsRead(ref IQueryable<CHKS.Models.mydb.Aspnetroleclaim> items);

        public async Task<IQueryable<CHKS.Models.mydb.Aspnetroleclaim>> GetAspnetroleclaims(Query query = null)
        {
            var items = Context.Aspnetroleclaims.AsQueryable();

            items = items.Include(i => i.Aspnetrole);

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

            OnAspnetroleclaimsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspnetroleclaimGet(CHKS.Models.mydb.Aspnetroleclaim item);
        partial void OnGetAspnetroleclaimById(ref IQueryable<CHKS.Models.mydb.Aspnetroleclaim> items);


        public async Task<CHKS.Models.mydb.Aspnetroleclaim> GetAspnetroleclaimById(int id)
        {
            var items = Context.Aspnetroleclaims
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Aspnetrole);
 
            OnGetAspnetroleclaimById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspnetroleclaimGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspnetroleclaimCreated(CHKS.Models.mydb.Aspnetroleclaim item);
        partial void OnAfterAspnetroleclaimCreated(CHKS.Models.mydb.Aspnetroleclaim item);

        public async Task<CHKS.Models.mydb.Aspnetroleclaim> CreateAspnetroleclaim(CHKS.Models.mydb.Aspnetroleclaim aspnetroleclaim)
        {
            OnAspnetroleclaimCreated(aspnetroleclaim);

            var existingItem = Context.Aspnetroleclaims
                              .Where(i => i.Id == aspnetroleclaim.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Aspnetroleclaims.Add(aspnetroleclaim);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetroleclaim).State = EntityState.Detached;
                throw;
            }

            OnAfterAspnetroleclaimCreated(aspnetroleclaim);

            return aspnetroleclaim;
        }

        public async Task<CHKS.Models.mydb.Aspnetroleclaim> CancelAspnetroleclaimChanges(CHKS.Models.mydb.Aspnetroleclaim item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspnetroleclaimUpdated(CHKS.Models.mydb.Aspnetroleclaim item);
        partial void OnAfterAspnetroleclaimUpdated(CHKS.Models.mydb.Aspnetroleclaim item);

        public async Task<CHKS.Models.mydb.Aspnetroleclaim> UpdateAspnetroleclaim(int id, CHKS.Models.mydb.Aspnetroleclaim aspnetroleclaim)
        {
            OnAspnetroleclaimUpdated(aspnetroleclaim);

            var itemToUpdate = Context.Aspnetroleclaims
                              .Where(i => i.Id == aspnetroleclaim.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetroleclaim);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspnetroleclaimUpdated(aspnetroleclaim);

            return aspnetroleclaim;
        }

        partial void OnAspnetroleclaimDeleted(CHKS.Models.mydb.Aspnetroleclaim item);
        partial void OnAfterAspnetroleclaimDeleted(CHKS.Models.mydb.Aspnetroleclaim item);

        public async Task<CHKS.Models.mydb.Aspnetroleclaim> DeleteAspnetroleclaim(int id)
        {
            var itemToDelete = Context.Aspnetroleclaims
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspnetroleclaimDeleted(itemToDelete);


            Context.Aspnetroleclaims.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspnetroleclaimDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspnetrolesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/aspnetroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/aspnetroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspnetrolesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/aspnetroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/aspnetroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspnetrolesRead(ref IQueryable<CHKS.Models.mydb.Aspnetrole> items);

        public async Task<IQueryable<CHKS.Models.mydb.Aspnetrole>> GetAspnetroles(Query query = null)
        {
            var items = Context.Aspnetroles.AsQueryable();


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

            OnAspnetrolesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspnetroleGet(CHKS.Models.mydb.Aspnetrole item);
        partial void OnGetAspnetroleById(ref IQueryable<CHKS.Models.mydb.Aspnetrole> items);


        public async Task<CHKS.Models.mydb.Aspnetrole> GetAspnetroleById(string id)
        {
            var items = Context.Aspnetroles
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetAspnetroleById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspnetroleGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspnetroleCreated(CHKS.Models.mydb.Aspnetrole item);
        partial void OnAfterAspnetroleCreated(CHKS.Models.mydb.Aspnetrole item);

        public async Task<CHKS.Models.mydb.Aspnetrole> CreateAspnetrole(CHKS.Models.mydb.Aspnetrole aspnetrole)
        {
            OnAspnetroleCreated(aspnetrole);

            var existingItem = Context.Aspnetroles
                              .Where(i => i.Id == aspnetrole.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Aspnetroles.Add(aspnetrole);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetrole).State = EntityState.Detached;
                throw;
            }

            OnAfterAspnetroleCreated(aspnetrole);

            return aspnetrole;
        }

        public async Task<CHKS.Models.mydb.Aspnetrole> CancelAspnetroleChanges(CHKS.Models.mydb.Aspnetrole item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspnetroleUpdated(CHKS.Models.mydb.Aspnetrole item);
        partial void OnAfterAspnetroleUpdated(CHKS.Models.mydb.Aspnetrole item);

        public async Task<CHKS.Models.mydb.Aspnetrole> UpdateAspnetrole(string id, CHKS.Models.mydb.Aspnetrole aspnetrole)
        {
            OnAspnetroleUpdated(aspnetrole);

            var itemToUpdate = Context.Aspnetroles
                              .Where(i => i.Id == aspnetrole.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetrole);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspnetroleUpdated(aspnetrole);

            return aspnetrole;
        }

        partial void OnAspnetroleDeleted(CHKS.Models.mydb.Aspnetrole item);
        partial void OnAfterAspnetroleDeleted(CHKS.Models.mydb.Aspnetrole item);

        public async Task<CHKS.Models.mydb.Aspnetrole> DeleteAspnetrole(string id)
        {
            var itemToDelete = Context.Aspnetroles
                              .Where(i => i.Id == id)
                              .Include(i => i.Aspnetroleclaims)
                              .Include(i => i.Aspnetuserroles)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspnetroleDeleted(itemToDelete);


            Context.Aspnetroles.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspnetroleDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspnetuserclaimsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/aspnetuserclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/aspnetuserclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspnetuserclaimsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/aspnetuserclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/aspnetuserclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspnetuserclaimsRead(ref IQueryable<CHKS.Models.mydb.Aspnetuserclaim> items);

        public async Task<IQueryable<CHKS.Models.mydb.Aspnetuserclaim>> GetAspnetuserclaims(Query query = null)
        {
            var items = Context.Aspnetuserclaims.AsQueryable();

            items = items.Include(i => i.Aspnetuser);

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

            OnAspnetuserclaimsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspnetuserclaimGet(CHKS.Models.mydb.Aspnetuserclaim item);
        partial void OnGetAspnetuserclaimById(ref IQueryable<CHKS.Models.mydb.Aspnetuserclaim> items);


        public async Task<CHKS.Models.mydb.Aspnetuserclaim> GetAspnetuserclaimById(int id)
        {
            var items = Context.Aspnetuserclaims
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Aspnetuser);
 
            OnGetAspnetuserclaimById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspnetuserclaimGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspnetuserclaimCreated(CHKS.Models.mydb.Aspnetuserclaim item);
        partial void OnAfterAspnetuserclaimCreated(CHKS.Models.mydb.Aspnetuserclaim item);

        public async Task<CHKS.Models.mydb.Aspnetuserclaim> CreateAspnetuserclaim(CHKS.Models.mydb.Aspnetuserclaim aspnetuserclaim)
        {
            OnAspnetuserclaimCreated(aspnetuserclaim);

            var existingItem = Context.Aspnetuserclaims
                              .Where(i => i.Id == aspnetuserclaim.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Aspnetuserclaims.Add(aspnetuserclaim);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetuserclaim).State = EntityState.Detached;
                throw;
            }

            OnAfterAspnetuserclaimCreated(aspnetuserclaim);

            return aspnetuserclaim;
        }

        public async Task<CHKS.Models.mydb.Aspnetuserclaim> CancelAspnetuserclaimChanges(CHKS.Models.mydb.Aspnetuserclaim item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspnetuserclaimUpdated(CHKS.Models.mydb.Aspnetuserclaim item);
        partial void OnAfterAspnetuserclaimUpdated(CHKS.Models.mydb.Aspnetuserclaim item);

        public async Task<CHKS.Models.mydb.Aspnetuserclaim> UpdateAspnetuserclaim(int id, CHKS.Models.mydb.Aspnetuserclaim aspnetuserclaim)
        {
            OnAspnetuserclaimUpdated(aspnetuserclaim);

            var itemToUpdate = Context.Aspnetuserclaims
                              .Where(i => i.Id == aspnetuserclaim.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetuserclaim);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspnetuserclaimUpdated(aspnetuserclaim);

            return aspnetuserclaim;
        }

        partial void OnAspnetuserclaimDeleted(CHKS.Models.mydb.Aspnetuserclaim item);
        partial void OnAfterAspnetuserclaimDeleted(CHKS.Models.mydb.Aspnetuserclaim item);

        public async Task<CHKS.Models.mydb.Aspnetuserclaim> DeleteAspnetuserclaim(int id)
        {
            var itemToDelete = Context.Aspnetuserclaims
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspnetuserclaimDeleted(itemToDelete);


            Context.Aspnetuserclaims.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspnetuserclaimDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspnetuserloginsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/aspnetuserlogins/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/aspnetuserlogins/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspnetuserloginsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/aspnetuserlogins/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/aspnetuserlogins/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspnetuserloginsRead(ref IQueryable<CHKS.Models.mydb.Aspnetuserlogin> items);

        public async Task<IQueryable<CHKS.Models.mydb.Aspnetuserlogin>> GetAspnetuserlogins(Query query = null)
        {
            var items = Context.Aspnetuserlogins.AsQueryable();

            items = items.Include(i => i.Aspnetuser);

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

            OnAspnetuserloginsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspnetuserloginGet(CHKS.Models.mydb.Aspnetuserlogin item);
        partial void OnGetAspnetuserloginByLoginProviderAndProviderKey(ref IQueryable<CHKS.Models.mydb.Aspnetuserlogin> items);


        public async Task<CHKS.Models.mydb.Aspnetuserlogin> GetAspnetuserloginByLoginProviderAndProviderKey(string loginprovider, string providerkey)
        {
            var items = Context.Aspnetuserlogins
                              .AsNoTracking()
                              .Where(i => i.LoginProvider == loginprovider && i.ProviderKey == providerkey);

            items = items.Include(i => i.Aspnetuser);
 
            OnGetAspnetuserloginByLoginProviderAndProviderKey(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspnetuserloginGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspnetuserloginCreated(CHKS.Models.mydb.Aspnetuserlogin item);
        partial void OnAfterAspnetuserloginCreated(CHKS.Models.mydb.Aspnetuserlogin item);

        public async Task<CHKS.Models.mydb.Aspnetuserlogin> CreateAspnetuserlogin(CHKS.Models.mydb.Aspnetuserlogin aspnetuserlogin)
        {
            OnAspnetuserloginCreated(aspnetuserlogin);

            var existingItem = Context.Aspnetuserlogins
                              .Where(i => i.LoginProvider == aspnetuserlogin.LoginProvider && i.ProviderKey == aspnetuserlogin.ProviderKey)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Aspnetuserlogins.Add(aspnetuserlogin);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetuserlogin).State = EntityState.Detached;
                throw;
            }

            OnAfterAspnetuserloginCreated(aspnetuserlogin);

            return aspnetuserlogin;
        }

        public async Task<CHKS.Models.mydb.Aspnetuserlogin> CancelAspnetuserloginChanges(CHKS.Models.mydb.Aspnetuserlogin item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspnetuserloginUpdated(CHKS.Models.mydb.Aspnetuserlogin item);
        partial void OnAfterAspnetuserloginUpdated(CHKS.Models.mydb.Aspnetuserlogin item);

        public async Task<CHKS.Models.mydb.Aspnetuserlogin> UpdateAspnetuserlogin(string loginprovider, string providerkey, CHKS.Models.mydb.Aspnetuserlogin aspnetuserlogin)
        {
            OnAspnetuserloginUpdated(aspnetuserlogin);

            var itemToUpdate = Context.Aspnetuserlogins
                              .Where(i => i.LoginProvider == aspnetuserlogin.LoginProvider && i.ProviderKey == aspnetuserlogin.ProviderKey)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetuserlogin);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspnetuserloginUpdated(aspnetuserlogin);

            return aspnetuserlogin;
        }

        partial void OnAspnetuserloginDeleted(CHKS.Models.mydb.Aspnetuserlogin item);
        partial void OnAfterAspnetuserloginDeleted(CHKS.Models.mydb.Aspnetuserlogin item);

        public async Task<CHKS.Models.mydb.Aspnetuserlogin> DeleteAspnetuserlogin(string loginprovider, string providerkey)
        {
            var itemToDelete = Context.Aspnetuserlogins
                              .Where(i => i.LoginProvider == loginprovider && i.ProviderKey == providerkey)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspnetuserloginDeleted(itemToDelete);


            Context.Aspnetuserlogins.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspnetuserloginDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspnetuserrolesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/aspnetuserroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/aspnetuserroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspnetuserrolesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/aspnetuserroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/aspnetuserroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspnetuserrolesRead(ref IQueryable<CHKS.Models.mydb.Aspnetuserrole> items);

        public async Task<IQueryable<CHKS.Models.mydb.Aspnetuserrole>> GetAspnetuserroles(Query query = null)
        {
            var items = Context.Aspnetuserroles.AsQueryable();

            items = items.Include(i => i.Aspnetrole);
            items = items.Include(i => i.Aspnetuser);

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

            OnAspnetuserrolesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspnetuserroleGet(CHKS.Models.mydb.Aspnetuserrole item);
        partial void OnGetAspnetuserroleByUserIdAndRoleId(ref IQueryable<CHKS.Models.mydb.Aspnetuserrole> items);


        public async Task<CHKS.Models.mydb.Aspnetuserrole> GetAspnetuserroleByUserIdAndRoleId(string userid, string roleid)
        {
            var items = Context.Aspnetuserroles
                              .AsNoTracking()
                              .Where(i => i.UserId == userid && i.RoleId == roleid);

            items = items.Include(i => i.Aspnetrole);
            items = items.Include(i => i.Aspnetuser);
 
            OnGetAspnetuserroleByUserIdAndRoleId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspnetuserroleGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspnetuserroleCreated(CHKS.Models.mydb.Aspnetuserrole item);
        partial void OnAfterAspnetuserroleCreated(CHKS.Models.mydb.Aspnetuserrole item);

        public async Task<CHKS.Models.mydb.Aspnetuserrole> CreateAspnetuserrole(CHKS.Models.mydb.Aspnetuserrole aspnetuserrole)
        {
            OnAspnetuserroleCreated(aspnetuserrole);

            var existingItem = Context.Aspnetuserroles
                              .Where(i => i.UserId == aspnetuserrole.UserId && i.RoleId == aspnetuserrole.RoleId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Aspnetuserroles.Add(aspnetuserrole);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetuserrole).State = EntityState.Detached;
                throw;
            }

            OnAfterAspnetuserroleCreated(aspnetuserrole);

            return aspnetuserrole;
        }

        public async Task<CHKS.Models.mydb.Aspnetuserrole> CancelAspnetuserroleChanges(CHKS.Models.mydb.Aspnetuserrole item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspnetuserroleUpdated(CHKS.Models.mydb.Aspnetuserrole item);
        partial void OnAfterAspnetuserroleUpdated(CHKS.Models.mydb.Aspnetuserrole item);

        public async Task<CHKS.Models.mydb.Aspnetuserrole> UpdateAspnetuserrole(string userid, string roleid, CHKS.Models.mydb.Aspnetuserrole aspnetuserrole)
        {
            OnAspnetuserroleUpdated(aspnetuserrole);

            var itemToUpdate = Context.Aspnetuserroles
                              .Where(i => i.UserId == aspnetuserrole.UserId && i.RoleId == aspnetuserrole.RoleId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetuserrole);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspnetuserroleUpdated(aspnetuserrole);

            return aspnetuserrole;
        }

        partial void OnAspnetuserroleDeleted(CHKS.Models.mydb.Aspnetuserrole item);
        partial void OnAfterAspnetuserroleDeleted(CHKS.Models.mydb.Aspnetuserrole item);

        public async Task<CHKS.Models.mydb.Aspnetuserrole> DeleteAspnetuserrole(string userid, string roleid)
        {
            var itemToDelete = Context.Aspnetuserroles
                              .Where(i => i.UserId == userid && i.RoleId == roleid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspnetuserroleDeleted(itemToDelete);


            Context.Aspnetuserroles.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspnetuserroleDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspnetusersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/aspnetusers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/aspnetusers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspnetusersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/aspnetusers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/aspnetusers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspnetusersRead(ref IQueryable<CHKS.Models.mydb.Aspnetuser> items);

        public async Task<IQueryable<CHKS.Models.mydb.Aspnetuser>> GetAspnetusers(Query query = null)
        {
            var items = Context.Aspnetusers.AsQueryable();


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

            OnAspnetusersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspnetuserGet(CHKS.Models.mydb.Aspnetuser item);
        partial void OnGetAspnetuserById(ref IQueryable<CHKS.Models.mydb.Aspnetuser> items);


        public async Task<CHKS.Models.mydb.Aspnetuser> GetAspnetuserById(string id)
        {
            var items = Context.Aspnetusers
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetAspnetuserById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspnetuserGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspnetuserCreated(CHKS.Models.mydb.Aspnetuser item);
        partial void OnAfterAspnetuserCreated(CHKS.Models.mydb.Aspnetuser item);

        public async Task<CHKS.Models.mydb.Aspnetuser> CreateAspnetuser(CHKS.Models.mydb.Aspnetuser aspnetuser)
        {
            OnAspnetuserCreated(aspnetuser);

            var existingItem = Context.Aspnetusers
                              .Where(i => i.Id == aspnetuser.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Aspnetusers.Add(aspnetuser);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetuser).State = EntityState.Detached;
                throw;
            }

            OnAfterAspnetuserCreated(aspnetuser);

            return aspnetuser;
        }

        public async Task<CHKS.Models.mydb.Aspnetuser> CancelAspnetuserChanges(CHKS.Models.mydb.Aspnetuser item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspnetuserUpdated(CHKS.Models.mydb.Aspnetuser item);
        partial void OnAfterAspnetuserUpdated(CHKS.Models.mydb.Aspnetuser item);

        public async Task<CHKS.Models.mydb.Aspnetuser> UpdateAspnetuser(string id, CHKS.Models.mydb.Aspnetuser aspnetuser)
        {
            OnAspnetuserUpdated(aspnetuser);

            var itemToUpdate = Context.Aspnetusers
                              .Where(i => i.Id == aspnetuser.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetuser);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspnetuserUpdated(aspnetuser);

            return aspnetuser;
        }

        partial void OnAspnetuserDeleted(CHKS.Models.mydb.Aspnetuser item);
        partial void OnAfterAspnetuserDeleted(CHKS.Models.mydb.Aspnetuser item);

        public async Task<CHKS.Models.mydb.Aspnetuser> DeleteAspnetuser(string id)
        {
            var itemToDelete = Context.Aspnetusers
                              .Where(i => i.Id == id)
                              .Include(i => i.Aspnetuserclaims)
                              .Include(i => i.Aspnetuserlogins)
                              .Include(i => i.Aspnetuserroles)
                              .Include(i => i.Aspnetusertokens)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspnetuserDeleted(itemToDelete);


            Context.Aspnetusers.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspnetuserDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspnetusertokensToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/aspnetusertokens/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/aspnetusertokens/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspnetusertokensToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/mydb/aspnetusertokens/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/mydb/aspnetusertokens/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspnetusertokensRead(ref IQueryable<CHKS.Models.mydb.Aspnetusertoken> items);

        public async Task<IQueryable<CHKS.Models.mydb.Aspnetusertoken>> GetAspnetusertokens(Query query = null)
        {
            var items = Context.Aspnetusertokens.AsQueryable();

            items = items.Include(i => i.Aspnetuser);

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

            OnAspnetusertokensRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspnetusertokenGet(CHKS.Models.mydb.Aspnetusertoken item);
        partial void OnGetAspnetusertokenByUserIdAndLoginProviderAndName(ref IQueryable<CHKS.Models.mydb.Aspnetusertoken> items);


        public async Task<CHKS.Models.mydb.Aspnetusertoken> GetAspnetusertokenByUserIdAndLoginProviderAndName(string userid, string loginprovider, string name)
        {
            var items = Context.Aspnetusertokens
                              .AsNoTracking()
                              .Where(i => i.UserId == userid && i.LoginProvider == loginprovider && i.Name == name);

            items = items.Include(i => i.Aspnetuser);
 
            OnGetAspnetusertokenByUserIdAndLoginProviderAndName(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspnetusertokenGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspnetusertokenCreated(CHKS.Models.mydb.Aspnetusertoken item);
        partial void OnAfterAspnetusertokenCreated(CHKS.Models.mydb.Aspnetusertoken item);

        public async Task<CHKS.Models.mydb.Aspnetusertoken> CreateAspnetusertoken(CHKS.Models.mydb.Aspnetusertoken aspnetusertoken)
        {
            OnAspnetusertokenCreated(aspnetusertoken);

            var existingItem = Context.Aspnetusertokens
                              .Where(i => i.UserId == aspnetusertoken.UserId && i.LoginProvider == aspnetusertoken.LoginProvider && i.Name == aspnetusertoken.Name)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Aspnetusertokens.Add(aspnetusertoken);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetusertoken).State = EntityState.Detached;
                throw;
            }

            OnAfterAspnetusertokenCreated(aspnetusertoken);

            return aspnetusertoken;
        }

        public async Task<CHKS.Models.mydb.Aspnetusertoken> CancelAspnetusertokenChanges(CHKS.Models.mydb.Aspnetusertoken item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspnetusertokenUpdated(CHKS.Models.mydb.Aspnetusertoken item);
        partial void OnAfterAspnetusertokenUpdated(CHKS.Models.mydb.Aspnetusertoken item);

        public async Task<CHKS.Models.mydb.Aspnetusertoken> UpdateAspnetusertoken(string userid, string loginprovider, string name, CHKS.Models.mydb.Aspnetusertoken aspnetusertoken)
        {
            OnAspnetusertokenUpdated(aspnetusertoken);

            var itemToUpdate = Context.Aspnetusertokens
                              .Where(i => i.UserId == aspnetusertoken.UserId && i.LoginProvider == aspnetusertoken.LoginProvider && i.Name == aspnetusertoken.Name)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetusertoken);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspnetusertokenUpdated(aspnetusertoken);

            return aspnetusertoken;
        }

        partial void OnAspnetusertokenDeleted(CHKS.Models.mydb.Aspnetusertoken item);
        partial void OnAfterAspnetusertokenDeleted(CHKS.Models.mydb.Aspnetusertoken item);

        public async Task<CHKS.Models.mydb.Aspnetusertoken> DeleteAspnetusertoken(string userid, string loginprovider, string name)
        {
            var itemToDelete = Context.Aspnetusertokens
                              .Where(i => i.UserId == userid && i.LoginProvider == loginprovider && i.Name == name)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspnetusertokenDeleted(itemToDelete);


            Context.Aspnetusertokens.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspnetusertokenDeleted(itemToDelete);

            return itemToDelete;
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
        }
}