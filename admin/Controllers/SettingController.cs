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
    [Authorize(Policy = "CoAdmin")]
    public class SettingController : Controller
    {
        public readonly IUnitOfWork _uow;
        public SettingController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        [HttpGet]
        public IActionResult Manage()
        {
            Setting setting = _uow.Setting.Get(_uow.Cookie.GetAdminLangId);
            if (setting == null)
            {
                JsonSetting settingJson = new JsonSetting();
                string newJsonData = JsonConvert.SerializeObject(settingJson);

                Setting newSetting = new Setting { LangId = _uow.Cookie.GetAdminLangId, JsonData = newJsonData };
                _uow.Setting.Add(newSetting);
                setting = newSetting;
            }
            string settingJsonData = @"[" + setting.JsonData + "]";
            JsonSetting jsonEntity = JsonConvert.DeserializeObject<List<JsonSetting>>(settingJsonData).ToList()[0];
            return View(jsonEntity);
        }
        [HttpPost]
        public IActionResult Manage(JsonSetting entity)
        {
            Setting setting = _uow.Setting.Get(_uow.Cookie.GetAdminLangId);
            setting.JsonData = JsonConvert.SerializeObject(entity);
            _uow.Setting.Update(setting);
            TempData["SuccessMessage"] = "Ayarlar başarıyla uygulandı.";
            return RedirectToAction("Manage");
        }

    }
}