using Microsoft.AspNetCore.Http;
using service.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin.Middlewares
{
    public class LangWebsiteCheckingMiddleWare
    {
        private readonly RequestDelegate _next;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        public LangWebsiteCheckingMiddleWare(RequestDelegate next/*, IHttpContextAccessor httpContextAccessor*/)
        {
            _next = next;
            //_httpContextAccessor = httpContextAccessor;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork _uow, IHttpContextAccessor _httpContextAccessor)
        {
            var langId = _uow.Cookie.GetAdminLangId;

            //If the website's langId or the websiteId is null, the middleware redirects to cho
            if ((langId == 0) && context.User.Identity.IsAuthenticated && context.Request.Path != "/home/choice")
            {
                string returnUrl = context.Request.PathBase + context.Request.Path;
                string redirectChoiceUrl = "/admin/home/choice?returnUrl=" + returnUrl;
                context.Response.Redirect(redirectChoiceUrl);
            }
            else
            {
                // Call the next delegate/middleware in the pipeline
                await _next(context);
            }
        }
    }
}
