using CHKS.Entity;
using CHKS.Mappers;
using CHKS.Models;
using CHKS.Data;
using Microsoft.EntityFrameworkCore;

namespace CHKS.Services
{

    public class EmployeeControl(ILogger<InventoryControlService> logger, IDbContextFactory<Rardi_Context> context)
    {

        private readonly Rardi_Context _context = context.CreateDbContext();
        private readonly ILogger<InventoryControlService> logger = logger;

        /// <summary>
        /// Creates a new group with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Group> CreateGroup(string name)
        {
            var group = new GroupModel { Id = Random.Shared.Next(), Name = name };
            try
            {
                await _context.Groups.AddAsync(group);
                await _context.SaveChangesAsync();
                return GroupMapper.ToGroup(group);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Database update error while creating group");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating group");
                throw new InvalidOperationException("Failed to create group.", ex);
            }
            
        }
        /// <summary>
        /// Retrieves a group by its ID, including its employees.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Group> GetGroup(int id)
        {
            try
            {
                return _context.Groups
                .Include(i => i.Employee)
                .Select(GroupExpressionMapper.ToGroup())
                .First(i => i.Id == id);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, "Group with ID {Id} not found", id);
                return null; 
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving group with ID {Id}", id);
                throw new InvalidOperationException("Failed to retrieve group.", ex);
            }
        }
        private async Task<GroupModel> GetGroupModel(int id)
        {
            try
            {
                return _context.Groups
                .Include(i => i.Employee)
                .First(i => i.Id == id);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, "Group with ID {Id} not found", id);
                return null; 
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving group with ID {Id}", id);
                throw new InvalidOperationException("Failed to retrieve group.", ex);
            }
        }
        

        /// <summary>
        /// Retrieves all groups, including their employees.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Group>> GetAllGroups()
        {
            return await _context.Groups
                .Include(i => i.Employee)
                .Select(GroupExpressionMapper.ToGroup())
                .ToListAsync();
        }

        /// <summary>
        /// Renames a group by its ID.
        /// If the group does not exist, an exception is thrown.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newName"></param>
        /// <returns cref="Group"></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Group> RenameGroup(int id, string newName)
        {
            if(await GetGroupModel(id) == null)
            {
                return null; // Group does not exist
            }

            try
            {
                await _context.Groups.
                    Where(i => i.Id == id)
                    .ExecuteUpdateAsync(i => i.SetProperty(g => g.Name, newName));
                return await GetGroup(id);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Database update error while renaming group");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error renaming group");
                throw new InvalidOperationException("Failed to rename group.", ex);
            }
        }
        /// <summary>
        /// Deletes a group by its ID.
        /// If the group has employees, they will be removed from the group.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteGroup(int id)
        {
            var group = await GetGroupModel(id);
            if (group == null)
            {
                return false; // Group does not exist
            }

            try
            {
                if (group.Employee.Count > 0) group.Employee.Clear(); // Remove all employees from this group
                _context.Remove(group);
                await _context.SaveChangesAsync();
                return true; // Group deleted successfully
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Database update error while deleting group");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting group");
                throw new InvalidOperationException("Failed to delete group.", ex);
            }
            
        }

        // Employee CRUD
        public async Task<bool> CreateEmployee(string name, int groupId)
        {
            var group = await GetGroupModel(groupId);
            // Check if group exists
            if (group == null) return false;

            var employee = new EmployeeModel {Name = name };
            using var transaction = _context.Database.BeginTransaction();

            try
            {

                await _context.AddAsync(group);
                await _context.SaveChangesAsync();
                employee.Group.Add(group);
                await _context.AddAsync(employee);

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                logger.LogError(ex, "Error creating employee");
                return false;
            }
            return true;
        }
        /// <summary>
        /// Retrieves an employee by their ID, including their group.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<Employee> GetEmployee(int id)
        {
            return await _context.Employees
                .Include(i => i.Group)
                .Select(EmployeeExpressionMapper.ToEmployee())
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        /// <summary>
        /// Retrieves all employees
        /// </summary>
        /// <param name="Expand_Group"></param>
        /// <returns></returns>
        public async Task<List<Employee>> GetAllEmployees()
        {
            return  await _context.Employees
                        .Include(i => i.Group)
                        .Select(EmployeeExpressionMapper.ToEmployee())
                        .ToListAsync();

        }

        /// <summary>
        /// Renames an employee by their ID.
        /// If the employee does not exist, an exception is thrown.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newName"></param>
        /// <returns cref="bool">True is delete successfully, otherwise False.</returns>

        public async Task<bool> RenameEmployee(int id, string newName)
        {
            var Employee = await GetEmployee(id);
            if (Employee == null) return false;
            try
            {
                await _context.Employees
                    .Where(i => i.Id == id)
                    .ExecuteUpdateAsync(i => i.SetProperty(e => e.Name, newName));
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Database update error while renaming employee");
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error renaming employee");
                return false;
            }
            return true;
        }
        /// <summary>
        /// Deletes an employee by their ID.
        /// If the employee does not exist, it returns false.
        /// </summary>
        /// <param name="id"></param>
        /// <returns cref="bool">
        ///     True if the employee was deleted successfully, otherwise False.
        /// </returns>
        public async Task<bool> DeleteEmployee(int id)
        {
            var emp = await GetEmployee(id);
            if (emp == null) return false;

            try
            {
                // Remove employee from database
                await _context.Employees
                    .Where(i => i.Id == id)
                    .ExecuteDeleteAsync();
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Database update error while deleting employee");
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting employee");
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Retrieves all stock logs for a specific employee by their ID.
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        public async Task<List<StockLogs>> GetEmployeeLogs(int EmployeeId)
        {
            return await _context.StockLogs
                .Include(i => i.Employee)
                .Where(i => i.EmployeeId == EmployeeId)
                .Select(StockLogsExpressionMapper.ToStockLogs())
                .ToListAsync();
        }

        /// <summary>
        /// Assigns an employee to a group.
        /// If the employee or group does not exist, it returns false.
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">
        ///     Throws if there is an error during the assignment process, usually not caused by database.
        /// </exception>
        public async Task<bool> AssignEmployeeToGroup(int employeeId, int groupId)
        {
            var Employee = await GetEmployee(employeeId);
            var Group = await GetGroup(groupId);
            if (Employee == null || Group == null) return false;
            try
            {
                // Remove from previous group if assigned
                Employee.Group.Clear();
                await _context.SaveChangesAsync();

                Employee.Group.Add(Group);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Database update error while assigning employee to group");
                throw;                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error assigning employee to group");
                throw new InvalidOperationException("Failed to assign employee to group.", ex);
            }
            
        }
    }
}