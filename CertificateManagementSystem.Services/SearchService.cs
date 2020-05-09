using CertificateManagementSystem.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace CertificateManagementSystem.Services
{
    public class SearchService : ISearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
