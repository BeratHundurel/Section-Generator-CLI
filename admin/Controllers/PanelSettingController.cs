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
    //Yalnızca Geliştiriciler İşlem Yapmalı
    [Authorize(Policy = "Developer")]
    public class PanelSettingController : Controller
    {
        public readonly IUnitOfWork _uow;
        public PanelSettingController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        [HttpGet]
        public IActionResult Manage()
        {
            PanelSetting setting = _uow.PanelSetting.SetPanelSetting();
            string settingJsonData = @"[" + setting.JsonData + "]";
            JsonPanelSetting jsonEntity = JsonConvert.DeserializeObject<List<JsonPanelSetting>>(settingJsonData).ToList()[0];
            return View(jsonEntity);
        }
        [HttpPost]
        public IActionResult Manage(JsonPanelSetting entity)
        {
            PanelSetting setting = _uow.PanelSetting.GetAll().First();
            string settingJsonData = @"[" + setting.JsonData + "]";
            JsonPanelSetting jsonEntity = JsonConvert.DeserializeObject<List<JsonPanelSetting>>(settingJsonData).ToList()[0];
            if (setting != null)
            {
                entity.isSetup = jsonEntity.isSetup;
                setting.JsonData = JsonConvert.SerializeObject(entity);
                _uow.PanelSetting.Update(setting);
                TempData["SuccessMessage"] = "Ayarlar başarıyla uygulandı.";
            }
            return RedirectToAction("Manage");
        }
    }
}