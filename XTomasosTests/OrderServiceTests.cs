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
            _context.Dishes.Add(new Dish { Name = "Hawaii", Price = 85 });
            _context.Ingredients.AddRange(
                new Ingredient { Name = "Pineapple", Price = 10m },
                new Ingredient { Name = "Ham", Price = 5m });
            _context.SaveChanges();
            _context.Orders.Add(new Order { Date = new DateTime(2017,09,15), IsDelivered = false });
            _context.SaveChanges();
            _context.OrderRows.Add(new OrderRow { DishId = 1, Amount = 1, OrderId = 1 });
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
            Assert.Equal(order.Id, 2);
        }

        [Fact]
        public void Can_Create_OrderRow()
        {
            // Assemble
            ApplicationDbContext _context = ServiceProvider.GetService<ApplicationDbContext>();
            OrderService _order = ServiceProvider.GetService<OrderService>();
            // Act
            OrderRow newOrderRow = _order.CreateOrderRow(1, 1, 1);
            // Assert
            Assert.Equal(newOrderRow.OrderRowId, 2);
        }

        [Fact]
        public void Can_Create_OrderRowIngredient()
        {
            // Assemble
            ApplicationDbContext _context = ServiceProvider.GetService<ApplicationDbContext>();
            OrderService _order = ServiceProvider.GetService<OrderService>();
            // Act
            var extra = _order.CreateOrderRowIngredient(1, 1);
            // Assert
            Assert.Equal(extra.IngredientId, 1);
            Assert.Equal(extra.OrderRowId, 1);
        }

        [Fact]
        public void Are_Order_Tables_Integrated()
        {
            // Assemble
            ApplicationDbContext _context = ServiceProvider.GetService<ApplicationDbContext>();
            ApplicationUser customer = _context.ApplicationUsers.First(x => x.Name == "test@example.com");
            OrderService _order = ServiceProvider.GetService<OrderService>();
            // Act
            Order order = _order.CreateOrder(customer);
            OrderRow newOrderRow = _order.CreateOrderRow(1, order.Id, 1);
            OrderRowIngredient extra = _order.CreateOrderRowIngredient(newOrderRow.OrderRowId, 1);
            // Assert
            Assert.Equal(extra.OrderRowId, newOrderRow.OrderRowId);
            Assert.Equal(newOrderRow.OrderId, order.Id);
        }
    }
}
