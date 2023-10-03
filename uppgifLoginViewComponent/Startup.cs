using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using uppgifLoginViewComponent.Areas.Identity.Data;
using uppgifLoginViewComponent.CusomAttributes;
using uppgifLoginViewComponent.Data;
using uppgifLoginViewComponent.Models;

namespace uppgifLoginViewComponent
{
    public class Startup
    {
        //IConfiguration injected via runtime hosting service
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        //public IWebHostEnvironment WebHostEnv { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container. is composed to an IServiceProvider class. for resolving services directly.
        public void ConfigureServices(IServiceCollection services)
        {

            
            
            services.AddControllersWithViews().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

            services.AddDbContext<SchoolContext>(options =>
            {

                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

            });

            services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
                  .AddRoles<IdentityRole>()
                  .AddEntityFrameworkStores<IdentityDbContext>()
             .AddEntityFrameworkStores<IdentityDbContext>()
             .AddRoleManager<RoleManager<IdentityRole>>()
             .AddDefaultUI()
             .AddDefaultTokenProviders();
            
            services.AddRazorPages();

            services.AddScoped<ValidationFilterFileNotEmptyAttribute>();
            //services.AddControllersWithViews();
            

            var mapperconfiguration = new MapperConfiguration(c =>
            {
                c.AddProfile(new MapperProfile());
            });

            IMapper mapper = mapperconfiguration.CreateMapper();
            services.AddSingleton(mapper);
            
           services.AddTransient<StudentInfoHelp>();
           
            
        }
        
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            

            //Data.StartupData.MyWebHostEnv =  env;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
                endpoints.MapRazorPages();
            });
        }
    }
}
