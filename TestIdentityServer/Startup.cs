using Microsoft.EntityFrameworkCore;
using IdentityServer4.AccessTokenValidation;
using IdentityServerHost.Quickstart.UI;
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

            const string connectionString = "Data Source=sqlserver;Initial Catalog=TestIdentitySever;User Id=sa;Password=PasswordO1.;";
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            })
            .AddTestUsers(TestUsers.Users)
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

                // this enables automatic token cleanup. this is optional.
                options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = 30;
            });

            // in-memory, code config
            // builder.AddInMemoryIdentityResources(Config.IdentityResources);
            // this adds the operational data from DB (codes, tokens, consents)

            //builder.AddInMemoryApiResources(Config.ApiResources);
            //builder.AddInMemoryApiScopes(Config.ApiScopes);
            //builder.AddInMemoryClients(Config.Clients);

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

            services.AddAuthentication()
            .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Authority = "https://192.168.1.138:5001";
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