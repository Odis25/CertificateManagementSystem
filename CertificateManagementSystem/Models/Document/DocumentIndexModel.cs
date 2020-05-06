using System.Collections.Generic;

namespace CertificateManagementSystem.Models.Document
{
    public class DocumentIndexModel
    {
        public int DocumentsCount { get; set; }
        public int CertificatesCount { get; set; }
        public int FailureNotificationsCount { get; set; }

        public IEnumerable<DocumentListingModel> Documents { get; set; }
        public IEnumerable<YearModel> Years { get; set; }
    }
}
