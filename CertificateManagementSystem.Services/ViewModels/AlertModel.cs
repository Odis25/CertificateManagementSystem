namespace CertificateManagementSystem.Services.ViewModels
{
    public class AlertModel
    {
        public string Message { get; set; }
        public string Type { get; set; }

        public AlertModel(string message, string type)
        {
            Message = message;
            Type = type;
        }
    }
}
