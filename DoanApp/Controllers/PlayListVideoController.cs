using DoanApp.Models;
using DoanApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Controllers
{
    public class PlayListVideoController : Controller
    {
        private readonly IPlayListService _playlistService;
        public PlayListVideoController(IPlayListService playListService)
        {
            _playlistService = playListService;
        }
        // GET: PlayListVideoController
        public ActionResult Index()
        {
            return View();
        }

      
        // POST: PlayListVideoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PlaylistRequest request)
        {
            if (request != null)
            {
                var result = await _playlistService.Create(request);
                if (result > 0) return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: PlayListVideoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PlayListVideoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PlayListVideoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PlayListVideoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
