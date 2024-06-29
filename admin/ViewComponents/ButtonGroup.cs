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
    public class ButtonGroup : ViewComponent
    {
        public ButtonGroup()
        {
        }
        public IViewComponentResult Invoke(ButtonGroupViewModel vm, string viewName = "Default")
        {
            vm = vm == null ? new ButtonGroupViewModel() : vm;
            return View(viewName, vm);
        }
    }
}
