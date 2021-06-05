using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;

namespace leave_management.Repository
{
    public class LeaveAllocationRepository : ILeaveAllocationRepository 
    {
        private readonly ApplicationDbContext _db;

        public LeaveAllocationRepository(ApplicationDbContext db)
        {
            _db = db; 
        }

        public bool CheckAllocation(int leavetypeid, string employeeid)
        {
            var period = DateTime.Now.Year;
            return FindAll()
                .Where(q => q.EmployeeId == employeeid && q.LeaveTypeId == leavetypeid && q.Period == period)
                .Any();
        }

        public bool Create(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Add(entity);
            return Save(); 
        }

        public bool Delete(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Remove(entity);
            return Save(); 
        }

        public ICollection<LeaveAllocation> FindAll()
        {
            var LeaveAllocations = _db.LeaveAllocations
                .Include(q => q.LeaveType)
                
                .ToList();
            return LeaveAllocations; 
        }

        public LeaveAllocation FindById(int id)
        {
            var LeaveAllocation = _db.LeaveAllocations
                .Include(q => q.LeaveType)
                .Include(q => q.Employee)
                .FirstOrDefault(q => q.Id == id);
            return LeaveAllocation; 
        }

        public ICollection<LeaveAllocation> GetLeaveAllocationsByEmployee(string id)
        {
            var period = DateTime.Now.Year;
            Console.WriteLine("GetLeaveAllocationsByEmployee(string id)" + id);
            return FindAll().
                    Where(q => q.EmployeeId == id && q.Period == period)
                    .ToList(); 
        }

        public LeaveAllocation GetLeaveAllocationByEmployeeAndType(string employeeid, int leavetypeid)
        {
            var period = DateTime.Now.Year;

            return FindAll().
                    FirstOrDefault(q => q.EmployeeId == employeeid && q.Period == period && q.LeaveTypeId == leavetypeid); 
        }

        public bool IsPresent(int id)
        {
            var present = _db.LeaveAllocations.Any(q => q.Id == id);
            return present;
        }

        public bool Save()
        {
           var changes =  _db.SaveChanges();
            Thread.Sleep(2000);

            return changes > 0; 
        }

        public bool Update(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Update(entity);
            return Save(); 
        }
    }
}
