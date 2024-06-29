using entity.ExternalModel;
using Microsoft.AspNetCore.Mvc;
using service.Abstract;

namespace admin.ViewComponents
{
    public class ManageFormModal : ViewComponent
    {
        private readonly IUnitOfWork _uow;
        public ManageFormModal(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public IViewComponentResult Invoke(ManageFormModalViewModel vm)
        {
            return View(vm);
        }
    }
}
