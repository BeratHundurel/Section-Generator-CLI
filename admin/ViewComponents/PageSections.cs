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
    public class PageSections : ViewComponent
    {
        public PageSections()
        {
        }
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
