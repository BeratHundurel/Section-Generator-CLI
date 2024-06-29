﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using service.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace www.ViewComponents
{
    public class Footer : ViewComponent
    {
        private readonly IUnitOfWork _uow;
        public Footer(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}