﻿using System.Collections.Generic;

namespace CertificateManagementSystem.Data.Models
{
    public class Contract
    {
        public int Id { get; set; }

        public int Year { get; set; }
        public string ContractNumber { get; set; }

        public Client Client { get; set; }

        public IEnumerable<Device> Devices { get; set; }
    }
}
