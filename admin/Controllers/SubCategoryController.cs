using admin.ViewModel;
using entity.ExternalModel;
using entity.Models;
using entity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using service.Abstract;
using service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin.Controllers
{
    [Authorize(Policy = "CoAdmin")]
    public class SubCategoryController : Controller
    {
        private readonly IUnitOfWork _uow;
        public SubCategoryController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<IActionResult> List(int pageNumber = 1, int pageSize = 50)
        {
            var subcategories = await _uow.SubCategory.GetAll(_uow.Cookie.GetAdminLangId).ToListAsync();
            var paginatedList = new PaginatedList<SubCategory>(subcategories, subcategories.Count, pageNumber, pageSize);
            return View(paginatedList);

            //return View(await PaginatedList<SubCategory>.CreateAsync(_uow.SubCategory.GetAll(), pageNumber, pageSize));
        }
        public JsonResult PublishStatus(bool enabled, int itemId)
        {
            var item = _uow.SubCategory.GetById(itemId);
            if (item != null)
            {
                item.Enabled = enabled;
                _uow.SubCategory.Update(item);
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
                return View(new SubCategory());
            }
            else
            {
                var product = _uow.SubCategory.GetById(id);
                return View(product);
            }
        }
        [HttpPost]
        public IActionResult Manage(SubCategory entity)
        {
            var adminLangId = _uow.Cookie.GetAdminLangId;
            entity.LangId = adminLangId;
            entity.Enabled = false;
            var category = _uow.Category.GetById(entity.CategoryId);
            var combinedName = entity.Name + " " + category.Name;
            if (entity.Id == 0)
            {
                entity.SitemapId = _uow.Sitemap.CreatePermalink(combinedName, "Weekly", "Category", Convert.ToDecimal(0.6), _uow.Cookie.GetAdminLangId);
                _uow.SubCategory.Add(entity);
                return RedirectToAction("list");
            }
            else
            {
                #region sitemapUpdate
                if (_uow.SubCategory.TitleIsChanged(entity))
                {
                    var sitemap = _uow.Sitemap.GetSitemapBySitemapId(Convert.ToInt32(entity.SitemapId));
                    if (sitemap != null)
                    {
                        sitemap.Permalink = _uow.Sitemap.GenerateAndInsertPermalink(combinedName);
                        _uow.Sitemap.Update(sitemap);
                    }
                }
                #endregion
                _uow.SubCategory.Update(entity);

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
            var item = _uow.SubCategory.GetById(Convert.ToInt32(id));
            int sitemapId = Convert.ToInt32(item.SitemapId);
            if (item != null)
            {
                _uow.SubCategory.Delete(item);

                var media = _uow.Media.GetById(Convert.ToInt32(item.MediaId));
                _uow.Media.DeleteImage(media);
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                string methodTitle = item.Name;
                int methodId = id;
                var sitemap = _uow.Sitemap.GetById(sitemapId);
                _uow.Sitemap.Delete(sitemap);
                _uow.AdminAction.AddAdminAction(controllerName, actionName, methodId, methodTitle, _uow);

                return Json("success");
            }
            return Json("failed");
        }
        public IActionResult ListByCat(int catid)
        {
            return View();
        }
    }
}