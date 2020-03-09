using System.Collections.Generic;

namespace CertificateManagementSystem.Data.Models
{
    public class Device
    {
        public int Id { get; set; }

        // Наименование оборудования
        public string Name { get; set; }
        // Тип оборудования
        public string Type { get; set; }
        // Заводской номер
        public string SerialNumber { get; set; }

        // Документы на данное оборудование
        public IEnumerable<Document> Documents { get; set; }
    }
}
