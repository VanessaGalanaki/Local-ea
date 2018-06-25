using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project_2_Common.Data;
using Project_2_Common.Models;
using Project_2_Common.Services;

//using Microsoft.AspNetCore.SignalR;
using Project_2_Common.Infrastructure;
using Project_2_Common.Data.Interfaces;
using Project_2_Common.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace Project_2_Common
{
    public class Startup
    {
		public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            
			services.AddIdentity<ApplicationUser, IdentityRole>(options=>{
				options.User.RequireUniqueEmail = true;
			})
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

			services.AddTransient<IUserRepo, UserRepo>();
			services.AddTransient<IStoreMessages, MessageRepository>();
			services.AddTransient<IStoreRatings, RatingRepository>();

            services.AddMvc();
			services.AddSignalR(o => 
			{
				o.EnableDetailedErrors = true;
			});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
               // app.UseDatabaseErrorPage();
            }
            app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");

            app.UseStaticFiles();

            app.UseAuthentication();
   
			app.UseSignalR(routes =>
            {
				routes.MapHub<ChatHub>("/chatHub");
            });
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            

			//try
            //{
            //    CreateUserRoles(services).Wait();
            //}
            //catch (Exception)
            //{
            //    throw new AggregateException("FATAL ERROR!!! - Just kidding, Try again, sth went slow..");

            //}
			//CreateUserRoles(services).Wait();

        }


		private async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            IdentityResult roleResult;

            // Adding Roles
            //
            //
            // Admin 
            var roleCheckAdmin = await RoleManager.RoleExistsAsync("Admin");
			if (!roleCheckAdmin)
            {
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin"));
            }
            //
            //
			// God
            var roleCheckGod = await RoleManager.RoleExistsAsync("God");
			if (!roleCheckGod)
            {
                roleResult = await RoleManager.CreateAsync(new IdentityRole("God"));
            }         
            //
            //
			// locale
            var roleCheckLocale = await RoleManager.RoleExistsAsync("Locale");
			if (!roleCheckLocale)
            {
                //create the roles and seed them to the database
				roleResult = await RoleManager.CreateAsync(new IdentityRole("Locale"));
            }
			// User
            var roleCheckUser = await RoleManager.RoleExistsAsync("User");
			if (!roleCheckUser)
            {
                //create the roles and seed them to the database
				roleResult = await RoleManager.CreateAsync(new IdentityRole("User"));
            }


            // Check if admin users exist and add them to roles.
			var userMagenta = await UserManager.FindByEmailAsync("magentapowa@gmail.com");
			if (userMagenta == null)
			{ 
				userMagenta = new ApplicationUser { UserName = "magentapowa@gmail.com", Email = "magentapowa@gmail.com", UserNameStr = "magentapowa" };
				await UserManager.CreateAsync(userMagenta, "22510sA!");
			}
			await UserManager.AddToRoleAsync(userMagenta, "Admin");

			var userSouko = await UserManager.FindByEmailAsync("van@mail.com");
			if (userSouko == null)
            {
				userSouko = new ApplicationUser { UserName = "van@mail.com", Email = "van@mail.com", UserNameStr = "van@mail.com" };
				await UserManager.CreateAsync(userSouko, "22510sA!");
            }
			await UserManager.AddToRoleAsync(userSouko, "Admin");

			var userVasiliki = await UserManager.FindByEmailAsync("aellacrescent@gmail.com");
			if (userVasiliki == null)
            {
				userVasiliki = new ApplicationUser { UserName = "aellacrescent@gmail.com", Email = "aellacrescent@gmail.com", UserNameStr = "aellacrescent@gmail.com" };
				await UserManager.CreateAsync(userVasiliki, "22510sA!");
            }
			await UserManager.AddToRoleAsync(userVasiliki, "Admin");

			var userMikeTheBoss = await UserManager.FindByEmailAsync("ds@ds.ds");
			if (userMikeTheBoss == null)
            {
				userMikeTheBoss = new ApplicationUser { UserName = "ds@ds.ds", Email = "ds@ds.ds", UserNameStr = "ds@ds.ds" };
				await UserManager.CreateAsync(userMikeTheBoss, "22510sA!");
            }
			await UserManager.AddToRoleAsync(userMikeTheBoss, "Admin");

			var userGod = await UserManager.FindByEmailAsync("god@gmail.com");
            if (userGod == null)
            {
                userGod = new ApplicationUser { UserName = "god@gmail.com", Email = "god@gmail.com", UserNameStr = "God" };
                var result = await UserManager.CreateAsync(userGod, "God!1234");            
            }
            await UserManager.AddToRoleAsync(userGod, "God");


        }
    }
}
