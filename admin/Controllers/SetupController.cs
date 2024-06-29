using entity.ExternalModel;
using entity.Models;
using entity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using service.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace admin.Controllers
{
    [Authorize(Policy = "Developer")]
    public class SetupController : Controller
    {
        public readonly IConfiguration _configuration;
        public readonly IUnitOfWork _uow;
        public SetupController(IUnitOfWork uow, IConfiguration configuration)
        {
            _uow = uow;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Start()
        {
            JsonPanelSetting ps = _uow.PanelSetting.GetFirstPanelSetting();
            if (ps.isSetup == false)
            {
                return View();
            }
            return NotFound();
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Start([FromForm] string token)
        {
            if (token == _configuration.GetValue<string>("Setup:Token"))
            {
                JsonPanelSetting ps = _uow.PanelSetting.GetFirstPanelSetting();
                //panel ayarı yoksa onu kurmalıyız
                if (ps == null)
                {
                    //Varsayılan panel ayarlarını kuralım
                    _uow.Setup.SetPanelSetting(_uow);
                }

                //Daha önce hiç kurulum yapılmadıysa
                if (ps.isSetup == false)
                {
                    //Kullanıcı yoksa kullanıcı ekle
                    _uow.Setup.CreateDefaultAdmins(_uow, _configuration);

                    //Varsayılan dilleri ekle
                    _uow.Setup.CreateDefaultLanguages(_uow);

                    //Varsayılan ayarları ekle
                    _uow.Setup.CreateDefaultSettings(_uow);

                    ps.isSetup = true;
                    PanelSetting panelSetting = _uow.PanelSetting.GetFirst();
                    panelSetting.JsonData = JsonConvert.SerializeObject(ps);
                    _uow.PanelSetting.Update(panelSetting);

                    return RedirectToAction("login", "home");
                }
                return NotFound();
            }
            else
            {
                ViewBag.AlertCode = "not-found-token";
                return View();
            }
        }
        //Genel onarım yapın. Tüm onarımlar burada çalıştırılır.
        public IActionResult GeneralFix()
        {
            //Durum
            string status;

            //Ayarları kontrol edin/kurun.

            //Menüyü kontrol edin/kurun.
            status = _uow.Setup.SetDefaultMenu(_uow);

            //Kullanıcıları kontrol edin/kurun.

            //Sayfaları türlerine göre kontrol edin/kurun.
            _uow.Setup.FixPagesByType(_uow);

            //Varsayılan sayfaları kontrol edin/kurun.

            TempData["status"] = status;
            return RedirectToAction("index");
        }
        public IActionResult FixPagesByType()
        {
            #region fixPagesProblem
            #region adminAction
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            string methodTitle = "FixPagesByType - Sayfa Türleri Kontrolü";
            _uow.AdminAction.AddAdminAction(controllerName, actionName, null, methodTitle, _uow);
            #endregion adminAction

            //Sayfa türüne göre kontrolleri sağlayıp sayfa ekleme/silme yapın.
            _uow.Setup.FixPagesByType(_uow);

            #endregion
            return RedirectToAction("list", "page");
        }
        //Varsayılan menüyü kurun
        public IActionResult SetDefaultMenu()
        {
            #region adminAction
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            string methodTitle = "SetDefaultMenu - Hazır Menü Oluşturma";
            _uow.AdminAction.AddAdminAction(controllerName, actionName, null, methodTitle, _uow);
            #endregion adminAction

            string status = _uow.Setup.SetDefaultMenu(_uow);
            return RedirectToAction("index");
        }
    }
}