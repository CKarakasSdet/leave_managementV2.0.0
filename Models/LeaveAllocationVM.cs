using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace leave_management.Models
{
    public class LeaveAllocationVM
    {
        public LeaveAllocationVM()
        {
        }

       
        public int Id { get; set; }
       
        public int NumberOfDays { get; set; }

        public DateTime DateCreated { get; set; }

        public int Period { get; set; }

        public EmployeeVM Employee { get; set; }

        public string EmployeeId { get; set; }

        public LeaveTypeVM LeaveType { get; set; }

        public int LeaveTypeId { get; set; }


    }

    public class CreateLeaveAllocationVM
    {
        public int NumberUpdated { get; set; }

        public List<LeaveTypeVM> LeaveTypes { get; set; }
    }

    public class EditLeaveAllocationVM
    {
        public int Id { get; set; }

        public EmployeeVM Employee { get; set; }

        public string EmployeeId { get; set; }

        public int NumberOfDays { get; set; }

        public LeaveTypeVM LeaveType { get; set; }
    }

    public class ViewAllocationsVM {

        public EmployeeVM Employee { get; set; }

        public string EmployeeId { get; set; }

        public List<LeaveAllocationVM> leaveAllocations { get; set; }
    }
}
