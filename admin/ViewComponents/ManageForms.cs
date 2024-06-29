using entity.ExternalModel;
using Microsoft.AspNetCore.Mvc;
using service.Abstract;

namespace admin.ViewComponents
{
    public class ManageForms : ViewComponent
    {
        private readonly IUnitOfWork _uow;
        public ManageForms(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public IViewComponentResult Invoke(ManageFormModalViewModel vm)
        {
            string viewName = vm.ViewName == null ? "Default" : vm.ViewName;
            return View(viewName, vm);
        }
    }
}
