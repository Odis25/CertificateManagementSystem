using System.Collections.Generic;

namespace CertificateManagementSystem.Data.Models
{
    public class Contract
    {
        public int Id { get; set; }
        // Год заключения договора
        public int Year { get; set; }

        // Номер договора
        public string ContractNumber { get; set; }
        // Название организации-заказчика
        public string ClientName { get; set; }
        // Название объекта эксплуатации
        public string ObjectName { get; set; }

        // Список оборудования в рамках данного договора
        public IEnumerable<Device> Devices { get; set; }
    }
}
