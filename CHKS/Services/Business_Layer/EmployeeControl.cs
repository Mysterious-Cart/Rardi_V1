using CHKS.Entity;
using CHKS.Models.Interface;
using CHKS.Models;
using CHKS.Data;
using Microsoft.EntityFrameworkCore;

namespace CHKS.Services
{

    public class EmployeeControl
    {

        private readonly IDbProvider dbProvider;
        private readonly Rardi_Context _context;
        private readonly ILogger<InventoryControlService> logger;

        public EmployeeControl(IDbProvider provider, ILogger<InventoryControlService> logger, IDbContextFactory<Rardi_Context> context)
        {
            _context = context.CreateDbContext();
            dbProvider = provider;
            this.logger = logger;
        }
        // Group CRUD
        public async Task<Group> CreateGroup(string name)
        {
            var group = new GroupModel { Id = Random.Shared.Next(), Name = name };
            try
            {
                await _context.Groups.AddAsync(group);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating group");
                return null;
            }
            return Group.FromGroupModel(group);
        }

        private async Task<Group> GetGroup(int id) => Group.FromGroupModel(
            (await dbProvider.GetData<Models.GroupModel>()).First(g => g.Id == id)
        );
        private async Task<Models.GroupModel> GetGroupModel(int id) =>
            (await dbProvider.GetData<Models.GroupModel>()).FirstOrDefault(g => g.Id == id);

        public async Task<IEnumerable<Group>> GetAllGroups()
        {
            var groups = await dbProvider.GetData<Models.GroupModel>();
            var result = groups.Select(i => Group.FromGroupModel(i));
            return result;
        }
        public async Task<bool> UpdateGroup(int id, string newName)
        {

            var group = await GetGroup(id);
            if (group == null) return false;
            group.Name = newName;
            await dbProvider.UpdateData(group, i => i.Id);
            return true;
        }

        public async Task<bool> DeleteGroup(int id)
        {
            var group = _context.Groups.Include(i => i.Employee).FirstOrDefault(i => i.Id == id);
            if (group == null) return false;
            if(group.Employee.Count > 0)group.Employee.Clear(); // Remove all employees from this group
            _context.Remove(group);
            await _context.SaveChangesAsync();
            return true;
        }

        // Employee CRUD
        public async Task<bool> CreateEmployee(string name, int groupId)
        {
            var group = await GetGroupModel(groupId);
            if (group == null) return false;

            var employee = new Models.EmployeeModel { Id = Random.Shared.Next(), Name = name };

            try
            {
                await dbProvider.CreateData(employee);
                employee.Group.Add(group);
                await dbProvider.UpdateData(employee, i => i.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating employee");
                return false;
            }
            return true;
        }

        private async Task<Employee> GetEmployee(int id) => Employee.FromEmployeeModel(
            (await dbProvider.GetData<Models.EmployeeModel>()).FirstOrDefault(g => g.Id == id)
        );

        public async Task<IEnumerable<Employee>> GetAllEmployees(bool Expand_Group = true)
        {
            var employees =
                Expand_Group is true ?
                    await dbProvider.GetData<Models.EmployeeModel>([nameof(Models.EmployeeModel.Group)]) :
                    await dbProvider.GetData<Models.EmployeeModel>();

            var result = employees.Select(i => Employee.FromEmployeeModel(i));
            return result;
        }

        public async Task<bool> UpdateEmployee(int id, string newName)
        {
            var emp = await GetEmployee(id);
            if (emp == null) return false;
            emp.Name = newName;
            await dbProvider.UpdateData(emp, i => i.Id);
            return true;
        }

        public async Task<bool> DeleteEmployee(int id)
        {
            var emp = await dbProvider.GetDataWithChangeTracking<Models.EmployeeModel, int>(id);

            if (emp == null) return false;
            // Remove from group if assigned
            emp.Group.Clear();
            await dbProvider.SaveChangesAsync();

            await dbProvider.DeleteData<Employee, int>(id);
            return true;
        }

        public async Task<IEnumerable<StockLogs>> GetEmployeeLogs(int EmployeeId)
        {
            var stocklogs = await dbProvider.GetData<Models.StockLogs>();
            var employeelog = stocklogs.Where(i => i.EmployeeId.Equals(EmployeeId));
            return employeelog.Select(i => StockLogs.FromModelStockLogs(ref i));
        }

        // Assign Employee to Group
        public async Task<bool> AssignEmployeeToGroup(int employeeId, int groupId)
        {
            var emp = await GetEmployee(employeeId);
            var group = await GetGroup(groupId);
            if (emp == null || group == null) return false;

            // Remove from previous group if assigned
            emp.Group.Clear();
            await dbProvider.SaveChangesAsync();

            emp.Group.Add(group);
            await dbProvider.SaveChangesAsync();

            return true;
        }
    }
}