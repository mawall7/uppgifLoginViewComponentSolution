using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using uppgifLoginViewComponent.Areas.Identity.Data;
using uppgifLoginViewComponent.Data;

[assembly: HostingStartup(typeof(uppgifLoginViewComponent.Areas.Identity.IdentityHostingStartup))]
namespace uppgifLoginViewComponent.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<IdentityDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("IdentityDbContextConnection")));

               // services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
               //     .AddEntityFrameworkStores<IdentityDbContext>()
               //     .AddRoles<IdentityRole>()
               //.AddEntityFrameworkStores<IdentityDbContext>()
               //.AddRoleManager<RoleManager<IdentityRole>>()
               ////.AddDefaultUI()
               //.AddDefaultTokenProviders();
            });
        }
    }
}