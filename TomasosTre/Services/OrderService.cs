using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using TomasosTre.Data;
using TomasosTre.Extensions;
using TomasosTre.Models;
using TomasosTre.ViewModels;

namespace TomasosTre.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly SessionService _session;

        public OrderService(ApplicationDbContext context, SessionService session)
        {
            _context = context;
            _session = session;
        }

        public Order SetupNewOrder(CheckoutViewModel checkout, ApplicationUser User, HttpContext HttpContext)
        {
            //DateTime expires = checkout.ExpiryMonth.ToDateTime();

            // get and prepare data
            //var User = UserStateService.GetUser(User);
            var order = new Order
            {
                Date = DateTime.Now,
                IsDelivered = false,
            };
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (User != null)
            {
                order.ApplicationUserId = User.Id;
                order.Customer = User;
            }
            order.OrderRows = _session.LoadOrderRows(HttpContext);
            var ori = _session.LoadOrderRowIngredients(HttpContext);

            // connect all orderRows to new order
            foreach (var x in order.OrderRows)
            {
                x.OrderId = order.Id;
                order.Price += x.Dish.Price * x.Amount;
                var relatedOris = ori.Where(y => y.OrderRowId == x.OrderRowId);
                x.OrderRowIngredient = new List<OrderRowIngredient>();
                x.OrderRowIngredient.AddRange(relatedOris);
            }
            //order.OrderRows.ForEach(x => x.OrderId = order.Id);
            //order.OrderRows.ForEach(x => order.Price += x.Dish.Price * x.Amount);
            ori.ForEach(x => order.Price += x.IsExtra ? x.Ingredient.Price : 0);

            return order;
        }

        public void SaveNewOrder(Order order, List<OrderRow> orderRows, List<OrderRowIngredient> ori)
        {
            // save readied order
            _context.Orders.Add(order);
            _context.OrderRows.AddRange(orderRows);
            /* TODO getting error on SaveChanges: +		$exception	{System.ArgumentException: An item with the same key has already been added. Key: 2
                at System.ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException(Object key)
                at System.Collections.Generic.Dictionary`2.TryInsert(TKey key, TValue value, InsertionBehavior behavior)
                at Microsoft.EntityFrameworkCore.Storage.Internal.InMemoryTable`1.Create(IUpdateEntry entry)
                at Microsoft.EntityFrameworkCore.Storage.Internal.InMemoryStore.ExecuteTransaction(IEnumerable`1 entries, IDiagnosticsLogger`1 updateLogger)
                at Microsoft.EntityFrameworkCore.Storage.Internal.InMemoryDatabase.SaveChanges(IReadOnlyList`1 entries)
                at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChanges(IReadOnlyList`1 entriesToSave)
                at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChanges(Boolean acceptAllChangesOnSuccess)
                at Microsoft.EntityFrameworkCore.DbContext.SaveChanges(Boolean acceptAllChangesOnSuccess)
                at Microsoft.EntityFrameworkCore.DbContext.SaveChanges()
                at TomasosTre.Services.OrderService.SaveNewOrder(Order order, List`1 orderRows, List`1 ori) in C:\Source\Repos\TomasosTre\TomasosTre\Services\OrderService.cs:line 64
                at TomasosTre.Controllers.RenderController.Order(CheckoutViewModel checkout) in C:\Source\Repos\TomasosTre\TomasosTre\Controllers\RenderController.cs:line 161
                at lambda_method(Closure , Object , Object[] )
                at Microsoft.Extensions.Internal.ObjectMethodExecutor.Execute(Object target, Object[] parameters)
                at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.< InvokeActionMethodAsync > d__12.MoveNext()}
            System.ArgumentException
            */
            _context.SaveChanges();

            _context.OrderRowIngredients.AddRange(ori);
            _context.SaveChanges();
        }

        public decimal ModifyPrice(List<Ingredient> added, List<Ingredient> subtracted)
        {
            decimal newPrice = 0m;
            foreach (var item in added)
            {
                newPrice += item.Price;
            }
            foreach (var item in subtracted)
            {
                newPrice -= item.Price;
            }
            return newPrice;
        }
    }
}
