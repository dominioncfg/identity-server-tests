using Microsoft.EntityFrameworkCore;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using System.Linq;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using System;
using TestIdentityServer.Model;
using TestIdentityServer.Infra;

namespace TestIdentityServer
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            const string connectionString = "Data Source=sqlserver;Database=TestIdentitySever;User Id=sa;Password=PasswordO1.;";
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<QvaCarUsersDBContext>(o =>
            {
                string userCS = connectionString;
                o.UseSqlServer(userCS, sql => sql.MigrationsAssembly(migrationsAssembly));
            });

            services
               .AddIdentity<QvaCarIdentityUser, QvaCarIdentityRole>(options =>
               {
                   options.Password.RequiredLength = 6;
                   options.Password.RequireDigit = true;
                   options.Password.RequireNonAlphanumeric = true;

                   options.Lockout.MaxFailedAccessAttempts = 3;
                   options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

                   options.SignIn.RequireConfirmedEmail = true;
                   options.User.RequireUniqueEmail = true;
               })
               .AddEntityFrameworkStores<QvaCarUsersDBContext>()
               .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2));


            //Identity Server
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                options.EmitStaticAudienceClaim = true;
            })
            .AddAspNetIdentity<QvaCarIdentityUser>()
            .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                }
            )
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = 30;
            });



            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            services.AddAuthorization(authorizationOptions =>
            {
                //authorizationOptions.AddPolicy(
                //    "ApiAuth",
                //    policyBuilder =>
                //    {
                //        policyBuilder.AuthenticationSchemes.Add(IdentityServerAuthenticationDefaults.AuthenticationScheme);
                //        policyBuilder.RequireAuthenticatedUser();

                //    });
            });

            var hostURl = System.Environment.GetEnvironmentVariable("HOST_URL");
            services.AddAuthentication()
            .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Authority = hostURl;
                options.ApiName = "qvacar.api.core";
                options.JwtBackChannelHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                             (message, certificate, chain, sslPolicyErrors) => true
                };
                //options.ApiSecret = "apisecret";
                // options.BackchannelHttpHandler = null;// new System.Net.Http.HttpClientHandler { ServerCertificateCustomValidationCallback = delegate { return true; } };
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            InitializeDatabase(app);
            IdentitySeedData.CreateIdentitySeedData(app);
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in Config.GetApiScopes())
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }


            }
        }
    }
}