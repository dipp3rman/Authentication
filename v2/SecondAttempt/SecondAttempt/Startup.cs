using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecondAttempt.Data;
using SecondAttempt.Models;
using SecondAttempt.Services;

namespace SecondAttempt
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
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ValidCodeRequirement", policy => policy.Requirements.Add(new ValidCodeRequirement(1234)));
            });

            services.AddSingleton<IAuthorizationHandler, ValidCodeHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    public class ValidCodeHandler : AuthorizationHandler<ValidCodeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ValidCodeRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.SerialNumber && c.Issuer == "http://contoso.com"))
            {
                return Task.CompletedTask;
            }

            var code = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.SerialNumber && c.Issuer == "http://contoso.com").Value);

            if (code == requirement.CurrentCode)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }

    public class ValidCodeRequirement : IAuthorizationRequirement
    {
        public ValidCodeRequirement(int code)
        {
            CurrentCode = code;
        }

        public int CurrentCode { get; private set; }
    }
}