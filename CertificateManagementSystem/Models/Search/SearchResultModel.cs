using CertificateManagementSystem.Models.Document;
using System.Collections.Generic;

namespace CertificateManagementSystem.Models.Search
{
    public class SearchResultModel
    {
        public SearchModel SearchModel { get; set; }
        public IEnumerable<DocumentListingModel> SearchResults { get; set; }
    }
}
