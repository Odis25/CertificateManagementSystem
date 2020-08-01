using AutoMapper;
using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Helpers;
using CertificateManagementSystem.Models.Settings;
using CertificateManagementSystem.Services;
using CertificateManagementSystem.Services.Interfaces;
using CertificateManagementSystem.Services.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace CertificateManagementSystem
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
            services.AddDbContext<ApplicationDbContext>(options
                => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.AddSession();

            services.AddControllersWithViews()
                    .AddRazorRuntimeCompilation();

            services.Configure<SettingsModel>(Configuration.GetSection("Paths"));

            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IViewModelMapper, ViewModelMapper>();
            services.AddScoped<DataSeeder>();

            services.AddAutoMapper(c => c.AddProfile<EntityMappingProfile>(), typeof(Startup));

            var dir = Configuration.GetSection("Paths").GetSection("MethodicsFolder").Value;
            if (Directory.Exists(dir))
                services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Configuration.GetSection("Paths").GetSection("MethodicsFolder").Value));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataSeeder dataSeeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseStaticFiles();
            if (Directory.Exists(Configuration.GetSection("Paths").GetSection("DocumentsFolder").Value))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(Configuration.GetSection("Paths").GetSection("DocumentsFolder").Value),
                    RequestPath = new PathString("/documentsFolder")
                });
            }

            if (Directory.Exists(Configuration.GetSection("Paths").GetSection("MethodicsFolder").Value))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(Configuration.GetSection("Paths").GetSection("MethodicsFolder").Value),
                    RequestPath = new PathString("/methodicsFolder")
                });
            }

            app.UseRouting();

            // Первичное заполнение базы данных
            dataSeeder.SeedDataBase();

            // Аутентификация 
            app.UseAuthentication();

            // Авторизация
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Document}/{action=Index}/{id?}");
            });
        }
    }
}
