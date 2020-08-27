using CertificateManagementSystem.Services.ViewModels.Document;
using System.Collections.Generic;

namespace CertificateManagementSystem.Services.ViewModels.Search
{
    public class SearchResultModel
    {
        public SearchModel SearchModel { get; set; }
        public IEnumerable<DocumentListingModel> SearchResults { get; set; }
    }
}
