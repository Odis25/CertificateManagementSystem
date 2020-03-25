using CertificateManagementSystem.Data.Models;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Data
{
    public interface IFileService
    {
        Task CreateFile(Document document);
    }
}
