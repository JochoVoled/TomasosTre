using System;
using Microsoft.Extensions.DependencyInjection;
using TomasosTre.Data;
using TomasosTre.Models;
using TomasosTre.Services;
using Xunit;
using System.Linq;

namespace XTomasosTests
{
    public class AddressServiceTests: BaseTests
    {
        private IServiceProvider ServiceProvider { get; set; }

        public override void InitDb()
        {
            base.InitDb();
            ServiceProvider = Services.BuildServiceProvider();
            var _context = ServiceProvider.GetService<ApplicationDbContext>();
            _context.ApplicationUsers.Add(new ApplicationUser
            {
                Name = "test@example.com",
                Email = "test@example.com",
            });
            _context.Addresses.Add(new Address
            {
                Street = "Tester St 1",
                Zip = 12345,
                City = "Testington"
            });

            _context.SaveChanges();
        }

        [Fact]
        public void Can_Create()
        {
            // Assemble
            Address address = new Address
            {
                Street = "Tester St 2",
                Zip = 12312,
                City = "Testington"
            };
            ApplicationDbContext _context = ServiceProvider.GetService<ApplicationDbContext>();
            ApplicationUser customer = _context.ApplicationUsers.First(x => x.Name == "test@example.com");
            AddressService _address = ServiceProvider.GetService<AddressService>();

            // Act
            _address.Create(address.Street, address.Zip, address.City, customer.Id);
            // Assert
            Assert.Equal("Tester St 2", address.Street);
        }

        [Fact]
        public void Can_Read_AddressId()
        {
            // Assemble
            AddressService _address = ServiceProvider.GetService<AddressService>();
            int id = 1;
            // Act
            Address address = _address.Read(id);
            // Assert
            Assert.Equal("Tester St 1", address.Street);
        }

        [Fact]
        public void Can_Read_atDate()
        {
            // Assemble
            ApplicationDbContext _context = ServiceProvider.GetService<ApplicationDbContext>();
            ApplicationUser customer = _context.ApplicationUsers.First(x => x.Name == "test@example.com");

            AddressService _address = ServiceProvider.GetService<AddressService>();

            _context.Addresses.AddRange(new Address
            {
                Street = "Memory lane 5",
                CustomerId = customer.Id,
                StartDateTime = new DateTime(2000,1,1),
                EndDateTime = new DateTime(2010, 1, 1)
            }, new Address
            {
                Street = "New street 1",
                CustomerId = customer.Id,
                StartDateTime = new DateTime(2010, 1, 1),
            });
            _context.SaveChanges();

            // Act
            Address current = _address.Read(customer.Id);
            Address arhived = _address.Read(customer.Id, new DateTime(2005,01,01));
            // Assert
            Assert.Equal("New street 1", current.Street);
            Assert.Equal("Memory lane 5", arhived.Street);
        }

        [Fact]
        public void Can_Update()
        {
            // Assemble
            AddressService _address = ServiceProvider.GetService<AddressService>();
            ApplicationDbContext _context = ServiceProvider.GetService<ApplicationDbContext>();
            Address oldEntry = _context.Addresses.Find(1);
            
            _address.Create("My New Address", 54332, "Testyburgh");
            Address newEntry = _context.Addresses.First(x => x.Street == "My New Address");

            ApplicationUser customer = _context.ApplicationUsers.First(x => x.Name == "test@example.com");
            newEntry.CustomerId = customer.Id;

            // Act
            _address.Update(oldEntry.AddressId, newEntry);
            // Assert
            Assert.NotEqual(oldEntry.EndDateTime, null);
            Assert.Equal(newEntry.AddressId, 2);
        }

        [Fact]
        public void Can_Delete()
        {
            // Assemble
            AddressService _address = ServiceProvider.GetService<AddressService>();
            ApplicationDbContext _context = ServiceProvider.GetService<ApplicationDbContext>();
            Address address = _context.Addresses.Find(1);
            // Act
            _address.Delete(address.AddressId);
            // Assert
            Assert.Equal(_context.Addresses.Find(1) , null);
        }
    }
}
