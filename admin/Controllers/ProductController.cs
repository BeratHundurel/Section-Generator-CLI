//using entity.Models;
using entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
using service.Abstract;
using service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin.Controllers
{
    [Authorize(Policy = "Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _uow;
        public ProductController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IActionResult> List(int pageNumber = 1, int pageSize = 50)
        {

            //if (!_uow.Admin.IsAccessByRole("developer"))
            //{
            //    items = items.Where(p => p.LangId == items);
            //}
            return View(await PaginatedList<Product>.CreateAsync(_uow.Product.GetAllByLang(_uow.Cookie.GetAdminLangId), pageNumber, pageSize));
        }
        public JsonResult PublishStatus(bool enabled, int itemId)
        {
            var item = _uow.Product.GetById(itemId);
            if (item != null)
            {
                item.Enabled = enabled;
                _uow.Product.Update(item);
                if (enabled == true)
                {
                    return Json("start");
                }
                return Json("pause");
            }
            return Json("no");
        }
        public IActionResult Manage(int id)
        {
            if (id == 0)
            {
                return View(new Product());
            }
            else
            {
                var product = _uow.Product.GetById(id);
                return View(product);
            }
        }
        public IActionResult GetSubCategoryList(int id)
        {
            var res = Json(_uow.SubCategory.GetBySubCategoryId(id));
            return res;
        }
        [Authorize]
        [HttpGet]
        public IActionResult Manage(int? id)
        {
            IQueryable<Category> productCategories = _uow.Category.GetAll(_uow.Cookie.GetAdminLangId);
            IQueryable<ManCategory> manproductCategories = _uow.ManCategory.GetAll(_uow.Cookie.GetAdminLangId);



            if (id != null)
            {
                Product prodData = _uow.Product.GetProductById(id);
                List<SubCategory> subCategories = _uow.SubCategory.GetBySubCategoryId(prodData.CategoryId);
                //List<ManCategory> manCategories = _uow.ManCategory.GetByManCategoryId(prodData.CategoryId);

                var item = _uow.Product.GetIncludedProductCategoryProduct(Convert.ToInt32(id));
                var item2 = _uow.Product.GetIncludedProductSubCategoryProduct(Convert.ToInt32(id));
                var item3 = _uow.Product.GetIncludedManProductCategoryProduct(Convert.ToInt32(id));

                #region FillBlogTagsToSelectList 

                var selectedAmenityList = item.CategoryProductRelation;
                List<SelectListItem> selectListAmenities = new List<SelectListItem>();

                var subCategoryList = item.SubCategoryProductRelation;
                List<SelectListItem> selectSubCategories = new List<SelectListItem>();

                var manCategoryList = item3.ManCategoryProductRelation;
                List<SelectListItem> selectManCategories = new List<SelectListItem>();




                //Add Room Number Items To SelectList
                foreach (var rn in productCategories)
                {
                    SelectListItem sli;

                    if (selectedAmenityList.FirstOrDefault(p => p.CategoryId == rn.Id) != null)
                        sli = new SelectListItem(rn.Name, rn.Id.ToString(), true);
                    else
                        sli = new SelectListItem(rn.Name, rn.Id.ToString());
                    selectListAmenities.Add(sli);
                }

                foreach (var rn in subCategories)
                {
                    SelectListItem sli2;

                    if (subCategoryList.FirstOrDefault(p => p.SubCategoryId == rn.Id) != null)
                        sli2 = new SelectListItem(rn.Name, rn.Id.ToString(), true);
                    else
                        sli2 = new SelectListItem(rn.Name, rn.Id.ToString());
                    selectSubCategories.Add(sli2);
                }


                foreach (var rn in manproductCategories)
                {
                    SelectListItem sli3;

                    if (manCategoryList.FirstOrDefault(p => p.ManCategoryId == rn.Id) != null)
                        sli3 = new SelectListItem(rn.Name, rn.Id.ToString(), true);
                    else
                        sli3 = new SelectListItem(rn.Name, rn.Id.ToString());
                    selectManCategories.Add(sli3);
                }
                ViewBag.ProductCategory = selectListAmenities;
                ViewBag.ManProductCategory = selectManCategories;
                ViewBag.ProductSubCategory = selectSubCategories;
                #endregion

                #region FillColorsToSelectList
                List<SelectListItem> selectListAmenities2 = new List<SelectListItem>();

                #endregion

                return View(item);
            }

            ViewBag.ManProductCategory = manproductCategories.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name }).ToList();
            ViewBag.ProductCategory = productCategories.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name }).ToList();
            return View(new Product());
        }
        [HttpPost]
        public IActionResult Manage(Product entity, int CategoryId, int SubCategoryId, int ManCategoryId)
        {

            if (entity.Id == 0)
            {

                entity.Enabled = false;
                entity.SitemapId = _uow.Sitemap.CreatePermalink(entity.Name, "Weekly", "Product", Convert.ToDecimal(0.6), _uow.Cookie.GetAdminLangId);
                entity.Permalink = _uow.Sitemap.GetById(Convert.ToInt32(entity.SitemapId)).Permalink;
                entity.CategoryId = CategoryId;
                entity.SubCategoryId = SubCategoryId;
                entity.ManCategoryId = ManCategoryId;
                _uow.Product.Add(entity);
            }
            else
            {
                #region sitemapUpdate
                if (_uow.Product.TitleIsChanged(entity))
                {
                    var sitemap = _uow.Sitemap.GetSitemapBySitemapId(Convert.ToInt32(entity.SitemapId));
                    if (sitemap != null)
                    {
                        sitemap.Permalink = _uow.Sitemap.GenerateAndInsertPermalink(entity.Name);
                        _uow.Sitemap.Update(sitemap);
                    }
                }
                #endregion

                entity.Permalink = _uow.Sitemap.GetById(Convert.ToInt32(entity.SitemapId)).Permalink;
                entity.CategoryId = CategoryId;
                entity.SubCategoryId = SubCategoryId;
                entity.ManCategoryId = ManCategoryId;
                _uow.Product.Update(entity);
                _uow.CategoryProductRelation.AllRemoveCategoryRelation(entity.Id);
                _uow.SubCategoryProductRelation.RemoveSubCategoryRelation(entity.Id);
                _uow.ManCategoryProductRelation.AllRemoveManCategoryRelation(entity.Id);

            }


            if (ManCategoryId != 0)
            {
                _uow.ManCategoryProductRelation.AddFromArray(ManCategoryId, entity.Id);

            }
            else
            {
                _uow.CategoryProductRelation.AddFromArray(CategoryId, entity.Id);
                SubCategoryProductRelation subCategoryProductRelation = new SubCategoryProductRelation
                {
                    SubCategoryId = entity.SubCategoryId,
                    ProductId = entity.Id,
                };
                _uow.SubCategoryProductRelation.Add(subCategoryProductRelation);
            }
            return RedirectToAction("list");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = _uow.Product.GetById(Convert.ToInt32(id));
            int sitemapId = Convert.ToInt32(item.SitemapId);
            if (item != null)
            {
                _uow.CategoryProductRelation.AllRemoveCategoryRelation(id);
                _uow.SubCategoryProductRelation.RemoveSubCategoryRelation(id);
                _uow.ManCategoryProductRelation.AllRemoveManCategoryRelation(id);


                var media = _uow.Media.GetById(Convert.ToInt32(item.MediaId));
                _uow.Media.DeleteImage(media);

                _uow.Product.Delete(item);

                var sitemap = _uow.Sitemap.GetById(sitemapId);
                _uow.Sitemap.Delete(sitemap);

                return Json("success");
            }
            return Json("failed");
        }

        public IActionResult GetSubCategories(int categoryId)
        {
            //Category Id doğru getirildi
            var subCategories = _uow.SubCategory.GetBySubCategoryId(categoryId);
            var subCategoryList = subCategories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToList();

            return Json(subCategoryList);
        }

    }
}