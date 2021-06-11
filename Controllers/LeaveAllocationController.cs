using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using leave_management.Contracts;
using leave_management.Data;
using leave_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace leave_management.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveAllocationController : Controller
    {

        //private readonly ILeaveTypeRepository _leaverepo;
        //private readonly ILeaveAllocationRepository _leaveallocationrepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public LeaveAllocationController(
            ILeaveTypeRepository leaverepo,
            ILeaveAllocationRepository leaveallocationrepo,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UserManager<Employee> userManager
        )
        {
            //_leaverepo = leaverepo;
            //_leaveallocationrepo = leaveallocationrepo;
            _unitOfWork = unitOfWork; 
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task< IActionResult> Index()
        {
            //var leavetypes = await _leaverepo.FindAll();
            var leavetypes = await _unitOfWork.LeaveTypes.FindAll();
            var mappedLeaveTypes = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leavetypes.ToList());
            var model = new CreateLeaveAllocationVM
            {
                LeaveTypes = mappedLeaveTypes,
                NumberUpdated = 0
            };
            return View(model);
        }

        public async Task< ActionResult> SetLeave(int id)
        {
            //var leavetype = await _leaverepo.FindById(id);
            var leavetype = await _unitOfWork.LeaveTypes.Find(q => q.Id == id);
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            var period = DateTime.Now.Year; 
            foreach (var emp in employees)
            {
                // var checkAllocation =await _leaveallocationrepo.CheckAllocation(id, emp.Id);

                if (await _unitOfWork.LeaveAllocations.IsPresent(q => q.EmployeeId == emp.Id
                            && q.LeaveTypeId == id
                            && q.Period == period))
                    continue;
                var allocation = new LeaveAllocationVM
                {
                    DateCreated = DateTime.Now,
                    EmployeeId = emp.Id,
                    LeaveTypeId = id,
                    NumberOfDays = leavetype.DefaultDays,
                    Period = DateTime.Now.Year
                };
                var leaveallocation = _mapper.Map<LeaveAllocation>(allocation);
                //await _leaveallocationrepo.Create(leaveallocation);
                await _unitOfWork.LeaveAllocations.Create(leaveallocation);
                await _unitOfWork.Save(); 
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task< ActionResult> ListEmployees()
        {
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            var model = _mapper.Map<List<EmployeeVM>>(employees);
            return View(model); 
        }

        public async Task<ActionResult> Details(string id)
        {
            
            // following is pretty much same as above, but now in one line 
            var employee =  _mapper.Map<EmployeeVM>(await _userManager.FindByIdAsync(id));
            var period = DateTime.Now.Year;
            //var allocations =  _mapper.Map<List<LeaveAllocationVM>>(await _leaveallocationrepo.GetLeaveAllocationsByEmployee(id));

            var records = await _unitOfWork.LeaveAllocations.FindAll(
                                expression: q => q.EmployeeId == id && q.Period == period,
                                includes: new List<string> { "LeaveType"}) ;

            var allocations = _mapper.Map<List<LeaveAllocationVM>>(records);

            var model = new ViewAllocationsVM
            {
                Employee = employee,
                leaveAllocations = allocations 
            };
            return View(model); 
        }

        public async Task< ActionResult> Edit(int id) {

            // var leaveallocation = await _leaveallocationrepo.FindById(id);
            var leaveallocation = await _unitOfWork.LeaveAllocations.Find(q => q.Id == id,
                                        includes: new List<string> { "Employee", "LeaveType" }); 
            var model = _mapper.Map<EditLeaveAllocationVM>(leaveallocation); 
            return View(model); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Edit(EditLeaveAllocationVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model); 
                }
                // var record = await _leaveallocationrepo.FindById(model.Id);
                var record = await _unitOfWork.LeaveAllocations.Find(q => q.Id == model.Id); 
                record.NumberOfDays = model.NumberOfDays;
                // var isSuccess = await _leaveallocationrepo.Update(record);
                _unitOfWork.LeaveAllocations.Update(record);
                await _unitOfWork.Save(); 
                //if (!isSuccess)
                //{
                //    ModelState.AddModelError("", "Error while saving...");
                //    return View(model); 
                //}

                return RedirectToAction(nameof(Details), new {id = model.EmployeeId }); 
            }
            catch 
            {
                return View(model);
            }

            
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing); 
        }

    }
}
