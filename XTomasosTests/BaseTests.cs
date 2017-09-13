using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TomasosTre.Data;
using TomasosTre.Services;

namespace XTomasosTests
{
    public abstract class BaseTests
    {
        public ServiceCollection Services { get; set; } = new ServiceCollection();

        protected BaseTests()
        {
            ServiceProvider efServiceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            Services.AddDbContext<ApplicationDbContext>(b =>
                b.UseInMemoryDatabase("DefaultConnection").
                UseInternalServiceProvider(efServiceProvider));
            Services.AddTransient<IngredientService>();
            Services.AddTransient<AddressService>();
            Services.AddTransient<OrderService>();

            InitDb();
        }

        public virtual void InitDb()
        {

        }
    }
}
