using CertificateManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Controllers
{
    public class SearchController: Controller
    {
        private readonly ISearchService _search;

        public SearchController(ISearchService search)
        {
            _search = search;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Find(string searchQuery)
        {
            
            return View();
        }
    }
}
