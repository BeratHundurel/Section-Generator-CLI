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
    public class CategoryController : Controller
    {
        public readonly IUnitOfWork _uow;
        public CategoryController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<IActionResult> List(int pageNumber = 1, int pageSize = 50)
        {
            var categories = await _uow.Category.GetAll(_uow.Cookie.GetAdminLangId).ToListAsync();
            var paginatedList = new PaginatedList<Category>(categories, categories.Count, pageNumber, pageSize);
            return View(paginatedList);

            //return View(await PaginatedList<Category>.CreateAsync(_uow.Category.GetAll(_uow.Cookie.GetAdminLangId).AsQueryable(), pageNumber, pageSize));

        }
        public JsonResult PublishStatus(bool enabled, int itemId)
        {
            var item = _uow.Category.GetById(itemId);
            if (item != null)
            {
                item.Enabled = enabled;
                _uow.Category.Update(item);
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
                return View(new Category());
            }
            else
            {
                var product = _uow.Category.GetById(id);
                return View(product);
            }
        }
        [HttpPost]
        public IActionResult Manage(Category entity)
        {
            var adminLangId = _uow.Cookie.GetAdminLangId;
            entity.LangId = adminLangId;
            entity.Enabled = false;
            if (entity.Id == 0)
            {
                entity.SitemapId = _uow.Sitemap.CreatePermalink(entity.Name, "Weekly", "Category", Convert.ToDecimal(0.6), _uow.Cookie.GetAdminLangId);

                _uow.Category.Add(entity);
                return RedirectToAction("list");
            }
            else
            {
                #region sitemapUpdate
                if (_uow.Category.TitleIsChanged(entity))
                {
                    var sitemap = _uow.Sitemap.GetSitemapBySitemapId(Convert.ToInt32(entity.SitemapId));
                    if (sitemap != null)
                    {
                        sitemap.Permalink = _uow.Sitemap.GenerateAndInsertPermalink(entity.Name);
                        _uow.Sitemap.Update(sitemap);
                    }
                }
                #endregion
                _uow.Category.Update(entity);

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
            var item = _uow.Category.GetById(Convert.ToInt32(id));
            var subCategories = _uow.SubCategory.GetBySubCategoryId(Convert.ToInt32(id));
            int sitemapId = Convert.ToInt32(item.SitemapId);
            var products = _uow.Product.GetByCategoryId(Convert.ToInt32(id));
            if (item != null)
            {


                foreach (var product in products)
                {
                    _uow.CategoryProductRelation.AllRemoveCategoryRelation(product.Id);
                }

                if (subCategories != null)
                {
                    foreach (var product in products)
                    {
                        _uow.SubCategoryProductRelation.AllRemoveSubCategoryRelation(product.Id);
                    }
                    foreach (var subcategory in subCategories)
                    {
                        _uow.SubCategory.Delete(subcategory);
                    }


                }
                foreach (var product in products)
                {
                    _uow.Product.Delete(product);
                }

                _uow.Category.Delete(item);
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

    }
}