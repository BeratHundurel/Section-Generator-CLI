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
    public class Sidebar : ViewComponent
    {
        private readonly IUnitOfWork _uow;
        public Sidebar(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public IViewComponentResult Invoke(string viewName)
        {
            if (viewName == "Switcher")
            {
                return View(viewName, _uow.Admin.GetAdminPanelSetting());
            }
            return View(viewName);
        }
    }
}
