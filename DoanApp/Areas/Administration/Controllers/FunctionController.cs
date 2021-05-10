
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
            if (result.Result > 0) return Content("Success");
            else return Content("Error");

        }
        public string ListFunctionJson()
        {
            var list = _functionService.GetAll();
            /* var serialize= new JavaScriptSerializer();
             var json=serialize.Serialize(list.Result);*/
            return JsonConvert.SerializeObject(list.Result);
        }
    }
}
