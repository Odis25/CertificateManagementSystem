using CertificateManagementSystem.Data.Models;
using System.Collections.Generic;

namespace CertificateManagementSystem.Services.Interfaces
{
    public interface ISearchService
    {
        IEnumerable<Document> Find(SearchRequest searchRequest);
    }
}
