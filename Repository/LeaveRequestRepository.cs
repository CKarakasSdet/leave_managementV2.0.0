using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;

namespace leave_management.Repository
{
    public class LeaveRequestRepository : ILeaveRequestRepository 
    {
        private readonly ApplicationDbContext _db;

        public LeaveRequestRepository(ApplicationDbContext db)
        {
            _db = db; 
        }

        public async Task<bool> Create(LeaveRequest entity)
        {
            await _db.LeaveRequests.AddAsync(entity);
            return await Save(); 
        }

        public async Task<bool> Delete(LeaveRequest entity)
        {
            _db.LeaveRequests.Remove(entity);
            return await Save(); 
        }

        public async Task< ICollection<LeaveRequest>> FindAll()
        {
            var LeaveRequests = await _db.LeaveRequests
                                .Include(q => q.RequestingEmployee)
                                .Include(q => q.ApprovedBy)
                                .Include(q => q.LeaveType)
                                .ToListAsync(); 
            return LeaveRequests; 
        }

        public async Task< LeaveRequest> FindById(int id)
        {
            var LeaveRequest = await _db.LeaveRequests
                                .Include(q => q.RequestingEmployee)
                                .Include(q => q.ApprovedBy)
                                .Include(q => q.LeaveType)
                                
                                .FirstOrDefaultAsync(q => q.Id == id);
            return LeaveRequest; 

        }

        public async Task< ICollection<LeaveRequest>> GetLeaveRequestsByEmployee(string employeeid)
        {
            var leaveRequests = await FindAll();

              return leaveRequests.Where(q => q.RequestingEmployeeId == employeeid)
                    .ToList();
            
        }

        public async Task<bool> IsPresent(int id)
        {
            var present = await _db.LeaveRequests.AnyAsync(q => q.Id == id);
            return present;
        }

        public async Task<bool> Save()
        {
            var changes = await  _db.SaveChangesAsync();
            return  changes > 0; 
        }

        public async Task<bool> Update(LeaveRequest entity)
        {
            _db.LeaveRequests.Update(entity);
            return await Save(); 
        }

        
    }
}
