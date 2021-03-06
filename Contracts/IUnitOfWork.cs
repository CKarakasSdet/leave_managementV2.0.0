using System;
using System.Threading.Tasks;
using leave_management.Data;

namespace leave_management.Contracts
{
    public interface IUnitOfWork : IDisposable 
    {

        IGenericRepository<LeaveType> LeaveTypes { get; }

        IGenericRepository<LeaveRequest> LeaveRequests { get; }

        IGenericRepository<LeaveAllocation> LeaveAllocations { get; }

        Task Save(); 
    }
}
