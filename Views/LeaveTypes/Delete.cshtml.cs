using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using leave_management.Data;
using leave_management.Models;

namespace leave_management.Views.LeaveTypes
{
    public class DeleteModel : PageModel
    {
        private readonly leave_management.Data.ApplicationDbContext _context;

        public DeleteModel(leave_management.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LeaveTypeVM LeaveTypeVM { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            LeaveTypeVM = await _context.LeaveTypeVM.FirstOrDefaultAsync(m => m.Id == id);

            if (LeaveTypeVM == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            LeaveTypeVM = await _context.LeaveTypeVM.FindAsync(id);

            if (LeaveTypeVM != null)
            {
                _context.LeaveTypeVM.Remove(LeaveTypeVM);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
