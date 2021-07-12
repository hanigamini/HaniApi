using System;
using HaniApi.Areas.Identity.Data;
using HaniApi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(HaniApi.Areas.Identity.IdentityHostingStartup))]
namespace HaniApi.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<TestContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("TestContextConnection")));

                services.AddDefaultIdentity<HaniUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<TestContext>();
            });
        }
    }
}