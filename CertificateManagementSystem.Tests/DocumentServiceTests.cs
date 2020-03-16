using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;

namespace CertificateManagementSystem.Tests
{
    [TestFixture]
    public class Document_Service_Sholuld
    {
        [Test]
        public void Return_Results_Corresponding_To_Querry() 
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Document_Database").Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Clients.Add(new Client
                {
                    Id = 1,
                    Name = "Client1",
                    ExploitationPlace = "Ep1"
                });

                context.Contracts.Add(new Contract
                {
                    Id = 1,
                    ContractNumber = "Contract1",
                    Year = 2019,
                    Client = context.Clients.Find(1)
                });

                context.Devices.Add(new Device
                {
                    Id =1,
                    Contract = context.Contracts.Find(1),
                    Name = "Device1",
                    SerialNumber = "SerialNumber1",
                    Type = "Type1",
                    VerificationMethodic = null
                });
            }
        }
    }
}
