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
    public class SimpleFileUploadInput : ViewComponent
    {
        private readonly IUnitOfWork _uow;
        public SimpleFileUploadInput(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public IViewComponentResult Invoke(FileUploadEntity vm)
        {
            return View(vm);
        }
    }
}
