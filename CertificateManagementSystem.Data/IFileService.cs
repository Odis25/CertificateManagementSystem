using System.Threading.Tasks;

namespace CertificateManagementSystem.Data
{
    public interface IFileService
    {
        void CreateFile(string sourceFilePath, ref string destinationFilePath);
    }
}
