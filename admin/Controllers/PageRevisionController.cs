using entity.ExternalModel;
using entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using service.Abstract;
using service.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace pageRevision.Controllers
{
    [Authorize(Policy = "Admin")]
    public class PageRevisionController : Controller
    {
        private readonly IUnitOfWork _uow;
        public PageRevisionController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public IActionResult Backup([FromQuery] int id)
        {
            PageRevision item = _uow.PageRevision.GetById(id);
            if (item != null)
            {
                return View(item);
            }
            return NotFound();
        }
        [HttpPost]
        public IActionResult Backup()
        {
            //Eğer böyle bir sayfa bulunuyorsa revizyonu geri al.
            PageRevision entity = _uow.PageRevision.GetById(Convert.ToInt32(HttpContext.Request.Query["id"]));
            if (entity != null)
            {
                Page page = _uow.Page.GetById(Convert.ToInt32(entity.PageId));
                if (page != null)
                {
                    //PageRevision'dan geri döndür.
                    _uow.PageRevision.Backup(entity, page, _uow);
                    return RedirectToAction("Manage", "Page", new { id = entity.PageId });
                }
                ViewBag.AlertCode = "not-found-page";
                return View(entity);
            }
            ViewBag.AlertCode = "not-found-revision";
            return View();
        }
        public async Task<IActionResult> List(int pageId, int pageNumber = 1, int pageSize = 200)
        {
            var list = _uow.PageRevision.GetAll().OrderByDescending(p => p.Date).AsQueryable();
            if (pageId != 0 && pageId.GetType() == typeof(int))
            {
                list = list.Where(p => p.PageId == pageId);
            }
            return View(await PaginatedList<PageRevision>.CreateAsync(list, pageNumber, pageSize));
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            PageRevision pageRevision = _uow.PageRevision.GetById(id);
            if (pageRevision != null)
            {
                _uow.PageRevision.Delete(pageRevision);
                return Json("success");
            }
            return Json("failed");
        }
    }
}