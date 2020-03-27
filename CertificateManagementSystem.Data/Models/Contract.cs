
using System.Collections.Generic;

namespace CertificateManagementSystem.Data.Models
{
    public class Contract
    {
        public Contract()
        {
            ContractDevices = new List<ContractDevice>();
        }

        public int Id { get; set; }

        public int Year { get; set; }
        public string ContractNumber { get; set; }

        public virtual Client Client { get; set; }

        public List<ContractDevice> ContractDevices { get; set; }
    }
}
