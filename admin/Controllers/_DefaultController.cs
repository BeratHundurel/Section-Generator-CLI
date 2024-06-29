using entity.ExternalModel;
using entity.Models;
using entity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using service.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace admin.Controllers
{
    [Authorize(Policy = "Developer")]
    public class _DefaultController : Controller
    {
        public readonly IUnitOfWork _uow;
        public _DefaultController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        [HttpGet]
        public IActionResult Setup()
        {
            return RedirectToAction("Manage");
        }

    }
}