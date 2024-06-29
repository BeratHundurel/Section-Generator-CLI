using entity.Models;
using entity.ViewModels;
using ImageProcessor.Processors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using service.Abstract;
using service.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace www.Controllers
{
    public class ActionController : Controller
    {
        private readonly ILogger<ActionController> _logger;
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _configuration;


        //Dependency Injection
        public ActionController(ILogger<ActionController> logger, IUnitOfWork uow, IConfiguration configuration)
        {
            _logger = logger;
            _uow = uow;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Contact(Contact entity)
        {
            if (entity != null)
            {
                entity.IsRead = false;
                entity.LanguageId = _uow.Language.GetById(_uow.Cookie.GetUserLanguageId).Id;
                entity.CreatedDate = DateTime.Now;
                entity.CreatedIp = HttpContext.Connection.RemoteIpAddress.ToString();
                _uow.Contact.Add(entity);
                SendMessageMail(entity);
                return Content($"<p class='text-success'>Mesajınız başarıyla gönderilmiştir</p>", "text/html");

            }
            else
            {
                return Content($"<p class='text-danger'>Mesajınız gönderilememiştir</p>", "text/html");
            }
        }

        public void SendMessageMail(Contact entity)
        {
            // Mailin gövdesini oluşturma kısmı
            string bodyHTML = "";

            Email ef;
            ef = _uow.Contact.GetIdEmailId(1);

            bodyHTML = ef.Html
                .Replace("{name}", entity.Name)
                //.Replace("{phone}", entity.Phone)
                .Replace("{email}", entity.Email)
                .Replace("{message}", entity.Message)
                .Replace("{date}", entity.CreatedDate.ToString())
                .Replace("{subject}", entity.Subject);

            Helpers.Send(_configuration["Email:InformationEmail"], "Yeni bir ileti alındı.", bodyHTML, string.Empty, _configuration);
        }

        public IActionResult ChangeCookieLang(int langId)
        {
            _uow.Cookie.SetUserLanguageCookie(langId);
            var page = _uow.Page.GetAllByLang(langId).FirstOrDefault();
            var permaLink = _uow.Sitemap.GetById(page.SitemapId).Permalink;
            //return RedirectToAction("Index", "Main",new{ permaLink = permaLink});
            return Ok(permaLink);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


