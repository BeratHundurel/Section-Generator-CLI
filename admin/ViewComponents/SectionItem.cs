using admin.ViewModel;
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
    public class SectionItem : ViewComponent
    {
        public SectionItem()
        {
        }
        public IViewComponentResult Invoke(SectionItemViewModel vm)
        {
            return View(vm);
        }
    }
}
