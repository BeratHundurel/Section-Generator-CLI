using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin.ViewModel
{
    public class ButtonViewModel
    {
        public string Title = "Button";
        public string ClassName = "btn btn-admin";
    }
    public class ButtonGroupViewModel
    {
        public string ListButtonTitle = "Listeye Dön";
        public string SubmitButtonTitle = "Gönder";
        public bool IsThereSubmitButton = true;
        //For example "page" --> /page/list
        public string ListControllerName { get; set; }
        public List<LinkViewModel> ListButtonsPermalinks { get; set; }
    }
}
