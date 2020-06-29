using System.Collections.Generic;

namespace CertificateManagementSystem.Models.Account
{
    public class ManageAccountsModel
    {
        public IEnumerable<UserModel> Users { get; set; }
    }
}
