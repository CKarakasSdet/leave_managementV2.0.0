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
using Microsoft.AspNetCore.Mvc;

namespace leave_management.Controllers
{

    [Authorize(Roles = "Administrator")]
    public class LeaveTypesController : Controller
    {
        private readonly ILeaveTypeRepository _repo;
        private readonly IUnitOfWork _unitOfWork; 
        private readonly IMapper _mapper;

        public LeaveTypesController(ILeaveTypeRepository repo, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repo = repo;
            _unitOfWork = unitOfWork; 
            _mapper = mapper; 
        }

        // GET: LeaveTypes
        public async Task<ActionResult> Index()
        {
            // var leavetypes = await _repo.FindAll(); <-- this is earlier version and unitofwork is below. 
            var leavetypes = await _unitOfWork.LeaveTypes.FindAll();
            var model = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leavetypes.ToList()); 
            return View(model);
        }

        // GET: LeaveTypes/Details/5
        public async Task< ActionResult> Details(int id)
        {
            // var IsPresent = await _repo.IsPresent(id); <-- this is earlier version and unitofwork is below. 
            var IsPresent = await _unitOfWork.LeaveTypes.IsPresent(q => q.Id == id);
            if (!IsPresent)
            {
                return NotFound(); 
            }

            // var leavetype = await _repo.FindById(id); <-- this is earlier version and unitofwork is below.
            var leavetype = await _unitOfWork.LeaveTypes.Find(q => q.Id == id);
            var model = _mapper.Map<LeaveTypeVM>(leavetype); 
            return View(model);
        }

        // GET: LeaveTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< ActionResult > Create(LeaveTypeVM model)
        {
            try
            {
                if (!ModelState.IsValid) {
                    return View(model); 
                }
                var leaveType = _mapper.Map<LeaveType>(model);
                leaveType.DateCreated = DateTime.Now;

                // var isSuccess = await _repo.Create(leaveType);
                await _unitOfWork.LeaveTypes.Create(leaveType);

                await _unitOfWork.Save(); 



                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong... ");
                return View(model);
            }
        }

        // GET: LeaveTypes/Edit/5
        public async Task< ActionResult>  Edit(int id)
        {
            // var IsPresent = await _repo.IsPresent(id); <-- this is earlier version and unitofwork is below. 
            var IsPresent = await _unitOfWork.LeaveTypes.IsPresent(q => q.Id == id);
            if (!IsPresent)
            {
                return NotFound();
            }

            // var leavetype = await _repo.FindById(id); <-- this is earlier version and unitofwork is below.
            var leavetype = await _unitOfWork.LeaveTypes.Find(q => q.Id == id);
            var model = _mapper.Map<LeaveTypeVM>(leavetype);
            return View(model);
        }

        // POST: LeaveTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< ActionResult> Edit(LeaveTypeVM model) 
        {
            try
            {
                // TODO: Add update logic here
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var leaveType = _mapper.Map<LeaveType>(model);
                //var isSuccess = await _repo.Update(leaveType);

                //if (!isSuccess)
                //{
                //    ModelState.AddModelError("", "Something went wrong..."); 
                //    return View(model);
                //}
                _unitOfWork.LeaveTypes.Update(leaveType);
                await _unitOfWork.Save(); 
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong...");
                return View(model);
            }
        }

        // GET: LeaveTypes/Delete/5
        public async Task< ActionResult> Delete(int id)
        {
            // var leavetype = await _repo.FindById(id);
            var leavetype = await _unitOfWork.LeaveTypes.Find(q => q.Id == id);
            if (leavetype == null)
            {
                return NotFound(); 
            }

            _unitOfWork.LeaveTypes.Delete(leavetype);
            await _unitOfWork.Save(); 
           
            return RedirectToAction(nameof(Index)); 
        }

        // POST: LeaveTypes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< ActionResult> Delete(int id, LeaveTypeVM model)
        {
            try
            {
                var leavetype = await _unitOfWork.LeaveTypes.Find(q => q.Id == id);
                if (leavetype == null)
                {
                    return NotFound();
                }

                _unitOfWork.LeaveTypes.Delete(leavetype);
                await _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
              
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