using entity.ExternalModel;
using entity.Models;
using entity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using service.Abstract;
using service.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace admin.Controllers
{
    [Authorize]
    public class MediaController : Controller
    {

        private readonly IUnitOfWork _uow;
        private readonly IWebHostEnvironment _env;
        public MediaController(IUnitOfWork uow, IWebHostEnvironment env)
        {
            _uow = uow;
            _env = env;
        }
        [HttpGet]
        public IActionResult Gallery()
        {
            MediaViewModel vm = new MediaViewModel();
            vm.Medias = _uow.Media.GetAllByDate();
            return View(vm);
        }
        public IActionResult Manage()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> List(int pageNumber = 1)
        {
            return View(await PaginatedList<Media>.CreateAsync(_uow.Media.GetAllByDate(), pageNumber, 20));
        }

        [HttpPost]
        public JsonResult Insert(IFormFile fileUpload)
        {

            // Eğer dosya yüklenmişse dosya adını düzenle ve kaydet
            // Ayrıca dosya adında özel karakterler olmaması için özel karakterleri temizliyoruz.
            if (fileUpload != null && fileUpload.Length > 0)
            {
                var fileName = Path.GetFileName(fileUpload.FileName);
                var extension = Path.GetExtension(fileName);
                var withoutSpecialCharacters = CharacterRegulatory(Path.GetFileNameWithoutExtension(fileName));
                fileName = _uow.Media.ChangeLastChar(withoutSpecialCharacters, extension);

                UploadFileToFolder(fileUpload, fileName);

                var media = new Media()
                {
                    FileNames = fileName,
                    Date = DateTime.Now,
                    Path = "/uploads/" + fileName,
                    Title = withoutSpecialCharacters,
                    FileProjectId = 1,
                    Description = null,
                    Type = fileUpload.ContentType
                };
                _uow.Media.Add(media);
                return Json(media);
            }
            else
            {
                return Json("Failed");
            }
        }

        private void UploadFileToFolder(IFormFile fileUpload, string fileName)
        {
            //Yüklenen dosyayı stream ile yükleme
            // Dosya yüklendiğinde root olarak admin geliyor. Bu yüzden www klasörüne kaydetmek için admin klasörünü www ile değiştiriyoruz.
            var uploadFolderPath = Path.Combine(_env.WebRootPath, "uploads");
            var uploadFolderwww = uploadFolderPath.Replace("admin", "www");
            var imagePath = Path.Combine(uploadFolderwww, fileName);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                fileUpload.CopyTo(stream);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = _uow.Media.GetById(Convert.ToInt32(id));
            if (item != null)
            {
                _uow.Media.DeleteImage(item);
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                string methodTitle = item.Title;
                int methodId = item.Id;

                _uow.AdminAction.AddAdminAction(controllerName, actionName, methodId, methodTitle, _uow);
                return Json("success");
            }
            return Json("failed");
        }

        public static string CharacterRegulatory(string name)
        {
            string pattern = "[\"!'^+%&/()=?_@€¨~,;:.ÖöÜüıIİğĞæßâîşŞÇç<>|]";
            string cleanedName = Regex.Replace(name, pattern, "");

            cleanedName = cleanedName.Replace(" ", "-");
            cleanedName = cleanedName.ToLower();

            return cleanedName;
        }
    }
}
