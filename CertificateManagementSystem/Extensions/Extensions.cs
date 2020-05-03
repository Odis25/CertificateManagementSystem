using CertificateManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CertificateManagementSystem.Extensions
{
    public static class Extensions
    {
        private const string AlertKey = "CurrentAlert";

        public static void AddAlertSuccess(this Controller controller, string message)
        {
            var alerts = GetAlerts(controller);

            alerts.Add(new AlertModel(message, "alert-success"));
            controller.TempData[AlertKey] = JsonConvert.SerializeObject(alerts);
        }

        public static void AddAlertInfo(this Controller controller, string message)
        {
            var alerts = GetAlerts(controller);

            alerts.Add(new AlertModel(message, "alert-info"));
            controller.TempData[AlertKey] = JsonConvert.SerializeObject(alerts);
        }

        public static void AddAlertWarning(this Controller controller, string message)
        {
            var alerts = GetAlerts(controller);

            alerts.Add(new AlertModel(message, "alert-warning"));
            controller.TempData[AlertKey] = JsonConvert.SerializeObject(alerts);
        }

        public static void AddAlertDanger(this Controller controller, string message)
        {
            var alerts = GetAlerts(controller);

            alerts.Add(new AlertModel(message, "alert-danger"));
            controller.TempData[AlertKey] = JsonConvert.SerializeObject(alerts);
        }

        private static ICollection<AlertModel> GetAlerts(Controller controller)
        {
            if (controller.TempData[AlertKey] == null)
                controller.TempData[AlertKey] = JsonConvert.SerializeObject(new HashSet<AlertModel>());

            var alerts = JsonConvert.DeserializeObject<ICollection<AlertModel>>(controller.TempData[AlertKey].ToString());

            if (alerts == null)
            {
                alerts = new HashSet<AlertModel>();
            }

            return alerts;
        }
    }
}
