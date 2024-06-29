using admin.ViewModel;
using entity.ExternalModel;
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
    [Authorize(Policy = "Admin")]
    public class AdminActionController : Controller
    {
        private readonly IUnitOfWork _uow;
        public AdminActionController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<IActionResult> List(SearchFilterType f, int pageNumber = 1, int pageSize = 500)
        {
            var list = _uow.AdminAction.GetAll();
            if (f.userId != null && f.userId.GetType() == typeof(int))
            {
                list = list.Where(p => p.AdminId == f.userId);
            }
            if (f.q != null && f.q.GetType() == typeof(string))
            {
                list = list.Where(p => p.RelatedName == f.q);
            }
            if (f.q2 != null && f.q2.GetType() == typeof(string))
            {
                list = list.Where(p => p.RelatedType == f.q2);
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
                AdminActions = await PaginatedList<AdminAction>.CreateAsync(list, pageNumber, pageSize),
                f = f
            };
            return View(vm);
        }
        public IActionResult FilterList(string name, string process)
        {

            if (name != null && process != null)
            {
                var tpfilter = _uow.AdminAction.AdminActionFilter(name, process);
                return PartialView("_AdminActionPartial", tpfilter);
            }
            if (name != null && process == null)
            {
                var namefilter = _uow.AdminAction.AdminActionFilterName(name);
                return PartialView("_AdminActionPartial", namefilter);
            }
            if (name == null && process != null)
            {
                var processfilter = _uow.AdminAction.AdminActionFilterProcess(process);
                return PartialView("_AdminActionPartial", processfilter);
            }
            return PartialView("_AdminActionPartial");
        }



    }
}