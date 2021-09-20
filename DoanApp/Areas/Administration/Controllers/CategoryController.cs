using DoanApp.Commons;
using DoanApp.Models;
using DoanApp.ServiceApi;
using DoanApp.Services;
using DoanData.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace DoanApp.Areas.Administration.Controllers
{
    [Area("Administration")]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly ICategoryApiClient _categoryApi;
        private readonly IUserApiCient _userClient;
        public CategoryController(ICategoryService categoryService,ICategoryApiClient categoryApi,
            IUserApiCient userApi,IUserApiCient userApiCient)
        {
            _categoryService = categoryService;
            _categoryApi = categoryApi;
            _userClient = userApiCient;
        }
        // GET: CategoryController
        public async Task<IActionResult> Index(int? page)
        {
            ViewBag.Active = 2;
            if (page == null) page = 1;
            var pageSize = 6;
            var pageNumber = page ?? 1;
            var Token = HttpContext.Session.GetString("Token");
           if(_userClient.CheckToken(Token,User.Identity.Name)==null)
                return Redirect("/Administration/Home/Login");
           else
            {
                if (Token == null)
                    Token = await _userClient.CheckToken(Token, User.Identity.Name);
            }
            var  list = _categoryApi.GetAll(Token).Result.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize);
            return View(list);
        }
        public async Task<IActionResult> Index_Partial(int? page, string name = null)
        {
            ViewBag.Active = 2;
            if (page == null) page = 1;
            var pageSize = 6;
            var pageNumber = page ?? 1;
            var Token = HttpContext.Session.GetString("Token");
            if (_userClient.CheckToken(Token, User.Identity.Name) == null)
                return Redirect("/Administration/Home/Login");
            else
            {
                if (Token == null)
                    Token = await _userClient.CheckToken(Token, User.Identity.Name);
            }
            var list = _categoryApi.GetAll(Token, name).Result.OrderByDescending(x => x.Id).ToList();
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        // GET: CategoryController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CategoryRequest categoryRequest)
        {
            if (categoryRequest.Name!=null)
            {
                var Token = HttpContext.Session.GetString("Token");
                if (_userClient.CheckToken(Token, User.Identity.Name) == null)
                    return Redirect("/Administration/Home/Login");
                else
                {
                    if (Token == null)
                        Token = await _userClient.CheckToken(Token, User.Identity.Name);
                }
                var result = await _categoryApi.Create(Token, categoryRequest);
                if (result > 0) return Redirect("Index");
            }
            return View();
        }

        // GET: CategoryController/Edit/5
        public ActionResult Edit(int id)
        {
            var category = _categoryService.FinByIdAsync(id);
            return View(category.Result);
           
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(CategoryRequest categoryRequest)
        {
            if (ModelState.IsValid)
            {
                var Token = HttpContext.Session.GetString("Token");
                if (_userClient.CheckToken(Token, User.Identity.Name) == null)
                    return Redirect("/Administration/Home/Login");
                else
                {
                    if (Token == null)
                        Token = await _userClient.CheckToken(Token, User.Identity.Name);
                }
                var result = await _categoryApi.Update(Token, categoryRequest);
                if (result > 0) return Content("Success");
            }
            return Redirect("Index");
        }

        // GET: CategoryController/Delete/5
        public async  Task<ActionResult> Delete(int id)
        {
            var Token = HttpContext.Session.GetString("Token");
            if (_userClient.CheckToken(Token, User.Identity.Name) == null)
                return Redirect("/Administration/Home/Login");
            else
            {
                if (Token == null)
                    Token = await _userClient.CheckToken(Token, User.Identity.Name);
            }
            var result = await _categoryApi.Delete(Token,id);
            if (result > 0) return Content("Success");
            else return Content("Error");
        }


       
        public ContentResult checkName(string name)
        {
            if (name == null) return Content("");
            else
            {
                foreach (var item in _categoryService.GetAll().Result.ToList())
                {
                    if (item.Name.Contains(name))
                    {
                        return Content("Error");
                    }
                }
                return Content("Success");
            }
          
        }
    }
}
