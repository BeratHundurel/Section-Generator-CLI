using entity.ExternalModel;
using entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using service.Abstract;
using service.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace admin.Controllers
{
    [Authorize(Policy = "Admin")]
    public class FaqController : Controller
    {
        private readonly IUnitOfWork _uow;
        public FaqController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<IActionResult> List(int pageNumber = 1, int pageSize = 50)
        {
            return View(await PaginatedList<Faq>.CreateAsync(_uow.Faq.GetAll().OrderBy(p => p.CreateDate), pageNumber, pageSize));
        }
        public JsonResult PublishStatus(bool enabled, int itemId)
        {
            var item = _uow.Faq.GetById(itemId);
            if (item != null)
            {
                item.Enabled = enabled;
                _uow.Faq.Update(item);
                if (enabled == true)
                {
                    return Json("start");
                }
                return Json("pause");
            }
            return Json("no");
        }
        public IActionResult Manage(int id)
        {
            if (id == 0)
            {
                return View(new Faq());
            }
            else
            {
                var faq = _uow.Faq.GetById(id);
                return View(faq);
            }
        }

        [HttpPost]
        public IActionResult Manage(Faq entity)
        {
            entity.Enabled = false;
            if (entity.Id == 0)
            {
                entity.CreateDate = DateTime.Now;

                _uow.Faq.Add(entity);
                return RedirectToAction("list");
            }
            else
            {
                _uow.Faq.Update(entity);

                //string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                //string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                //string methodTitle = entity.Question;
                //int methodId = entity.Id;

                //_uow.AdminAction.AddAdminAction(controllerName, actionName, methodId, methodTitle, _uow);

                return RedirectToAction("list");
            }
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = _uow.Faq.GetById(Convert.ToInt32(id));
            if (item != null)
            {
                _uow.Faq.Delete(item);

                //string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                //string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                //string methodTitle = item.Question;
                //int methodId = id;
                //_uow.AdminAction.AddAdminAction(controllerName, actionName, methodId, methodTitle, _uow);

                return Json("success");
            }
            return Json("failed");
        }
    }
}