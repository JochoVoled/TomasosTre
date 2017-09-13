using System;
using Microsoft.Extensions.DependencyInjection;
using TomasosTre.Data;
using TomasosTre.Models;
using TomasosTre.Services;
using Xunit;
using System.Linq;

namespace XTomasosTests
{
    public class OrderServiceTests: BaseTests
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
            _context.SaveChanges();
        }

        [Fact]
        public void Can_Create_Order()
        {
            // Assemble
            ApplicationDbContext _context = ServiceProvider.GetService<ApplicationDbContext>();
            ApplicationUser customer = _context.ApplicationUsers.First(x => x.Name == "test@example.com");
            OrderService _order = ServiceProvider.GetService<OrderService>();
            // Act
            Order order = _order.CreateOrder(customer);
            // Assert
            
            Assert.Equal(order.Id, 1);
        }

        [Fact]
        public void Can_Create_OrderRow()
        {
            // Assemble
            ApplicationDbContext _context = ServiceProvider.GetService<ApplicationDbContext>();
            OrderService _order = ServiceProvider.GetService<OrderService>();
            // Act
            // Assert
        }

        [Fact]
        public void Can_Create_OrderRowIngredient()
        {
            // Assemble
            ApplicationDbContext _context = ServiceProvider.GetService<ApplicationDbContext>();
            OrderService _order = ServiceProvider.GetService<OrderService>();
            // Act
            // Assert
        }

        [Fact]
        public void Are_Order_Tables_Integrated()
        {
            // Assemble
            ApplicationDbContext _context = ServiceProvider.GetService<ApplicationDbContext>();
            OrderService _order = ServiceProvider.GetService<OrderService>();
            // Act
            // Assert
        }
    }
}
