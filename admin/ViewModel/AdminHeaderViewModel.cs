using entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin.ViewModel
{
    public class AdminHeaderViewModel
    {
        public Admin Admin { get; set; }
        public IQueryable<Language> Languages { get; set; }
    }
}
