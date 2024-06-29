using entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using service.Abstract;
using service.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace admin.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {
        private readonly IUnitOfWork _uow;
        public BlogController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        [Authorize(Policy = "Editor")]
        public async Task<IActionResult> List(int pageNumber = 1, int pageSize = 50)
        {
            return View(await PaginatedList<Blog>.CreateAsync(_uow.Blog.GetAllByDate(_uow.Cookie.GetAdminLangId), pageNumber, pageSize));
        }
        [Authorize(Policy = "CoAdmin")]
        public JsonResult PublishStatus(bool enabled, int itemId)
        {
            var item = _uow.Blog.GetById(itemId);
            if (item != null)
            {
                item.Enabled = enabled;
                _uow.Blog.Update(item);
                if (enabled == true)
                {
                    return Json("start");
                }
                return Json("pause");
            }
            return Json("no");
        }
        [Authorize(Policy = "Editor")]
        [HttpGet]
        public IActionResult Manage(int? id)
        {
            if (id != null)
            {
                var item = _uow.Blog.GetById(Convert.ToInt32(id));
                return View(item);
            }
            return View(new Blog());
        }
        [Authorize(Policy = "Editor")]
        [HttpPost]
        public IActionResult Manage(Blog entity)
        {
            entity.Enabled = false;
            if (entity.Id == 0)
            {
                entity.CreatedDate = DateTime.Now;
                entity.SitemapId = _uow.Sitemap.CreatePermalink(entity.Title, "Weekly", "Blog", Convert.ToDecimal(0.6), _uow.Cookie.GetAdminLangId);
                entity.LastModifiedDate = DateTime.Now;
                entity.LangId = _uow.Cookie.GetAdminLangId;
                _uow.Blog.Add(entity);
                return RedirectToAction("list");
            }
            else
            {
                #region sitemapUpdate
                if (_uow.Blog.TitleIsChanged(entity))
                {
                    var sitemap = _uow.Sitemap.GetSitemapBySitemapId(entity.SitemapId);
                    if (sitemap != null)
                    {
                        sitemap.Permalink = _uow.Sitemap.GenerateAndInsertPermalink(entity.Title);
                        _uow.Sitemap.Update(sitemap);
                    }
                }
                #endregion
                entity.LastModifiedDate = DateTime.Now;
                _uow.Blog.Update(entity);

                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                string methodTitle = entity.Title;
                int methodId = entity.Id;

                _uow.AdminAction.AddAdminAction(controllerName, actionName, methodId, methodTitle, _uow);

                return RedirectToAction("list");
            }
        }
        [Authorize(Policy = "CoAdmin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = _uow.Blog.GetById(Convert.ToInt32(id));
            int sitemapId = item.SitemapId;
            if (item != null)
            {
                var media = _uow.Media.GetById(Convert.ToInt32(item.MediaId));
                _uow.Media.DeleteImage(media);

                _uow.Blog.Delete(item);

                var sitemap = _uow.Sitemap.GetById(sitemapId);
                _uow.Sitemap.Delete(sitemap);

                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                string methodTitle = item.Title;
                int methodId = id;
                _uow.AdminAction.AddAdminAction(controllerName, actionName, methodId, methodTitle, _uow);

                return Json("success");
            }
            return Json("failed");
        }
    }
}