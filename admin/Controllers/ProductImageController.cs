using entity.ExternalModel;
using entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using service.Abstract;
using service.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace admin.Controllers
{
    [Authorize(Policy = "Admin")]
    public class ProductImageController : Controller
    {
        private readonly IUnitOfWork _uow;
        public ProductImageController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<IActionResult> List(int productId, int pageNumber = 1, int pageSize = 50)
        {
            ViewBag.ProductId = productId;
            return View(await PaginatedList<ProductImage>.CreateAsync(_uow.ProductImage.GetQueryableListProductImages(productId), pageNumber, pageSize));
        }
        public IActionResult Manage(int id)
        {
            if (id == 0)
            {
                return View(new ProductImage());
            }
            else
            {
                var product = _uow.ProductImage.GetById(id);
                return View(product);
            }
        }
        [HttpPost]
        public IActionResult Manage(ProductImage entity)
        {
            if (entity.Id == 0)
            {
                ViewBag.ProductId = entity.ProductId;
                _uow.ProductImage.Add(entity);
                return RedirectToAction("list", new { productId = entity.ProductId });
            }
            else
            {
                ViewBag.ProductId = entity.ProductId;
                _uow.ProductImage.Update(entity);
                return RedirectToAction("list", new { productId = entity.ProductId });
            }
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = _uow.ProductImage.GetById(Convert.ToInt32(id));
            if (item != null)
            {
                _uow.ProductImage.Delete(item);

                var media = _uow.Media.GetById(item.MediaId);
                _uow.Media.Delete(media);

                return Json("success");
            }
            return Json("failed");
        }
    }
}