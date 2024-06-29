using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin.ViewModel
{
    public class LinkViewModel
    {
        public int DataId { get; set; }
        public string DataUrl { get; set; }
        public bool IsActive { get; set; }

        public string Id { get; set; }
        public string Title = "";
        public string ClassName = "btn btn-admin";
        public string Href = "/admin";
        public string Target { get; set; }
        public string IconClass = "bx bx-pencil";
    }
}
