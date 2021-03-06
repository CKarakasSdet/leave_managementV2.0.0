using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leave_management.Data;
using leave_management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace leave_management.Views.LeaveAllocation
{
    public class ListEmployeesModel : PageModel
    {
        private readonly leave_management.Data.ApplicationDbContext _context;

        public ListEmployeesModel(leave_management.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<LeaveType> LeaveType { get; set; }

        public async Task OnGetAsync()
        {
            LeaveType = await _context.LeaveTypes.ToListAsync();
        }
    }
}
