using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TomasosTre.Data;
using TomasosTre.Models;
using TomasosTre.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.AzureAppServices;

namespace TomasosTre
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            switch (HostingEnvironment.EnvironmentName)
            {
                case Constants.DEVELOPMENT_ENVIRONMENT:
                    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("LocalConnection")));
                    break;
                case Constants.STAGED_ENVIRONMENT:
                    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AzureConnection")));
                    //services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
                    break;
                case Constants.PRODUCTION_ENVIRONMENT:
                    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AzureConnection")));
                    break;
                default:
                    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("LocalConnection")));
                    break;
            }

            //services.Configure<IISOptions>(options => options.UseInMemoryDatabase("DefaultConnection"));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<SessionService>();
            services.AddTransient<OrderService>();
            services.AddTransient<OrderRowIngredientService>();
            services.AddTransient<IngredientService>();
            services.AddTransient<UserStateService>();
            services.AddTransient<DishIngredientService>();
            services.AddTransient<DishService>();
            services.AddTransient<AddressService>();
            services.AddMvc();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILoggerFactory loggerFactory )
        {
            if (env.IsProduction())
                loggerFactory.AddAzureWebAppDiagnostics(
                 new AzureAppServicesDiagnosticsSettings
                 {
                     OutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss zzz} [{Level}] {RequestId}-{SourceContext}: {Message}{NewLine}{Exception}"
                 });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Render/Error");
            }

            app.UseSession();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Render}/{action=Index}/{id?}");
            });

            if (HostingEnvironment.IsProduction() || HostingEnvironment.IsStaging())
            {
                context.Database.Migrate();
            }

            DataSetup.Setup(context, userManager,roleManager).Wait();

        }
    }
}
