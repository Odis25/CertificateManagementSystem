using CertificateManagementSystem.Services.Models;
using System.Collections.Generic;

namespace CertificateManagementSystem.Services.ViewModels.Document
{
    public class DocumentIndexModel
    {
        public int DocumentsCount { get; set; }
        public int CertificatesCount { get; set; }
        public int FailureNotificationsCount { get; set; }

        public IEnumerable<DocumentListingModel> Documents { get; set; }
        public IEnumerable<YearDTO> Years { get; set; }
    }
}
