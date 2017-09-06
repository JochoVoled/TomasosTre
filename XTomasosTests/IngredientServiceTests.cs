using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TomasosTre.Data;
using TomasosTre.Models;
using TomasosTre.Services;
using Xunit;

namespace XTomasosTests
{
    public class IngredientServiceTests : BaseTests
    {
        private readonly IServiceProvider _serviceProvider;

        public IngredientServiceTests()
        {
            var efServiceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(b =>
                b.UseInMemoryDatabase("DefaultConnection").UseInternalServiceProvider(efServiceProvider));
            services.AddTransient<IngredientService>();

            _serviceProvider = services.BuildServiceProvider();

            InitDb();
        }

        public override void InitDb()
        {
            base.InitDb();
            var context = _serviceProvider.GetService<ApplicationDbContext>();
            context.Ingredients.Add(new Ingredient { Name = "Banana", Price = 2 });
            context.Ingredients.Add(new Ingredient { Name = "Apple",  Price = 5 });

            context.SaveChanges();
        }

        [Fact]
        public void All_Are_Sorted()
        {
            var ingredients = _serviceProvider.GetService<IngredientService>();
            var ings = ingredients.All();
            Assert.Equal(2, ings.Count);
            Assert.Equal("Apple", ings[0].Name);
            Assert.Equal("Banana",ings[1].Name);
        }
    }
}
