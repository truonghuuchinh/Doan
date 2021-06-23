using DoanApp.Models;
using DoanApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Controllers
{

    public class DetailVideoController : Controller
    {
        private readonly IDetailVideoService _detailVideo;
        public DetailVideoController(IDetailVideoService detailVideo)
        {
            _detailVideo = detailVideo;
        }





        // POST: DetailVideoController/Create
        [HttpPost]
        public async Task<ActionResult> Create(string data)
        {
            var detailvideo = JsonConvert.DeserializeObject<DetailVideoRequest>(data);
            var result = await _detailVideo.Create(detailvideo);
            if (result > 0) return Content("Success");
            return Content("Error");
        }

        // POST: DetailVideoController/Delete/5
        public async Task<ActionResult> Delete(string data)
        {
            var detailvideo = JsonConvert.DeserializeObject<DetailVideoRequest>(data);
            var result = await _detailVideo.Delete(detailvideo);
            if (result > 0) return Content("Success");
            return Content("Error");
        }
        public string ListId(int id)
        {
            var listId = new List<int>();
            foreach (var item in _detailVideo.GetAll())
            {
                if (item.VideoId == id) listId.Add(item.PlayListId);
            }
            if(listId.Count>0) return JsonConvert.SerializeObject(listId);
            return "OK";
        }
    }
}
