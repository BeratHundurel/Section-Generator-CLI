using entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using service.Abstract;
using service.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace admin.Controllers
{
    [Authorize(Policy = "Developer")]
    public class LanguageController : Controller
    {
        private readonly IUnitOfWork _uow;
        public LanguageController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        [HttpPost]
        public JsonResult PublishStatus(bool enabled, int itemId)
        {
            var item = _uow.Language.GetById(itemId);
            if (item != null)
            {
                item.Enabled = enabled;
                _uow.Language.Update(item);
                if (enabled == true)
                {
                    return Json("start");
                }
                return Json("pause");
            }
            return Json("no");
        }
        [HttpPost]
        public IActionResult IsRoot(bool value, int id)
        {
            var item = _uow.Language.GetById(id);
            if (item != null)
            {
                item.IsRoot = value;
                _uow.Language.Update(item);
                if (value == true)
                {
                    return Json("start");
                }
                return Json("pause");
            }
            return Json("no");
        }
        public async Task<IActionResult> List(int pageNumber = 1)
        {
            IQueryable<Language> list = _uow.Language.GetAll();
            return View(await PaginatedList<Language>.CreateAsync(list, pageNumber, 200));
        }

        [HttpGet]
        public IActionResult Manage(int? id)
        {
            if (id != 0)
            {
                return View(_uow.Language.GetById(Convert.ToInt32(id)));
            }
            else
            {
                return View(new Language());
            }
        }

        [HttpPost]
        public IActionResult Manage(Language entity)
        {
            if (_uow.Language.CheckForUnique(entity))
            {
                if (entity.Id == 0)
                {
                    _uow.Language.Add(entity);
                    return RedirectToAction("List");
                }
                else
                {
                    _uow.Language.Update(entity);
                    return RedirectToAction("List");
                }
            }
            ViewBag.AlertMessage = "same-language";
            return View(entity);
        }
    }
}
