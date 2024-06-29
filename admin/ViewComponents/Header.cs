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
    public class Header : ViewComponent
    {
        private readonly IUnitOfWork _uow;
        public Header(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public IViewComponentResult Invoke(string viewName)
        {
            AdminHeaderViewModel vm = new AdminHeaderViewModel
            {
                Admin = _uow.Admin.GetById(Convert.ToInt32(_uow.Admin.GetIdByAdmin)),
                Languages = _uow.Language.GetAllByEnabled()
            };
            return View(viewName, vm);
        }
    }
}
