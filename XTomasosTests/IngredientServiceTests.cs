using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TomasosTre.Data;
using TomasosTre.Services;
using Xunit;

namespace XTomasosTests
{
    public class IngredientServiceTests : BaseTests
    {
        private readonly IServiceProvider _serviceProvider;

        public IngredientServiceTests()
        {
            var efServiceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

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

        [Fact]
        public void IST_All_Are_Sorted()
        {
            Assert.Equal(2, 0);
        }
    }
}
