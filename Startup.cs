using DF_EvolutionAPI.Services;
using DF_EvolutionAPI.Services.Designations;
using DF_EvolutionAPI.Services.History;
using DF_EvolutionAPI.Services.KRA;
using DF_EvolutionAPI.Services.Login;
using DF_EvolutionAPI.Services.RolesMapping;
using DF_EvolutionAPI.Services.Submission;
using DF_EvolutionAPI.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace DF_EvolutionAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private void LoadConfiguration()
        {
            Constant.CONNECTION_STRING = Configuration["DB:ConnectionString"];

            Constant.AZURE_DOMAIN = Configuration["Azure:Domain"];
            Constant.AZURE_INSTANCE = Configuration["Azure:Instance"];
            Constant.AZURE_CLIENT_ID = Configuration["Azure:ClientId"];
            Constant.AZURE_TENANT_ID = Configuration["Azure:TenantId"];
            Constant.AZURE_CALLBACK_PATH = Configuration["Azure:CallbackPath"];
            Constant.AZURE_STORAGE_CONNECTION_STRING = Configuration["Azure:StorageConnectionString"];
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            LoadConfiguration();

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"));

            services.AddDbContext<DFEvolutionDBContext>(x => x.UseSqlServer(Constant.CONNECTION_STRING));

            //PRMS Master Tables Services
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IResourceService, ResourceService>();
            services.AddScoped<IBusinessUnitService, BusinessUnitService>();
            services.AddScoped<IProjectResourceService, ProjectResourceService>();
            services.AddScoped<IResourceFunctionService, ResourceFunctionService>();

            //DF Evolution tables
            services.AddScoped<IRolesService, RolesService>();
            services.AddScoped<IStatusService, StatusService>();
            services.AddScoped<IQuarterService, QuarterService>();
            services.AddScoped<IUserKRAService, UserKRAService>();
            services.AddScoped<IKRALibraryService, KRALibraryService>();
            services.AddScoped<IDesignationService, DesignationService>();
            services.AddScoped<IRoleMappingService, RoleMappingService>();
            services.AddScoped<IKRAWeightageService, KRAWeightageService>();
            services.AddScoped<IUserApprovalService, UserApprovalService>();
            services.AddScoped<ISubmissionStatusService, SubmissionStatusService>();
            services.AddScoped<IAppraisalHistoryService, AppraisalHistoryService>();

            services.AddIdentity<IdentityUser, IdentityRole>(
                   option =>
                   {
                       option.Password.RequireDigit = false;
                       option.Password.RequiredLength = 6;
                       option.Password.RequireNonAlphanumeric = false;
                       option.Password.RequireUppercase = false;
                       option.Password.RequireLowercase = false;
                   }
               ).AddEntityFrameworkStores<DFEvolutionDBContext>()
               .AddDefaultTokenProviders();

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Site"],
                    ValidIssuer = Configuration["Jwt:Site"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SigningKey"]))
                };
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder
                    .WithOrigins(
                        "https://delightful-beach-0135c210f.4.azurestaticapps.net",
                        "http://localhost:4200",
                        "https://login.windows.net"
                     )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithMethods("PUT", "DELETE", "GET", "POST");
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DF_EvolutionAPI", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      new string[] { }
                    }
                  });
            });

            services.AddControllers();

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DF_EvolutionAPI", Version = "v1" });
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DF_EvolutionAPI v1");
               } ) ;
            }

            app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthentication();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("/index.html");
            });
        }


    }
}
