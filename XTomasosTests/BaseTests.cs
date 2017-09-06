using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TomasosTre.Data;
using TomasosTre.Services;
using Xunit;

namespace XTomasosTests
{
    public class BaseTests
    {
        public BaseTests()
        {
            var efServiceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(b =>
                b.UseInMemoryDatabase("DefaultConnection").UseInternalServiceProvider(efServiceProvider));
            services.AddTransient<IngredientService>();


            //InitDb();
        }

        public virtual void InitDb()
        {

        }

        //[Fact]
        //public void Base_All_Are_Sorted()
        //{
        //    Assert.Equal(2, 0);
        //}
    }
}
