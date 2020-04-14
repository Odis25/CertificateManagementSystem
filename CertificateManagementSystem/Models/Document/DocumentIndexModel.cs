using System.Collections.Generic;

namespace CertificateManagementSystem.Models.Document
{
    public class DocumentIndexModel
    {
        public IEnumerable<DocumentListingModel> Documents { get; set; }
        public IEnumerable<YearModel> Years { get; set; }
    }
}
