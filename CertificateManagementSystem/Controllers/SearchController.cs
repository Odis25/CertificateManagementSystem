using CertificateManagementSystem.Data;
using CertificateManagementSystem.Models.Search;
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

        public IActionResult Index(SearchModel model)
        {            
            return View(model);
        }

        [HttpPost]
        public IActionResult Find(SearchModel model)
        {
          
            return RedirectToAction("Index", model);
        }
    }
}
