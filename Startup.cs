using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using FantasyWealth.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using FantasyWealth.Utilities;
using FantasyWealth.Controllers;

namespace FantasyWealth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<StockMasterDbContext>(options =>
                            options.UseSqlServer(
                                Configuration.GetConnectionString("Default")));
            services.AddTransient<Repository, Repository>();
            services.AddTransient<iexTrading, iexTrading>();

            //Uncomment option to set simple password such as password
            services.AddIdentity<User, IdentityRole>(/* options =>
                 {
                     // Password settings
                     options.Password.RequireDigit = false;
                     options.Password.RequiredLength = 8;
                     options.Password.RequiredUniqueChars = 2;
                     options.Password.RequireLowercase = true;
                     options.Password.RequireNonAlphanumeric = false;
                     options.Password.RequireUppercase = false;
                 }*/ )
                .AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<StockMasterDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddTransient<TradeHelperService>();
            services.AddSignalR();


        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseCookiePolicy();
            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/hubs/chat");
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            CreateUserRoles(services).Wait();

        }

        private async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<User>>();

            IdentityResult roleResult;
            //Adding Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck)
            {
                //create the roles and seed them to the database
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin"));
            }
            //Assign Admin role to the main User here we have given our newly registered 
            //login id for Admin management
            User user = await UserManager.FindByEmailAsync("nkaramousadakis@outlook.com");
            if (user is null)
            {
                user = new User
                {
                    UserName = "foivos",
                    FirstName = "Nick",
                    LastName = "Karamousadakis",
                    Email = "fkalfopoulos@hotmail.com",                   
                };
                var result = await UserManager.CreateAsync(user, "asdQWE123!@#");
            }
            await UserManager.AddToRoleAsync(user, "Admin");
        }
    }
} 

