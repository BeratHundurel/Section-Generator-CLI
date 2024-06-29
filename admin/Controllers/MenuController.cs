using entity.ExternalModel;
using entity.Models;
using entity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using service.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace admin.Controllers
{
    [Authorize(Policy = "Editor")]
    public class MenuController : Controller
    {
        public readonly IUnitOfWork _uow;
        public MenuController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        [HttpPost]
        public IActionResult SetStringJson(string str)
        {
            Setting setting = _uow.Setting.Get(_uow.Cookie.GetAdminLangId);
            //Ayar kurulsun
            _uow.Setting.SetSetting(_uow);
            setting.MenuJsonData = str;
            _uow.Setting.Update(setting);
            return Json("success");
        }
        [HttpGet]
        public IActionResult Manage()
        {
            Setting setting = _uow.Setting.Get(_uow.Cookie.GetAdminLangId);
            if (setting == null)
            {
                setting = _uow.Setup.CreateSetting(_uow, null);
            }
            if (setting.MenuJsonData != null)
            {
                List<JsonMenu> jsonEntity = JsonConvert.DeserializeObject<List<JsonMenu>>(setting.MenuJsonData).ToList();
                return View(jsonEntity);
            }
            return View(new List<JsonMenu>());
        }
        [HttpPost]
        public IActionResult Manage(JsonMenu entity)
        {
            Setting setting = _uow.Setting.Get(_uow.Cookie.GetAdminLangId);
            setting.MenuJsonData = JsonConvert.SerializeObject(entity);
            _uow.Setting.Update(setting);
            TempData["SuccessMessage"] = "Ayarlar başarıyla uygulandı.";
            return RedirectToAction("Manage");
        }

    }
}