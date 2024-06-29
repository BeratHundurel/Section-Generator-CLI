using entity.Models;
using entity.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using service.Abstract;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace admin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly IUnitOfWork _uow;
        public HomeController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                if (_uow.Cookie.GetAdminLangId != 0)
                {
                    return Redirect("/admin/home/welcome");
                }
                else
                {
                    return Redirect("/admin/home/choice");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RememberPassword(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }
        public IActionResult Welcome()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Admin entity, string returnUrl)
        {

            HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");
            Admin admin = _uow.Admin.GetAdmin(entity.Email, entity.Password);
            if (admin != null)
            {


                Claim[] claims = new[] {
                    new Claim("id", admin.Id.ToString()),
                    new Claim("name",  admin.Name),
                    new Claim("role", admin.Role),
                };
                ClaimsIdentity identity = new ClaimsIdentity(claims, admin.Name);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                identity.AddClaim(new Claim(ClaimTypes.Role, admin.Role));
                //Cookiede dil vewebsitesi varsa

                if (_uow.Cookie.GetAdminLangId != 0)
                {
                    _uow.AdminLogin.AddAdminLogin(_uow, admin.Id);
                }

                #region setSetting
                string newJsonData = JsonConvert.SerializeObject(new PanelSetting());
                admin.PanelSettingJsonData = newJsonData;
                _uow.Admin.Update(admin);
                #endregion

                _uow.Admin.UpdateAdmin(admin);



                if (returnUrl != null)
                {
                    return Redirect(returnUrl ?? "/");
                }
                await HttpContext.SignInAsync(principal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddYears(1)
                });
                return RedirectToAction("Index");
            }
            else if (admin == null)
            {
                ViewBag.WrongPassword = "Girmiş olduğunuz bilgiler birbirleriyle uyuşmuyor. Lütfen tekrar deneyiniz.";
                return View("Login");
            }
            return View();
        }
        [HttpGet]
        public IActionResult Choice(string returnUrl)
        {
            //Birden fazla dil varsa
            bool isAnyLanguage = _uow.Language.GetAll().Count() <= 1;
            if (isAnyLanguage)
            {
                //Otomatik dil ve site seçiliyor.
                _uow.Cookie.SetDefaultPanelLangWebsite(_uow);

                return Redirect("welcome");
            }
            ViewBag.returnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        public IActionResult Choice(int langId, string returnUrl)
        {
            //This keys are important to login and choice websites and languages.
            string languageKey = "AdminPanelLanguageId";

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddMonths(2);


            //Cookiede dil vewebsitesi varsa



            if (langId != 0)
            {

                _uow.Cookie.SetCookie(languageKey, langId.ToString(), option);
                _uow.AdminLogin.AddAdminLogin(_uow, langId);

                if (returnUrl != null)
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("welcome");
            }
            return View();
        }

        [AllowAnonymous]
        public JsonResult SetPanelChoiceCookie(int langId)
        {
            //This keys are important to login and choice websites and languages.
            string languageKey = "AdminPanelLanguageId";

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddMonths(2);

            if (langId != 0)
            {
                if (langId != 0)
                {
                    _uow.Cookie.SetCookie(languageKey, langId.ToString(), option);
                }
                return Json("success");
            }
            return Json("failed");
        }
        [AllowAnonymous]
        public JsonResult GetPanelChoiceCookie()
        {
            var langCookie = _uow.Cookie.GetAdminLangId;
            return Json(new { lang = langCookie }); ;
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }



    }
}
