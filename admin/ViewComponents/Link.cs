﻿using admin.ViewModel;
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
    public class Link : ViewComponent
    {
        public Link()
        {
        }
        public IViewComponentResult Invoke(LinkViewModel vm, string viewName = "Default")
        {
            return View(viewName, vm);
        }
    }
}