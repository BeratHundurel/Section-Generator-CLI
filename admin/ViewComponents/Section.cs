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
    public class Section : ViewComponent
    {
        public Section()
        {
        }
        public IViewComponentResult Invoke(SectionItemViewModel vm)
        {
            if (vm.SectionName == "_Slider")
            {
                return View(vm.ViewName, vm.Slide);
            }
            return View(vm.ViewName, vm.SVM);
        }
    }
}
