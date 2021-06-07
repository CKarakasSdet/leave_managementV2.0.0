using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leave_management.Data;
using leave_management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace leave_management.Views.LeaveAllocations
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<LeaveType> LeaveTypes { get; set; }

        public async Task OnGetAsync()
        {
            LeaveTypes = await _context.LeaveTypes.ToListAsync();
        }
    }
}
