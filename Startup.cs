using System;
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
using DF_EvolutionAPI.Configuration;
using DF_EvolutionAPI.Models.Response;
using Microsoft.Extensions.Options;

namespace DF_EvolutionAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private void LoadConfiguration(
            DbOptions dbOptions,
            MailOptions mailOptions,
            AzureOptions azureOptions,
            AppBehaviorOptions appBehaviorOptions)
        {
            Constant.CONNECTION_STRING = dbOptions.ConnectionString;

            Constant.SMTP_HOST = mailOptions.SMTP_HOST;
            Constant.SMTP_PASSWORD = mailOptions.SMTP_PASSWORD;
            Constant.SMTP_USERNAME = mailOptions.SMTP_USERNAME;
            Constant.SMTP_PORT = mailOptions.SMTP_PORT;
            Constant.NO_MAIL_DESIGNATION = appBehaviorOptions.NoMailDesignation
                .Select(designationName => designationName.Trim())
                .Where(designationName => !string.IsNullOrWhiteSpace(designationName))
                .ToList();

            Constant.AZURE_DOMAIN = azureOptions.Domain;
            Constant.AZURE_INSTANCE = azureOptions.Instance;
            Constant.AZURE_CLIENT_ID = azureOptions.ClientId;
            Constant.AZURE_TENANT_ID = azureOptions.TenantId;
            Constant.AZURE_CALLBACK_PATH = azureOptions.CallbackPath;
            Constant.AZURE_STORAGE_CONNECTION_STRING = azureOptions.StorageConnectionString;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions<DbOptions>()
                .Bind(Configuration.GetRequiredSection("DB"))
                .ValidateDataAnnotations()
                .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), "DB:ConnectionString is required.")
                .ValidateOnStart();

            services.AddOptions<MailOptions>()
                .Bind(Configuration.GetRequiredSection("Mail"))
                .ValidateDataAnnotations()
                .Validate(options => !string.IsNullOrWhiteSpace(options.SMTP_HOST), "MAIL:SMTP_HOST is required.")
                .Validate(options => !string.IsNullOrWhiteSpace(options.SMTP_USERNAME), "MAIL:SMTP_USERNAME is required.")
                .Validate(options => !string.IsNullOrWhiteSpace(options.SMTP_PASSWORD), "MAIL:SMTP_PASSWORD is required.")
                .ValidateOnStart();

            services.AddOptions<AzureOptions>()
                .Bind(Configuration.GetRequiredSection("Azure"))
                .ValidateDataAnnotations()
                .Validate(options => !string.IsNullOrWhiteSpace(options.Instance), "Azure:Instance is required.")
                .Validate(options => !string.IsNullOrWhiteSpace(options.Domain), "Azure:Domain is required.")
                .Validate(options => !string.IsNullOrWhiteSpace(options.TenantId), "Azure:TenantId is required.")
                .Validate(options => !string.IsNullOrWhiteSpace(options.ClientId), "Azure:ClientId is required.")
                .Validate(options => !string.IsNullOrWhiteSpace(options.CallbackPath), "Azure:CallbackPath is required.")
                .Validate(options => !string.IsNullOrWhiteSpace(options.StorageConnectionString), "Azure:StorageConnectionString is required.")
                .ValidateOnStart();

            services.AddOptions<AppBehaviorOptions>()
                .Configure(options =>
                {
                    options.NoMailDesignation = ParseCsvSetting("NO_MAIL_DESIGNATION");
                    options.Origins = ParseCsvSetting("Cors:Origins");
                })
                .Validate(options => options.NoMailDesignation.Length > 0, "NO_MAIL_DESIGNATION must contain at least one value.")
                .Validate(options => options.Origins.Length > 0, "Cors:Origins must contain at least one origin.")
                .Validate(options => options.Origins.All(origin => Uri.TryCreate(origin, UriKind.Absolute, out _)), "Cors:Origins must contain valid absolute origins.")
                .ValidateOnStart();

            services.AddControllersWithViews();
            services.Configure<EmailSetting>(Configuration.GetSection("Mail"));

            var dbOptions = Configuration.GetRequiredSection("DB").Get<DbOptions>()
                ?? throw new InvalidOperationException("DB configuration is missing.");
            var mailOptions = Configuration.GetRequiredSection("Mail").Get<MailOptions>()
                ?? throw new InvalidOperationException("Mail configuration is missing.");
            var azureOptions = Configuration.GetRequiredSection("Azure").Get<AzureOptions>()
                ?? throw new InvalidOperationException("Azure configuration is missing.");
            var appBehaviorOptions = new AppBehaviorOptions
            {
                NoMailDesignation = ParseCsvSetting("NO_MAIL_DESIGNATION"),
                Origins = ParseCsvSetting("Cors:Origins")
            };

            ValidateRequiredSettings(dbOptions, mailOptions, azureOptions, appBehaviorOptions);
            LoadConfiguration(dbOptions, mailOptions, azureOptions, appBehaviorOptions);

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
                    builder
                        .WithOrigins(appBehaviorOptions.Origins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithMethods("PUT", "DELETE", "GET", "POST", "PATCH");
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

        private string[] ParseCsvSetting(string key)
        {
            return (Configuration[key] ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToArray();
        }

        private static void ValidateRequiredSettings(
            DbOptions dbOptions,
            MailOptions mailOptions,
            AzureOptions azureOptions,
            AppBehaviorOptions appBehaviorOptions)
        {
            if (string.IsNullOrWhiteSpace(dbOptions.ConnectionString))
            {
                throw new InvalidOperationException("DB:ConnectionString is required.");
            }

            if (string.IsNullOrWhiteSpace(mailOptions.SMTP_HOST) ||
                string.IsNullOrWhiteSpace(mailOptions.SMTP_USERNAME) ||
                string.IsNullOrWhiteSpace(mailOptions.SMTP_PASSWORD) ||
                mailOptions.SMTP_PORT <= 0)
            {
                throw new InvalidOperationException("MAIL settings are required: SMTP_HOST, SMTP_PORT, SMTP_USERNAME, SMTP_PASSWORD.");
            }

            if (string.IsNullOrWhiteSpace(azureOptions.Instance) ||
                string.IsNullOrWhiteSpace(azureOptions.Domain) ||
                string.IsNullOrWhiteSpace(azureOptions.TenantId) ||
                string.IsNullOrWhiteSpace(azureOptions.ClientId) ||
                string.IsNullOrWhiteSpace(azureOptions.CallbackPath) ||
                string.IsNullOrWhiteSpace(azureOptions.StorageConnectionString))
            {
                throw new InvalidOperationException("Azure settings are required: Instance, Domain, TenantId, ClientId, CallbackPath, StorageConnectionString.");
            }

            if (appBehaviorOptions.NoMailDesignation.Length == 0)
            {
                throw new InvalidOperationException("NO_MAIL_DESIGNATION must contain at least one value.");
            }

            if (appBehaviorOptions.Origins.Length == 0)
            {
                throw new InvalidOperationException("Cors:Origins must contain at least one origin.");
            }
        }


    }
}
