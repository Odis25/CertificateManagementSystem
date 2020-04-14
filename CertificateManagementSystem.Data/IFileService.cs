using System.Threading.Tasks;

namespace CertificateManagementSystem.Data
{
    public interface IFileService
    {
        void CreateFile(string sourcePath, string destinationPath);
    }
}
