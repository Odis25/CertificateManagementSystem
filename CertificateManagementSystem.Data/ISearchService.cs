using CertificateManagementSystem.Data.Models;
using System.Collections.Generic;

namespace CertificateManagementSystem.Data
{
    public interface ISearchService
    {
        IEnumerable<Document> Find(SearchRequest searchRequest);
    }
}
