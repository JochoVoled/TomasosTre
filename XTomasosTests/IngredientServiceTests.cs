using System;
using Microsoft.Extensions.DependencyInjection;
using TomasosTre.Data;
using TomasosTre.Models;
using TomasosTre.Services;
using Xunit;

namespace XTomasosTests
{
    public class IngredientServiceTests : BaseTests
    {
        private IServiceProvider ServiceProvider { get; set; }

        public override void InitDb()
        {
            base.InitDb();
            ServiceProvider = Services.BuildServiceProvider();
            ApplicationDbContext context = ServiceProvider.GetService<ApplicationDbContext>();
            context.Ingredients.Add(new Ingredient { Name = "Banana", Price = 2 });
            context.Ingredients.Add(new Ingredient { Name = "Apple",  Price = 5 });

            context.SaveChanges();
        }

        [Fact]
        public void All_Are_Sorted()
        {
            var ingredients = ServiceProvider.GetService<IngredientService>();
            var ings = ingredients.All();
            Assert.Equal(2, ings.Count);
            Assert.Equal("Apple", ings[0].Name);
            Assert.Equal("Banana",ings[1].Name);
        }
    }
}
