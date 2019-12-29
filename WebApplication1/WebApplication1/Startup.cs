using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
//using AutoMapper.Extensions.Microsoft.DependencyInjection

namespace WebApplication1
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
            services.AddAutoMapper(typeof(Startup));
            //var config = new AutoMapper.MapperConfiguration(cfg => {
            //    cfg.AddProfile(new AutoMapperProfileConfiguration());
            //});
            //var mapper = config.CreateMapper();
            //services.AddSingleton(mapper);
            //services.AddAutoMapper();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //services.AddSwaggerGen(options =>
            //{
            //    options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
            //    {
            //        Version = "v1",
            //        Title = "DotNet Core WebAPI文档"
            //    });

            //});
            //版本控制
            services.AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");
            services.AddApiVersioning(option =>
            {
                // allow a client to call you without specifying an api version
                // since we haven't configured it otherwise, the assumed api version will be 1.0
                option.AssumeDefaultVersionWhenUnspecified = true;
                option.ReportApiVersions = false;
            });
            //custom swagger
            //services.AddCustomSwagger(CURRENT_SWAGGER_OPTIONS);
            services.AddSwaggerGen(
                options =>
                {
                    // resolve the IApiVersionDescriptionProvider service
                    // note: that we have to build a temporary service provider here because one has not been created yet
                    var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                    // add a swagger document for each discovered API version
                    // note: you might choose to skip or document deprecated API versions differently
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                    }

                    // add a custom operation filter which sets default values
                    options.OperationFilter<SwaggerDefaultValues>();

                    //
                    // integrate xml comments
                    options.IncludeXmlComments(XmlCommentsFilePath);
                    //options.IncludeXmlComments(XmlModelsFilePath);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DotNet Core WebAPI文档");
            //    //c.RoutePrefix = string.Empty; //默认值是 "swagger" ,需要这样请求:https://localhost:44384/swagger
            //});
            //custom swagger
            //自动检测存在的版本
            // CURRENT_SWAGGER_OPTIONS.ApiVersions = provider.ApiVersionDescriptions.Select(s => s.GroupName).ToArray();
            //app.UseCustomSwagger(CURRENT_SWAGGER_OPTIONS);
            //Swagger
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });

        }
        static string XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }

        static string XmlModelsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = "";//typeof(UserInfoModel).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }

        static Info CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new Info()
            {
                Title = $"TB_AspNetCore_Swagger v{description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "多版本管理（点右上角版本切换）<br/>",
                Contact = new Contact() { Name = "TopBrids_1373978075@qq.com" }
            };

            if (description.IsDeprecated)
            {
                info.Description += "<br/><b>TopBrids</b>";
            }

            return info;
        }
        private readonly CustsomSwaggerOptions CURRENT_SWAGGER_OPTIONS = new CustsomSwaggerOptions()
        {
            ProjectName = "墨玄涯博客接口",
            ApiVersions = new string[] { "v1", "v2" },//要显示的版本
            UseCustomIndex = true,
            RoutePrefix = "swagger",
            AddSwaggerGenAction = c =>
            {
                var filePath = System.IO.Path.Combine(System.AppContext.BaseDirectory, typeof(Program).GetTypeInfo().Assembly.GetName().Name + ".xml");
                c.IncludeXmlComments(filePath, true);
            },
            UseSwaggerAction = c =>
            {

            },
            UseSwaggerUIAction = c =>
            {

            }
        };
    }
}
