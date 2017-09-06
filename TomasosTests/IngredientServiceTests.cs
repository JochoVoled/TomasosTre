using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TomasosTre.Data;
using TomasosTre.Services;

namespace TomasosTests
{
    public class IngredientServiceTests : BaseTest
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
        }

        [TestMethod]
        public void Unit_All_Are_Sorted()
        {
            Assert.Equals(2, 0);
        }
    }
}
