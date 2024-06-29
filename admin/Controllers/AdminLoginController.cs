using admin.ViewModel;
using entity.ExternalModel;
using entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service.Abstract;
using service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin.Controllers
{
    [Authorize(Policy = "Admin")]
    public class AdminLoginController : Controller
    {

        private readonly IUnitOfWork _uow;
        public AdminLoginController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IActionResult> List(SearchFilterType f, int pageNumber = 1, int pageSize = 500)
        {
            var list = _uow.AdminLogin.GetAll();

            if (f.userId != null && f.userId.GetType() == typeof(int))
            {
                list = list.Where(p => p.AdminId == f.userId);
            }
            if (f.langId != null && f.langId.GetType() == typeof(int))
            {
                list = list.Where(p => p.LangId == f.langId);
            }
            if (f.ip != null && f.ip.GetType() == typeof(string))
            {
                list = list.Where(p => p.UserIp == f.ip);
            }
            if (f.pageSize >= 10 && f.pageSize < 10000 && f.pageSize.GetType() == typeof(int))
            {
                pageSize = Convert.ToInt32(f.pageSize);
            }
            if (f.pageNumber > 0 && f.pageNumber.GetType() == typeof(int))
            {
                pageNumber = Convert.ToInt32(f.pageNumber);
            }
            AdminPageViewModel vm = new AdminPageViewModel
            {
                AdminLogins = await PaginatedList<AdminLogin>.CreateAsync(list, pageNumber, pageSize),
                strings = _uow.AdminLogin.GetAll().Select(p => p.UserIp).Distinct().AsQueryable(),
                f = f
            };
            return View(vm);
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult FilterList(int lang, int web)
        {
            if (lang != 0 && web != 0)
            {
                var lwfilter = _uow.AdminLogin.AdminLoginFilter(lang, web);
                return PartialView("_AdminLoginPartial", lwfilter);
            }
            if (lang == 0 && web != 0)
            {
                var webfilter = _uow.AdminLogin.AdminLoginFilterWeb(web);
                return PartialView("_AdminLoginPartial", webfilter);
            }
            if (lang != 0 && web == 0)
            {
                var langfilter = _uow.AdminLogin.AdminLoginFilterLang(lang);
                return PartialView("_AdminLoginPartial", langfilter);
            }
            return PartialView("_AdminLoginPartial");
        }
    }
}
