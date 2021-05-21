using DoanApp.Commons;
using DoanApp.Models;
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
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        // GET: CategoryController
        public ActionResult Index()
        {
            ViewBag.TitlePage = "Quản lý Chủ đề";
            return View();
        }
        public IActionResult IndexPartial(int? page, string name = null)
        {
            if (page == null) page = 1;
            var pageSize = 3;
            var pageNumber = page ?? 1;
            IPagedList<Category> list;
            if (name != null)
            {
                name = ConvertUnSigned.convertToUnSign(name).ToLower();
                list = _categoryService.GetAll().Result.
                    Where(x=>ConvertUnSigned.convertToUnSign(x.Name).ToLower().Contains(name)).ToPagedList(pageNumber, pageSize);
            }
            else
            {
                list = _categoryService.GetAll().Result.ToPagedList(pageNumber, pageSize);
            }
            return View(list);
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
            if (ModelState.IsValid)
            {
                var result = await _categoryService.CreateAsync(categoryRequest);
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CategoryRequest categoryRequest)
        {
            if (ModelState.IsValid)
            {
                var result = await _categoryService.UpdateAsync(categoryRequest);
                if (result > 0) return Redirect("Index");
            }
            return Redirect("Index");
        }

        // GET: CategoryController/Delete/5
        public ActionResult Delete(int id)
        {
            var result = _categoryService.DeleteAsync(id);
            if (result.Result > 0) return Content("Success");
            else return Content("Error");
        }


       
        public ContentResult checkName(string name)
        {
            name = name == null ? " " : name;
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
