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
    public class CommentController : Controller
    {
        private readonly IUnitOfWork _uow;
        public CommentController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<IActionResult> List(int pageNumber = 1, int pageSize = 50)
        {
            return View(await PaginatedList<Comment>.CreateAsync(_uow.Comment.GetAll().OrderBy(p => p.CreateDate), pageNumber, pageSize));
        }
        public JsonResult PublishStatus(bool enabled, int itemId)
        {
            var item = _uow.Comment.GetById(itemId);
            if (item != null)
            {
                item.Enabled = enabled;
                _uow.Comment.Update(item);
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
                return View(new Comment());
            }
            else
            {
                var comment = _uow.Comment.GetById(id);
                return View(comment);
            }
        }

        [HttpPost]
        public IActionResult Manage(Comment entity)
        {
            entity.Enabled = false;
            if (entity.Id == 0)
            {
                entity.CreateDate = DateTime.Now;

                _uow.Comment.Add(entity);
                return RedirectToAction("list");
            }
            else
            {
                _uow.Comment.Update(entity);

                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                string methodTitle = entity.Name;
                int methodId = entity.Id;

                _uow.AdminAction.AddAdminAction(controllerName, actionName, methodId, methodTitle, _uow);

                return RedirectToAction("list");
            }
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = _uow.Comment.GetById(Convert.ToInt32(id));
            if (item != null)
            {
                _uow.Comment.Delete(item);

                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                string methodTitle = item.Name;
                int methodId = id;
                _uow.AdminAction.AddAdminAction(controllerName, actionName, methodId, methodTitle, _uow);

                return Json("success");
            }
            return Json("failed");
        }
    }
}