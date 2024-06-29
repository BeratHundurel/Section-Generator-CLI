using entity.ExternalModel;
using entity.Models;
using service.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace admin.ViewModel
{
    public class AdminPageViewModel
    {
        public IQueryable<string> strings { get; set; }
        public IQueryable<string> strings1 { get; set; }
        public IQueryable<string> strings2 { get; set; }
        public IQueryable<string> strings3 { get; set; }
        public PaginatedList<Billing> Billings { get; set; }
        public PaginatedList<Customer> Customers { get; set; }
        public PaginatedList<Contact> Contacts { get; set; }
        public PaginatedList<AdminLogin> AdminLogins { get; set; }
        public PaginatedList<AdminAction> AdminActions { get; set; }
        public SearchFilterType f { get; set; }
    }
}