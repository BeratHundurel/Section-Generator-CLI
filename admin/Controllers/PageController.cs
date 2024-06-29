
using admin.ViewModel;
using entity.Context;
using entity.ExternalModel;
using entity.Models;
using entity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
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
    [Authorize]
    public class PageController : Controller
    {
        private readonly IUnitOfWork _uow;
        public PageController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        [Authorize(Policy = "CoAdmin")]
        public JsonResult PublishStatus(bool enabled, int itemId)
        {
            var item = _uow.Page.GetById(itemId);
            if (item != null)
            {
                item.Enabled = enabled;
                _uow.Page.Update(item);

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
        public async Task<IActionResult> List(int pageNumber = 1, int pageSize = 50)
        {
            var items = _uow.Page.GetAllByLang(_uow.Cookie.GetAdminLangId).Include(p => p.PageType).AsQueryable();
            //Page türü dışındaki sayfaları sadece geliştiriler görmeli ve düzenlemeli.
            if (!_uow.Admin.IsAccessByRole("developer"))
            {
                items = items.Where(p => p.PageTypeId == _uow.PageType.GetNormalPageTypeId);
            }
            return View(await PaginatedList<Page>.CreateAsync(items, pageNumber, pageSize));
        }
        [Authorize(Policy = "CoAdmin")]
        public IActionResult SectionManager(string sectionName)
        {
            var sivm = new SectionItemViewModel { SectionName = sectionName, ViewName = sectionName, SVM = new SectionViewModel(), Slide = new Slide() };
            return ViewComponent("SectionItem", sivm);
        }

        [Authorize(Policy = "Editor")]
        [HttpGet]
        public IActionResult Manage(int? id = 0)
        {
            if (id != 0)
            {
                PageViewModel item = new PageViewModel
                {
                    Page = _uow.Page.GetById(id.Value),
                    Sections = _uow.Page.GetSectionsByPageId(id.Value),
                    Header = _uow.Page.GetHeaderByPageId(id.Value),
                    Slides = _uow.Page.GetSlidesByPageId(id.Value)
                };
                return View(item);
            }
            else
            {
                PageViewModel item = new PageViewModel
                {
                    Page = new Page(),
                    Sections = new List<SectionViewModel>(),
                    Header = new Header(),
                    Slides = new List<Slide>()
                };
                return View(item);
            }
        }

        [Authorize(Policy = "Editor")]
        [HttpPost]
        public IActionResult Manage(PageViewModel vm)
        {
            Page entity = vm.Page;
            var isDeveloper = _uow.Admin.IsAccessByRole("developer");

            #region serializeHeader
            Header headerJson = vm.Header;
            string newHeaderJsonData = JsonConvert.SerializeObject(headerJson);
            entity.HeaderJsonData = newHeaderJsonData;
            #endregion
            if (entity.Id == 0)
            {
                entity.SitemapId = _uow.Sitemap.CreatePermalink(entity.Title, "Weekly", "Page", Convert.ToDecimal(0.9), _uow.Cookie.GetAdminLangId);

                if (!isDeveloper)
                {
                    entity.PageTypeId = _uow.PageType.GetNormalPageTypeId;
                }

                _uow.Page.Add(entity);

                #region addPageRevision
                _uow.PageRevision.Add(entity, _uow);
                #endregion addPageRevision
            }
            else
            {
                var item = _uow.Page.GetById(entity.Id);

                item.Title = entity.Title;
                item.SeoTitle = entity.SeoTitle;
                item.SeoTitle = entity.SeoTitle;
                item.SeoDescription = entity.SeoDescription;
                item.PageTypeId = isDeveloper ? entity.PageTypeId : _uow.PageType.GetNormalPageTypeId;
                item.SectionsJsonData = entity.SectionsJsonData;
                item.SlidersJsonData = entity.SlidersJsonData;
                item.HeaderJsonData = entity.HeaderJsonData;
                _uow.Page.Update(item);


                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                string methodTitle = entity.Title;
                int methodId = entity.Id;

                _uow.AdminAction.AddAdminAction(controllerName, actionName, methodId, methodTitle, _uow);

                #region addPageRevision
                _uow.PageRevision.Add(item, _uow);
                #endregion addPageRevision
            }
            return RedirectToAction("list");
        }
        [Authorize(Policy = "Admin")]
        public IActionResult Delete(int id)
        {
            var item = _uow.Page.GetById(id);
            if (item != null)
            {
                #region deleteSitemap
                Sitemap sitemap = _uow.Sitemap.GetById(Convert.ToInt32(item.SitemapId));
                if (sitemap != null)
                {
                    _uow.Sitemap.Delete(sitemap);
                }

                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                string methodTitle = item.Title;
                int methodId = item.Id;

                _uow.AdminAction.AddAdminAction(controllerName, actionName, methodId, methodTitle, _uow);

                #endregion
                return Json("success");
            }
            return Json("failed");
        }

        [Authorize(Policy = "Admin")]
        [HttpPost]
        public IActionResult IsRoot(bool value, int id)
        {
            Page item = _uow.Page.GetById(id);
            if (item != null)
            {
                if (!(item.IsRoot == true && value == false))
                {
                    item.IsRoot = value;
                    _uow.Page.IsRootCommand(item, value, _uow.Cookie.GetAdminLangId);
                    if (value == true)
                    {
                        return Json("start");
                    }
                }
                return Json("pause");
            }
            return Json("no");
        }
    }
}