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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace leave_management.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {

        private readonly ILeaveRequestRepository _leaveRequestRepo;
        private readonly ILeaveAllocationRepository _leaveAllocationRepo; 
        private readonly ILeaveTypeRepository _leaveTypeRepo;
        private readonly UserManager<Employee> _userManager;
        private readonly IMapper _mapper;

        public LeaveRequestController
            (
              ILeaveRequestRepository leaveRequestRepo,
              ILeaveAllocationRepository leaveAllocationRepo,
              ILeaveTypeRepository leaveTypeRepo,
              UserManager<Employee> userManager,
              IMapper mapper
            )
        {
            _leaveRequestRepo = leaveRequestRepo;
            _leaveAllocationRepo = leaveAllocationRepo; 
            _leaveTypeRepo = leaveTypeRepo; 
            _userManager = userManager;
            _mapper = mapper; 
        }

        [Authorize(Roles = "Administrator")]
        public async Task< IActionResult> Index()
        {
            var leaveRequests = await _leaveRequestRepo.FindAll();
            var leaveRequestsModel = _mapper.Map<List<LeaveRequestVM>>(leaveRequests);
            var model = new AdminLeaveRequestViewVM
            {
                TotalRequests = leaveRequestsModel.Count,
                ApprovedRequests = leaveRequestsModel.Count(q => q.Approved == true),
                PendingRequests = leaveRequestsModel.Count(q => q.Approved == null),
                RejectedRequests = leaveRequestsModel.Count(q => q.Approved == false),
                LeaveRequests = leaveRequestsModel
            };

            return View(model);
        }

        // CREATE LEAVE REQUEST FOR EMPLOYEE 

        public async Task< ActionResult> Create() {

            // "to load it up with model"
            var leaveTypes = await _leaveTypeRepo.FindAll();
            var leaveTypeItems = leaveTypes.Select(q => new SelectListItem {
                Text = q.Name,
                Value = q.Id.ToString()
            });

            var model = new CreateLeaveRequestVM
            {
                LeaveTypes = leaveTypeItems 
            };


            return View(model); 
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< ActionResult>  Create(CreateLeaveRequestVM model)

        {
            try
            {
                var startDate = Convert.ToDateTime(model.StartDate);
                var endDate = Convert.ToDateTime(model.EndDate); 
                var leaveTypes = await _leaveTypeRepo.FindAll();
                var leaveTypeItems = leaveTypes.Select(q => new SelectListItem
                {
                    Text = q.Name,
                    Value = q.Id.ToString()
                });

                model.LeaveTypes = leaveTypeItems;

                if (!ModelState.IsValid)
                {
                    return View(model); 
                }

                if (DateTime.Compare(startDate, endDate) > 0)
                {
                    ModelState.AddModelError("", "Start date cannot be later than end date"); 
                    return View(model); 
                }
                
                var employee = await  _userManager.GetUserAsync(User); // retrieving current user.

                var allocation = await _leaveAllocationRepo.GetLeaveAllocationByEmployeeAndType(employee.Id, model.LeaveTypeId);

                if (allocation == null)
                {
                    ModelState.AddModelError("", "allocation is null error. employee.Id: " + employee.Id + " model.LeaveTypeId: "+model.LeaveTypeId);
                    return View(model);
                }

                int daysRequested = (int)(endDate - startDate).TotalDays;

                if (daysRequested > allocation.NumberOfDays)
                {
                    ModelState.AddModelError("", "You do not have enough days for this request.");
                    return View(model);
                }

                var leaveRequestModel = new LeaveRequestVM
                {
                    RequestingEmployeeId = employee.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    Approved = null,
                    DateRequested = DateTime.Now,
                    DateActioned = DateTime.Now,
                    LeaveTypeId = model.LeaveTypeId,
                    RequestComment = model.RequestComment,
                    FeedbackForRequestComment = model.FeedbackForRequestComment,
                    Cancelled = false 
                };

                var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestModel);
                var isSuccess = await _leaveRequestRepo.Create(leaveRequest);

                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Error with submitting your record.");
                    return View(model);
                }

                return RedirectToAction( "MyLeave"); 
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.ToString()); 
                return View(model); 
            }
        }

        public async Task< IActionResult> Details(int id) {

            var leaveRequest = await _leaveRequestRepo.FindById(id);
            var model = _mapper.Map<LeaveRequestVM>(leaveRequest); 
            return View(model); 
        }


        
      

        public async Task< IActionResult> ApproveRequest(int id )
        {
            try
            {
                

                var leaveRequest = await _leaveRequestRepo.FindById(id);
                var allocation = await _leaveAllocationRepo.GetLeaveAllocationByEmployeeAndType(leaveRequest.RequestingEmployeeId, leaveRequest.LeaveTypeId);
                
                int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;

                allocation.NumberOfDays -= daysRequested;  

                leaveRequest.Approved = true;

                leaveRequest.ApprovedById = _userManager.GetUserAsync(User).Result.Id;
                leaveRequest.DateActioned = DateTime.Now;
                
                 
                
                

                await _leaveRequestRepo.Update(leaveRequest);
                await _leaveAllocationRepo.Update(allocation);

                return RedirectToAction(nameof(Index));


            }
            catch (Exception ex) 
            {
                ModelState.AddModelError("", ex.ToString()); 
                return RedirectToAction(nameof(Index));
            }
             
            
        }

        public async Task< IActionResult> RejectRequest(int id)
        {

            try
            {
                var user = await _userManager.GetUserAsync(User); 
                var leaveRequest = await _leaveRequestRepo.FindById(id);

                leaveRequest.Approved = false;
                leaveRequest.ApprovedById =  user.Id;
                leaveRequest.DateActioned = DateTime.Now;

                await _leaveRequestRepo.Update(leaveRequest);

                return RedirectToAction(nameof(Index));

            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error occurred with this query. Please try again.");
                return RedirectToAction(nameof(Index));
            }

        }

        public async Task< IActionResult> CancelRequest(int id)
        {
            // TODO: will complete cancel request process 
            var leaveRequest = await _leaveRequestRepo.FindById(id);
            leaveRequest.Cancelled = true;
            await _leaveRequestRepo.Update(leaveRequest);
            return RedirectToAction("MyLeave"); 
        }

        public async Task< IActionResult> MyLeave()
        {
            var employee = await _userManager.GetUserAsync(User);
            var employeeid = employee.Id;
            var employeeAllocations = await _leaveAllocationRepo.GetLeaveAllocationsByEmployee(employeeid);
            var employeeRequests = await _leaveRequestRepo.GetLeaveRequestsByEmployee(employeeid);

            var employeeAllocationsModel = _mapper.Map<List<LeaveAllocationVM>>(employeeAllocations);
            var employeeRequestsModel = _mapper.Map<List<LeaveRequestVM>>(employeeRequests); 

            var model = new EmployeeLeaveRequestViewVM
            {

                LeaveAllocations = employeeAllocationsModel,
                LeaveRequests = employeeRequestsModel,
               
               
            }; 

            return View(model); 
        }


    }
}
