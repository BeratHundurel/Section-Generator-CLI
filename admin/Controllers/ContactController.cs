using admin.ViewModel;
using entity.ExternalModel;
using entity.Models;
using entity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class ContactController : Controller
    {
        public readonly IUnitOfWork _uow;
        public ContactController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<IActionResult> List(SearchFilterType f, int pageNumber = 1, int pageSize = 500)
        {
            var list = _uow.Contact.GetAll(_uow.Cookie.GetAdminLangId);
            if (f.q != null && f.q.GetType() == typeof(string))
            {
                list = list.Where(p =>
                p.Name.Contains(f.q)
                || p.Surname.Contains(f.q)
                || p.Subject.Contains(f.q)
                || p.Message.Contains(f.q));
            }
            if (f.r1 != null && f.r1.GetType() == typeof(bool))
            {
                list = list.Where(p => p.IsRead == f.r1);
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
                Contacts = await PaginatedList<Contact>.CreateAsync(list, pageNumber, pageSize),
                f = f
            };
            return View(vm);
        }
        public IActionResult Read([FromQuery] int id)
        {
            Contact item = _uow.Contact.GetById(id);
            if (item != null)
            {
                item.IsRead = true;
                _uow.Contact.Update(item);
                return View(item);
            }
            return NotFound();
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = _uow.Contact.GetById(Convert.ToInt32(id));
            if (item != null)
            {
                _uow.Contact.Delete(item);

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