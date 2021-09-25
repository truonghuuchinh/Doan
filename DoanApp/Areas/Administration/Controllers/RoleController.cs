using DoanApp.Models;
using DoanData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace DoanApp.Areas.Administration.Controllers
{
   [Area("Administration")]
    public class RoleController : BaseController
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        public RoleController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index(int? page)
        {
            ViewBag.Active = 3;
            if (page == null) page = 1;
            var pageSize = 6;
            var pageNumber = page ?? 1;
            var listUser = new List<string>();
            var listRole = _roleManager.Roles.ToPagedList(pageNumber, pageSize);
            foreach (var item in listRole)
            {
                string user = item.Name+"|";
                var list = _userManager.GetUsersInRoleAsync(item.Name).Result;
                if (list.Count > 0)
                {
                    foreach (var user1 in list)
                    {
                        user += user1.FirtsName + " " + user1.LastName + ", ";
                    }
                    listUser.Add(user);
                }
            }
            ViewBag.ListUser = listUser;
            return View(listRole);
        }
        public IActionResult Index_Partial(int? page, string name = null)
        {
            ViewBag.Active = 3;
            if (page == null) page = 1;
            var pageSize = 6;
            var pageNumber = page ?? 1;
            var listUser = new List<string>();
            var listRole = _roleManager.Roles.ToList();
            if (name != null)
            {
                name = name.ToLower();
                listRole = listRole.Where(x => x.Name.ToLower().Contains(name)).ToList();
            }
            foreach (var item in listRole)
            {
                string user = null;
                var list = _userManager.GetUsersInRoleAsync(item.Name).Result;
                if (list.Count > 0)
                {
                    foreach (var user1 in list)
                    {
                        user += user1.FirtsName + " " + user1.LastName + ", ";
                    }
                    listUser.Add(user);
                }
            }
       
            ViewBag.ListUser = listUser;
            return View(listRole.ToPagedList(pageNumber, pageSize));
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([Required] string name)
        {
            if (ModelState.IsValid)
            {
               
                var roles = new AppRole();
                roles.Name = name;
                var result = await _roleManager.CreateAsync(roles);
                if (result.Succeeded)
                    return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Tạo không thành công");
            return View(name);
        }
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                    return Content("Success");
                else
                {
                    ModelState.AddModelError("", "Xóa không thành công");
                    return View("Index", _roleManager.Roles);
                }
            }
            else
                ModelState.AddModelError("", "Không tìm thấy");
            return View("Index", _roleManager.Roles);
        }
        public async Task<IActionResult> Update(string Id)
        {
            var role = await _roleManager.FindByIdAsync(Id);
            var members = new List<AppUser>();
           var nonMembers = new List<AppUser>();
            foreach (var user in _userManager.Users)
            {
                var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                list.Add(user);
            }
            return View(new RoleEdit
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }
        [HttpPost]
        public async Task<IActionResult> Update(RoleModification model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.AddIds ?? new string[] { })
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await _userManager.AddToRoleAsync(user, model.RoleName);
                    }
                }
                foreach (string userId in model.DeleteIds ?? new string[] { })
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await _userManager.RemoveFromRoleAsync(user, model.RoleName); 
                    }
                }
            }

            if (ModelState.IsValid)
                return RedirectToAction(nameof(Index));
            else
                return await Update(model.RoleId);
        }
        public string CheckName(string name)
        {
            if (name == null)
                return "NoSearch";
            foreach (var item in _roleManager.Roles)
            {
                if (item.Name.ToLower() == name.ToLower()) return "Error";
            }
            return "Success";
        }
    }
}

