using CertificateManagementSystem.Services.Models;
using System.Collections.Generic;

namespace CertificateManagementSystem.Services.Interfaces
{
    public interface ISearchService
    {
        IEnumerable<DocumentDTO> Find(SearchRequestDTO searchRequest);
    }
}
