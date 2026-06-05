using DF_EvolutionAPI.Services;
using DF_EvolutionAPI.Services.Designations;
using DF_EvolutionAPI.Services.History;
using DF_EvolutionAPI.Services.KRA;
using DF_EvolutionAPI.Services.Email;
using DF_EvolutionAPI.Services.Login;
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
using Microsoft.OpenApi;
using System.Collections.Generic;
using System.Text;
using DF_EvolutionAPI.Services.KRATemplate;
using DF_PA_API.Services;
using DF_PA_API.Services.RolesMaster;
using DF_PA_API.Services.DesignatedRoles;
using System.Linq;

namespace DF_EvolutionAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        private void LoadConfiguration()
        {
            Constant.CONNECTION_STRING = Configuration["DB:ConnectionString"] ?? string.Empty;

            Constant.SMTP_HOST = Configuration["MAIL:SMTP_HOST"] ?? string.Empty;
            Constant.SMTP_PASSWORD = Configuration["MAIL:SMTP_PASSWORD"] ?? string.Empty;
            Constant.SMTP_USERNAME = Configuration["MAIL:SMTP_USERNAME"] ?? string.Empty;
            Constant.SMTP_PORT = int.TryParse(Configuration["MAIL:SMTP_PORT"], out var smtpPort)
                ? smtpPort
                : Constant.SMTP_PORT;

            var noMailDesignation = Configuration["NO_MAIL_DESIGNATION"];
            Constant.NO_MAIL_DESIGNATION = string.IsNullOrWhiteSpace(noMailDesignation)
                ? new List<string>()
                : noMailDesignation
                    .Split(',')
                    .Select(designationName => designationName.Trim())
                    .Where(designationName => !string.IsNullOrWhiteSpace(designationName))
                    .ToList();

            Constant.AZURE_DOMAIN = Configuration["Azure:Domain"] ?? string.Empty;
            Constant.AZURE_INSTANCE = Configuration["Azure:Instance"] ?? string.Empty;
            Constant.AZURE_CLIENT_ID = Configuration["Azure:ClientId"] ?? string.Empty;
            Constant.AZURE_TENANT_ID = Configuration["Azure:TenantId"] ?? string.Empty;
            Constant.AZURE_CALLBACK_PATH = Configuration["Azure:CallbackPath"] ?? string.Empty;
            Constant.AZURE_STORAGE_CONNECTION_STRING = Configuration["Azure:StorageConnectionString"] ?? string.Empty;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.Configure<EmailService>(Configuration.GetSection("Mail"));
            LoadConfiguration();

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"));

            services.AddDbContext<DFEvolutionDBContext>(x => x.UseSqlServer(Constant.CONNECTION_STRING));

            services.AddSingleton<FileUtil>();

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
            services.AddScoped<IKRAWeightageService, KRAWeightageService>();
            services.AddScoped<IUserApprovalService, UserApprovalService>();
            services.AddScoped<ISubmissionStatusService, SubmissionStatusService>();
            services.AddScoped<IAppraisalHistoryService, AppraisalHistoryService>();            
            
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IKRATemplateService, KRATemplateService>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<ISubSkillService, SubSkillService>();
            services.AddScoped<IResourceSkillService, ResourceSkillService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IRolesMasterService, RolesMasterService>();
            services.AddScoped<IDesignatedRoleService, DesignatedRoleService>();
            


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
                options.RequireHttpsMetadata = !_env.IsDevelopment();
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Site"],
                    ValidIssuer = Configuration["Jwt:Site"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SigningKey"]))
                };
            });

            //Enforce Authorization Globally
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = options.DefaultPolicy;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                builder =>
                {
                    // Read the CORS origins from the environment variable
                    var corsOrigins = Configuration["Cors:Origins"]?.Split(',') ?? new string[] { };

                    builder
                        .WithOrigins(corsOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DF Performance Accelerator API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
                });
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env )

        {
           
            app.UseCors("CorsPolicy");

            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
                    
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DF Performance Accelerator API");
                });
            }

            if (!_env.IsDevelopment())
                app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("/index.html");
            });
        }


    }
}
