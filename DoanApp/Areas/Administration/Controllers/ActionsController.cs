using DoanApp.Models;
using DoanApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Areas.Administration.Controllers
{
    [Area("Administration")]
    public class ActionsController : Controller
    {
        private readonly IActionService _actionService;
        private readonly IFunctionService _functionService;
        public ActionsController(IActionService actionService,IFunctionService functionService)
        {
            _actionService = actionService;
            _functionService = functionService;
        }
        // GET: ActionsController
        public  ActionResult Index()
        {
            ViewBag.TitlePage = "Quản lý Hành động";
            return View( _actionService.GetAll());
        }

        // GET: ActionsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ActionsController/Create
        public async Task<ActionResult> Create()
        {
            ViewData["Function"] = new SelectList(await _functionService.GetAll(),"Id","Name");
            return View();
        }

        // POST: ActionsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ActionRequest actionRequest)
        {
            if (ModelState.IsValid)
            {
                var result = _actionService.CreateAsync(actionRequest);
                if (result.Result > 0) return Redirect("Index");
            }
            return BadRequest();
        }

        // GET: ActionsController/Edit/5
        public ActionResult Edit(int id)
        {
            var action = _actionService.FinByIdAsync(id);
            ViewBag.Functions = _functionService.GetAll().Result;
            return View(action.Result);
        }

        // POST: ActionsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ActionRequest request)
        {
            var result = _actionService.UpdateAsync(request);
            if (result.Result > 0) return Redirect("Index");
            return BadRequest();

        }
        // POST: ActionsController/Delete/5

        public ActionResult Delete(int id)
        {
            var result = _actionService.DeleteAsync(id);
            if (result.Result > 0) return Content(ListActionResult());
             return Content(null);
        }
        public string ListActionResult(string name=null)
        {
            List<DoanData.Models.Action> list;
            if (name != null)
                list = _actionService.GetAll().Where(x => x.Name.Contains(name)).ToList();
            else
            {
                list = _actionService.GetAll();
            }
           
            return JsonConvert.SerializeObject(list);
        }
        public ContentResult checkName(int id,string name)
        {
            name = name == null ? " " : name;
            foreach (var item in _actionService.GetAll().Where(x=>x.FunctionsId==id))
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
