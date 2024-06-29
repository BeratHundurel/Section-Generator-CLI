using entity.ExternalModel;
using entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace admin.ViewModel
{
    public class SectionItemViewModel
    {
        public string ViewName { get; set; }
        public string SectionName { get; set; }
        public SectionViewModel SVM { get; set; }
        public Slide Slide { get; set; }
        public bool IsCollapsed = true;
    }
}
