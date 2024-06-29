using entity.ExternalModel;
using Microsoft.AspNetCore.Mvc;
using service.Abstract;
using System;
using System.Threading.Tasks;

namespace www.Controllers
{
    public class SitemapController : Controller
    {
        private readonly IUnitOfWork _uow;
        public SitemapController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [Route("sitemap")]
        public async Task<ActionResult> SitemapAsync()
        {
            string domain = Request.Host.Value;
            string baseUrl = Request.Scheme + "://" + domain + "/";

            var items = await _uow.Sitemap.GetSitemapsAsync();


            var siteMapBuilder = new SitemapBuilder();

            // add the home page to the sitemap
            siteMapBuilder.AddUrl(baseUrl, modified: DateTime.UtcNow, changeFrequency: "Weekly", priority: 1.0);

            //siteMapBuilder.AddUrl(baseUrl + "hakkimizda", modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Monthly, priority: 0.8);
            //siteMapBuilder.AddUrl(baseUrl + "iletisim", modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Yearly, priority: 0.8);
            //siteMapBuilder.AddUrl(baseUrl + "makaleler", modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Daily, priority: 0.9);
            //siteMapBuilder.AddUrl(baseUrl + "danisan-uyelik-sozlesmesi", modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Yearly, priority: 0.7);
            //siteMapBuilder.AddUrl(baseUrl + "gizlilik-ve-kisisel-veri-politikasi-aydinlatma-ve-riza-metni", modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Yearly, priority: 0.7);

            // add the blog posts to the sitemap
            foreach (var item in items)
            {
                siteMapBuilder.AddUrl(baseUrl + item.Permalink, modified: item.Modified, changeFrequency: item.ChangeFrequency, priority: (double)item.Priority);
            }

            // generate the sitemap xml
            string xml = siteMapBuilder.ToString();
            return Content(xml, "text/xml");
        }
    }
}