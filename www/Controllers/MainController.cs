using entity.ExternalModel;
using entity.Models;
using entity.ViewModels;
using Iyzipay.Model.V2.Subscription;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using service.Abstract;
using service.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Product = entity.Models.Product;

namespace www.Controllers
{
    public class MainController : Controller
    {
        private readonly ILogger<MainController> _logger;
        private readonly IUnitOfWork _uow;
        private readonly IHttpContextAccessor _httpContextAccessor;

        //Dependency Injection
        public MainController(ILogger<MainController> logger, IUnitOfWork uow, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _uow = uow;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index(string permaLink)
        {
            permaLink = permaLink == null ? "/" : permaLink;

            Page rootPage;
            Setting setting;
            Language language;
            if (_uow.Cookie.GetUserLanguageId != 0)
            {
                language = _uow.Language.GetById(_uow.Cookie.GetUserLanguageId);
            }
            else
            {
                language = _uow.Language.GetIsRootLang();
            }

            var langId = language.Id;
            Page page = _uow.Page.GetPageByPermalink(permaLink, true);
            Sitemap sitemap = _uow.Sitemap.GetSitemapByPermalink(permaLink, langId);

            if (permaLink == "/")
            {
                //Set Language Cookie For User Pages
                _uow.Cookie.SetUserLanguageCookie(langId);

                page = _uow.Page.GetRootPage(langId, true);
                setting = _uow.Setting.GetByLang(page.Sitemap.LangId);

                #region viewbag
                if (page != null)
                {
                    ViewBag.PageTitle = page.Title == null ? string.Empty : page.Title;
                    ViewBag.SeoTitle = page.SeoTitle == null ? string.Empty : page.SeoTitle;
                    ViewBag.SeoDescription = page.SeoDescription == null ? string.Empty : page.SeoDescription;
                }
                #endregion

                #region jsonData


                string sectionjsonData = @"[" + page.SectionsJsonData + "]";
                string sliderjsonData = @"[" + page.SlidersJsonData + "]";
                string headerjsonData = @"[" + page.HeaderJsonData + "]";
                string settingJsonData = @"[" + setting.JsonData + "]";


                #endregion

                #region viewModel
                PageViewModel PVM = new PageViewModel
                {
                    Slides = JsonConvert.DeserializeObject<List<Slide>>(sliderjsonData).ToList(),
                    Setting = JsonConvert.DeserializeObject<List<JsonSetting>>(settingJsonData).ToList()[0],
                    Sections = JsonConvert.DeserializeObject<List<SectionViewModel>>(sectionjsonData).Where(x => x.LangId == langId).OrderBy(sec => sec.order).ToList(),
                    Page = page,
                    Header = JsonConvert.DeserializeObject<List<Header>>(headerjsonData).ToList()[0],
                    Language = language,
                    LangId = langId,
                    JsonMenus = _uow.Menu.GetMenuFromJson(setting.MenuJsonData),
                };
                #endregion
                return View(PVM);
            }
            else if (sitemap != null)
            {
                setting = _uow.Setting.GetByLang(sitemap.LangId);
                rootPage = _uow.Page.GetRootPage(sitemap.LangId);
                Category category = _uow.Category.GetBySitemapId(sitemap.Id, true);
                SubCategory subcategory = _uow.SubCategory.GetBySitemapId(sitemap.Id, true);
                Product product = _uow.Product.GetBySitemapId(sitemap.Id, true);
                ManCategory manCategory = _uow.ManCategory.GetBySitemapId(sitemap.Id, true);
                Blog blog = _uow.Blog.GetBySitemapId(sitemap.Id, true);

                #region pageByPageType
                if (category != null)
                {
                    page = _uow.Page.GetPageByPageType("CategoryDetail", langId);
                }
                if (subcategory != null)
                {
                    page = _uow.Page.GetPageByPageType("SubCategoryDetail", langId);
                }
                if (product != null)
                {
                    page = _uow.Page.GetPageByPageType("ProductDetail", langId);
                }
                if (manCategory != null)
                {
                    page = _uow.Page.GetPageByPageType("ManCategoryDetail", langId);

                }
                if (blog != null)
                {
                    page = _uow.Page.GetPageByPageType("BlogDetail", langId);
                }
                #endregion

                #region jsonData
                string settingJsonData = @"[" + setting.JsonData + "]";
                string sectionjsonData = @"[" + page.SectionsJsonData + "]";
                string sliderjsonData = @"[" + page.SlidersJsonData + "]";
                string headerjsonData = @"[" + page.HeaderJsonData + "]";
                #endregion

                #region viewModel
                PageViewModel PVM = new PageViewModel
                {
                    Slides = JsonConvert.DeserializeObject<List<Slide>>(sliderjsonData).ToList(),
                    Setting = JsonConvert.DeserializeObject<List<JsonSetting>>(settingJsonData).ToList()[0],
                    Sections = JsonConvert.DeserializeObject<List<SectionViewModel>>(sectionjsonData).Where(x => x.LangId == langId).OrderBy(sec => sec.order).ToList(),
                    Page = page,
                    Header = JsonConvert.DeserializeObject<List<Header>>(headerjsonData).ToList()[0],
                    Language = language,
                    LangId = langId,
                    JsonMenus = _uow.Menu.GetMenuFromJson(setting.MenuJsonData),
                    RootPage = rootPage,
                    Category = category,
                    SubCategory = subcategory,
                    Product = product,
                    ManCategory = manCategory,
                    Blog = blog
                };
                #endregion

                #region viewbag
                ViewBag.RootPageTitle = rootPage.Title;
                ViewBag.RootPagePermalink = rootPage.Sitemap.Permalink;

                ViewBag.PageTitle = page.Title;
                ViewBag.PagePermalink = page.Sitemap.Permalink;

                ViewBag.SeoTitle = page.SeoTitle;
                ViewBag.SeoDescription = page.SeoDescription;
                #endregion

                langId = page.Sitemap.LangId;
                _uow.Cookie.SetUserLanguageCookie(langId);
                return View(PVM);
            }
            else
            {
                if (langId == 1)
                {
                    return Redirect("error");
                }
                else
                {
                    return Redirect("error-en");
                }
            }
        }
        public IActionResult SubscriptionsForm(Subscriptions entity)
        {
            entity.CreatedDate = DateTime.Now;

            if (entity.Email == null)
            {
                return Json("null");
            }
            else
            {
                var isExist = _uow.Subscriptions.GetSubscriptionsByEmail(entity.Email);
                if (isExist != null)
                {
                    return Json("exist");
                }
                else
                {
                    _uow.Subscriptions.AddSubscriptionsForm(entity);
                    return Json("ok");
                }
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
