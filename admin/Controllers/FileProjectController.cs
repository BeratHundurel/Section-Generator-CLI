using entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class FileProjectController : Controller
    {
        private readonly IUnitOfWork _uow;
        public FileProjectController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        [HttpPost]
        public JsonResult PublishStatus(bool enabled, int itemId)
        {
            var item = _uow.FileProject.GetById(itemId);
            if (item != null)
            {
                item.Enabled = enabled;
                _uow.FileProject.Update(item);
                if (enabled == true)
                {
                    return Json("start");
                }
                return Json("pause");
            }
            return Json("no");
        }
        public async Task<IActionResult> List(int pageNumber = 1)
        {
            IQueryable<FileProject> list = _uow.FileProject.GetAll();
            return View(await PaginatedList<FileProject>.CreateAsync(list, pageNumber, 200));
        }
        [HttpGet]
        public IActionResult Manage(int? id)
        {
            if (id != 0 && id != null)
            {
                return View(_uow.FileProject.GetById(Convert.ToInt32(id)));
            }
            else
            {
                return View(new FileProject { CreatedDate = DateTime.Now, Id = 0 });
            }
        }
        [HttpPost]
        public IActionResult Manage(FileProject entity)
        {
            if (ModelState.IsValid)
            {
                if (entity.Id == 0)
                {
                    entity.CreatedDate = DateTime.Now;
                    _uow.FileProject.Add(entity);
                    return RedirectToAction("List");
                }
                else
                {
                    entity.Enabled = false;
                    _uow.FileProject.Update(entity);

                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    string methodTitle = entity.Title;
                    int methodId = entity.Id;

                    _uow.AdminAction.AddAdminAction(controllerName, actionName, methodId, methodTitle, _uow);

                    return RedirectToAction("List");
                }
            }
            else
            {
                ViewBag.AlertMessage = "Lütfen gerekli alanları doldurun.";
                return View(entity);
            }
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = _uow.FileProject.GetById(Convert.ToInt32(id));
            if (item != null)
            {
                #region deleteFolder

                #endregion
                _uow.FileProject.Delete(item);

                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                string methodTitle = item.Title;
                int methodId = item.Id;

                _uow.AdminAction.AddAdminAction(controllerName, actionName, methodId, methodTitle, _uow);
                return Json("success");
            }
            return Json("failed");
        }
        [HttpGet]
        public JsonResult GetFileMainLinkByWebsite()
        {
            //string projectByWebsite = _uow.WebSite.GetWebsite(_uow.Cookie.GetAdminWebSiteId).FileProject.ProjectName;
            //string domain = _uow.FileProject.GetByProjectName(projectByWebsite, null).FullDomain;
            return Json(null);
        }
    }
}
