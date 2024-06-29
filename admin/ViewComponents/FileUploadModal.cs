using entity.ExternalModel;
using entity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using service.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin.ViewComponents
{
    public class FileUploadModal : ViewComponent
    {
        private readonly IUnitOfWork _uow;
        public FileUploadModal(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public IViewComponentResult Invoke(AdminFileUploadViewModel vm)
        {
            return View(vm);
        }
    }
}
