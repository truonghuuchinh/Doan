
using DoanApp.Models;
using DoanApp.Services;
using DoanData.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Areas.Administration.Controllers
{
    [Area("Administration")]
    public class FunctionController : Controller
    {
        // GET: FunctionController
        private readonly IFunctionService _functionService;
        public FunctionController(IFunctionService functionService)
        {
            _functionService = functionService;
        }
        public ActionResult Index()
        {
            ViewBag.TitlePage = "Quản lý Chức năng";
            return View( _functionService.GetAll().Result);
        }

        
        // GET: FunctionController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FunctionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FunctionRequest functionRequest)
        {
            if (ModelState.IsValid)
            {
                var result =await _functionService.CreateAsync(functionRequest);
                if (result > 0) return Redirect("Index");
            }
            return View();
        }

        // GET: FunctionController/Edit/5
        public ActionResult Edit(int id)
        {
            var fucntion = _functionService.FinByIdAsync(id);
            return View(fucntion.Result);
        }

        // POST: FunctionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(FunctionRequest functionRequest)
        {
            if (ModelState.IsValid)
            {
               var result= await _functionService.UpdateAsync(functionRequest);
                if (result > 0) return Redirect("Index");
            }
            return Redirect("Index");
        }
       
        public ActionResult Delete(int id)
        {
            var result = _functionService.DeleteAsync(id);
            if (result.Result > 0) return Content(ListFunctionJson());
            else return Content("Error");

        }
        public string ListFunctionJson(string name = null)
        {
            List<Function> list;
            if (name != null)
            {
                list = _functionService.GetAll().Result.Where(x=>x.Name.Contains(name)).ToList();
            }
            else list = _functionService.GetAll().Result;
           return JsonConvert.SerializeObject(list);
        }
        public ContentResult checkName(string name)
        {
            name = name == null ? " " : name;
            foreach (var item in _functionService.GetAll().Result.ToList())
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
