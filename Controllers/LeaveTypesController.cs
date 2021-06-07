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
        private readonly IMapper _mapper;

        public LeaveTypesController(ILeaveTypeRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper; 
        }

        // GET: LeaveTypes
        public async Task<ActionResult> Index()
        {
            var leavetypes = await _repo.FindAll();
            var model = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leavetypes.ToList()); 
            return View(model);
        }

        // GET: LeaveTypes/Details/5
        public async Task< ActionResult> Details(int id)
        {
            var IsPresent = await _repo.IsPresent(id); 
            if (!IsPresent)
            {
                return NotFound(); 
            }

            var leavetype = await _repo.FindById(id);
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
                var isSuccess = await _repo.Create(leaveType);
                if (!isSuccess) {
                    ModelState.AddModelError("", "Something went wrong... ");

                    return View(model); 
                }



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
            var IsPresent = await _repo.IsPresent(id);
            if (!IsPresent)
            {
                return NotFound(); 
            }

            var leavetype = await _repo.FindById(id);
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
                var isSuccess = await _repo.Update(leaveType);
                
                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Something went wrong..."); 
                    return View(model);
                }
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
            var leavetype = await _repo.FindById(id);
            if (leavetype == null)
            {
                return NotFound(); 
            }

            var isSuccess = await _repo.Delete(leavetype);

            if (!isSuccess)
            {
                return BadRequest(); 
            }
            return RedirectToAction(nameof(Index)); 

            // originally i used the following code but for convenience applying above code 
            //if (!_repo.IsPresent(id))
            //{
            //    return NotFound();
            //}

            //var leavetype = _repo.FindById(id);
            //var model = _mapper.Map<LeaveTypeVM>(leavetype);

            //return View(model);
        }

        // POST: LeaveTypes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< ActionResult> Delete(int id, LeaveTypeVM model)
        {
            try
            {
                // TODO: Add delete logic here

                var leavetype = await _repo.FindById(id);
                if (leavetype == null)
                {
                    return NotFound(); 
                }

                var isSuccess = await _repo.Delete(leavetype);

                if (!isSuccess)
                {
                    return View(model); 
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(model);
            }
        }
    }
}