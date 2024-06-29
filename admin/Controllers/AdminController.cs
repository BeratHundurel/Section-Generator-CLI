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
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _uow;
        public AdminController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        [HttpPost]
        public IActionResult SetPanelSetting(AdminSetting setting)
        {
            Admin admin = _uow.Admin.GetById(Convert.ToInt32(_uow.Admin.GetIdByAdmin));
            if (admin != null)
            {
                #region setSetting
                string newJsonData = JsonConvert.SerializeObject(setting);
                admin.PanelSettingJsonData = newJsonData;
                _uow.Admin.Update(admin);
                #endregion
            }
            return Redirect("/admin");
        }
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> List(int pageNumber = 1, int pageSize = 50)
        {
            var list = _uow.Admin.GetAll(false);
            //Yöneticiler geliştiricileri göremesin.
            if (_uow.Admin.IsAccessByRole("admin"))
            {
                list = list.Where(p => p.Role != "developer");
            }
            return View(await PaginatedList<Admin>.CreateAsync(list.Include(p => p.ImageMedia), pageNumber, pageSize));
        }
        [Authorize(Policy = "Admin")]
        public JsonResult PublishStatus(bool enabled, int itemId)
        {
            var item = _uow.Admin.GetById(itemId);
            if (item != null)
            {
                item.Enabled = enabled;
                _uow.Admin.Update(item);
                if (enabled == true)
                {
                    return Json("start");
                }
                return Json("pause");
            }
            return Json("no");
        }
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public IActionResult Manage(int? id)
        {
            var admin = _uow.Admin.GetById(Convert.ToInt32(id));
            if (admin != null)
            {
                //Geliştirici Admin tarafından düzenlenemez.
                if (admin.Role == "developer" && _uow.Admin.IsAccessByRole("admin")) return NotFound();
                var decryptedPassword = Cipher.Decrypt(admin.Password, admin.Email);
                admin.Password = decryptedPassword;
                return View(admin);
            }
            return View(new Admin());
        }
        [Authorize(Policy = "Admin")]
        [HttpPost]
        public IActionResult Manage(Admin entity)
        {
            entity.Enabled = false;
            if (entity.Id == 0)
            {
                entity.RegisterDate = DateTime.Now;
                entity.LastLoginDate = DateTime.Now;
                entity.LastLoginIp = HttpContext.Connection.RemoteIpAddress.ToString();
                entity.Password = Cipher.Encrypt(entity.Password, entity.Email);


                _uow.Admin.Add(entity);
                return RedirectToAction("list");
            }
            else
            {
                entity.LastLoginDate = DateTime.Now;
                entity.LastLoginIp = HttpContext.Connection.RemoteIpAddress.ToString();
                entity.Password = Cipher.Encrypt(entity.Password, entity.Email);
                _uow.Admin.Update(entity);

                #region adminAction
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                string methodTitle = entity.Name;
                int methodId = entity.Id;

                _uow.AdminAction.AddAdminAction(controllerName, actionName, methodId, methodTitle, _uow);

                #endregion adminAction
                return RedirectToAction("list");
            }
        }
        [HttpGet]
        public IActionResult Profile()
        {
            var admin = _uow.Admin.GetById(Convert.ToInt32(_uow.Admin.GetIdByAdmin));
            if (admin != null)
            {
                var decryptedPassword = Cipher.Decrypt(admin.Password, admin.Email);
                admin.Password = decryptedPassword;
                return View(admin);
            }
            return RedirectToAction("Welcome", "Home");
        }
        [HttpPost]
        public IActionResult Profile(Admin entity)
        {
            Admin item = _uow.Admin.GetById(_uow.Admin.GetIdByAdmin);
            if (item != null)
            {
                item.Name = entity.Name;
                item.LastName = entity.LastName;
                item.Email = entity.Email;
                item.Phone = entity.Phone;

                item.LastLoginDate = DateTime.Now;
                item.LastLoginIp = HttpContext.Connection.RemoteIpAddress.ToString();
                item.Password = Cipher.Encrypt(entity.Password, entity.Email);

                item.ImageMediaId = entity.ImageMediaId;

                _uow.Admin.Update(item);
                return RedirectToAction("Welcome", "Home");
            }
            return NotFound();
        }
        [Authorize(Policy = "Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Admin admin = _uow.Admin.GetById(id);
            if (admin != null)
            {
                #region changeIsDeleted
                admin.IsDeleted = true;
                _uow.Admin.Update(admin);
                #endregion

                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                string methodTitle = admin.Name;
                int methodId = admin.Id;

                _uow.AdminAction.AddAdminAction(controllerName, actionName, methodId, methodTitle, _uow);
                return Json("success");
            }
            return Json("failed");
        }
    }
}