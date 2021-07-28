using DoanApp.Commons;
using DoanApp.Models;
using DoanApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategorysController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategorysController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        //localhost:port/category
        [HttpGet]
        public async Task<IActionResult> GetAll(string nameSearch=null)
        {
            var getall = await _categoryService.GetAllApi();
            if(nameSearch!=null)
            {
                nameSearch = ConvertUnSigned.convertToUnSign(nameSearch).ToLower();
                getall = getall.Where(x => ConvertUnSigned.convertToUnSign(x.Name).ToLower().Contains(nameSearch)).ToList();

            }
            return Ok(JsonConvert.SerializeObject(getall));
        }

        //localhost:port/category/1
        [HttpGet("findCategory/{categoryid}")]
        public async Task<IActionResult> GetFindById(int categoryid)
        {
            var category =await _categoryService.FinByIdAsync(categoryid);
            if (category!=null) return Ok(category);
            return BadRequest("Cannot category in database");
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CategoryRequest request)
        {
            var result =await  _categoryService.CreateAsync(request);
            if (result > 0)
            {
                var idCategoty = (await _categoryService.GetAll()).OrderByDescending(x => x.Id).Take(1).ToArray()[0].Id;
                var category =await _categoryService.FinByIdAsync(idCategoty);
                return CreatedAtAction(nameof(GetFindById),new { id=idCategoty},category);
            }
            return BadRequest("Created No Success");

        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody]CategoryRequest request)
        {
            var result = await _categoryService.UpdateAsync(request);
            if (result > 0)
            {
                return Ok(result);
            }
            return BadRequest("Update No Success");

        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int categoryid)
        {
            var result = await _categoryService.DeleteAsync(categoryid);
            if (result > 0)
            {
                return Ok(result);
            }
            return BadRequest("Delete No Success");

        }

    }
}
