using DF_EvolutionAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DF_EvolutionAPI.Models.Response;
using DF_PA_API.Models;

namespace DF_EvolutionAPI
{
    public class DFEvolutionDBContext : IdentityDbContext<IdentityUser, IdentityRole, string, IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {

        public DFEvolutionDBContext(DbContextOptions<DFEvolutionDBContext> options)
            : base(options)
        {
            // ChangeTracker.LazyLoadingEnabled = false;
        }

        public new DbSet<Role> Roles { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<UserKRA> UserKRA { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Resource> Resources { get; set; }
        public virtual DbSet<KRALibrary> KRALibrary { get; set; }
        public virtual DbSet<RoleMapping> RoleMapping { get; set; }
        public virtual DbSet<Designation> Designations { get; set; }
        public virtual DbSet<UserApproval> UserApproval { get; set; }
        public virtual DbSet<BusinessUnit> BusinessUnits { get; set; }
        public virtual DbSet<KRAWeightage> KRAWeightages { get; set; }
        public virtual DbSet<StatusLibrary> StatusLibrary { get;  set; }
        public virtual DbSet<QuarterDetails> QuarterDetails { get; set; }
        public virtual DbSet<ProjectResource> ProjectResources { get; set; }
        public virtual DbSet<AppraisalHistory> AppraisalHistory { get; set; }
        public virtual DbSet<SubmissionStatus> SubmissionStatus { get; set; }
        public virtual DbSet<TechFunction> TechFunctions { get; set; }
        public virtual DbSet<RoleMapping> PA_RoleMappings { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<PATemplate> PATemplates { get; set; }
        public virtual DbSet<PATemplateDesignation> PA_TemplateDesignations { get; set; }
        public virtual DbSet<PATemplateKra> PA_TemplateKras { get; set; }
        public virtual DbSet<IdentityRole> AspNetRoles { get; set; }
        public virtual DbSet<IdentityUser> AspNetUsers { get; set; }
        public virtual DbSet<ApplicationUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }
        public virtual DbSet<SubSkill> SubSkills { get; set; }
        public virtual DbSet<ResourceSkill> ResourceSkills { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<RoleMaster> RoleMasters { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<DesignatedRole> DesignatedRoles { get; set; }
        public virtual DbSet<UserQuarterlyAchievement> UserQuarterlyAchievements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<IdentityUser>(entity =>
            {
                entity.ToTable("AspNetUsers","dbo");
                entity.Property(e => e.Id).HasColumnName("Id");
            });

            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable("AspNetRoles","dbo");
                entity.Property(e => e.Id).HasColumnName("Id");
            });

            modelBuilder.Entity<ApplicationUserRole>(entity =>
            {
                entity.ToTable("AspNetUserRoles", "dbo");
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.RoleId).HasColumnName("RoleId");
                entity.Property(e => e.ApplicationName).HasColumnName("ApplicationName");
            });


            modelBuilder.HasDefaultSchema("dbo").Entity<Resource>(e =>
            {
                e.ToTable("Resource");
                e.HasKey(x => x.ResourceId);
                e.Property(e => e.ResourceId).HasColumnName("ResourceId");
                e.Property(e => e.ResourceName).HasColumnName("ResourceName");
                e.Property(e => e.EmployeeId).HasColumnName("EmployeeId");
                e.Property(e => e.EmailId).HasColumnName("EmailId");
                e.Property(e => e.DateOfBirth).HasColumnName("DateOfBirth");
                e.Property(e => e.DateOfJoin).HasColumnName("DateOfJoin");
                e.Property(e => e.ReportingTo).HasColumnName("ReportingTo");
                e.Property(e => e.CountryId).HasColumnName("CountryId");
                e.Property(e => e.CityId).HasColumnName("CityId");
                e.Property(e => e.StateId).HasColumnName("StateId");
                e.Property(e => e.AlternetNumber).HasColumnName("AlternetNumber");
                e.Property(e => e.ContactNumber).HasColumnName("ContactNumber");
                e.Property(e => e.Address).HasColumnName("Address");
                e.Property(e => e.StatusId).HasColumnName("StatusId");
                e.Property(e => e.Primaryskill).HasColumnName("PrimarySkill");
                e.Property(e => e.Secondaryskill).HasColumnName("SecondarySkill");
                e.Property(e => e.Zip).HasColumnName("Zip");
                e.Property(e => e.FunctionId).HasColumnName("FunctionId");
                e.Property(e => e.Strengths).HasColumnName("Strengths");
                e.Property(e => e.TechCategoryId).HasColumnName("TechCategoryId");
                e.Property(e => e.DesignationId).HasColumnName("DesignationId");
                e.Property(e => e.YearBucket).HasColumnName("YearBucket");
                e.Property(e => e.TenureInMonths).HasColumnName("TenureInMonths");
                e.Property(e => e.TenureInYears).HasColumnName("TenureInYears");
                e.Property(e => e.TotalYears).HasColumnName("TotalYears");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
                e.Property(e => e.DesignatedRoleId).HasColumnName("DesignatedRoleId");
                e.Property(e => e.BusinessUnitId).HasColumnName("BusinessUnitId");
                e.Ignore(e => e.ProjectList);
                e.Ignore(e => e.ResourceProjectList);
                e.Ignore(e => e.ClientList);
            });

            modelBuilder.HasDefaultSchema("dbo").Entity<ProjectResource>(e =>
            {
                e.ToTable("ProjectResource");
                e.HasKey(x => x.ProjectResourceId);
                e.Property(e => e.ProjectResourceId).HasColumnName("ProjectResourceId");
                e.Property(e => e.ResourceId).HasColumnName("ResourceId");
                e.Property(e => e.ProjectId).HasColumnName("ProjectId");
                e.Property(e => e.Billable).HasColumnName("Billable");
                e.Property(e => e.BillingCycleId).HasColumnName("BillingCycleId");
                e.Property(e => e.Shadow).HasColumnName("Shadow");
                e.Property(e => e.Remark).HasColumnName("Remark");
                e.Property(e => e.ResourceRole).HasColumnName("ResourceRole");
                e.Property(e => e.StartDate).HasColumnName("StartDate");
                e.Property(e => e.EndDate).HasColumnName("EndDate");
                e.Property(e => e.CurrencyId).HasColumnName("CurrencyId");
                e.Property(e => e.PercentageAllocation).HasColumnName("PercentageAllocation");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
                e.Property(e => e.AssignmentDate).HasColumnName("AssignmentDate");
                e.Ignore(e => e.ProjectList);

            });

            modelBuilder.HasDefaultSchema("dbo").Entity<Project>(e =>
            {
                e.ToTable("Project");
                e.HasKey(x => x.ProjectId);
                e.Property(e => e.ProjectId).HasColumnName("ProjectId");
                e.Property(e => e.ClientId).HasColumnName("ClientId");
                e.Property(e => e.ProjectLeadId).HasColumnName("ProjectLeadId");
                e.Property(e => e.ProjectManagerId).HasColumnName("ProjectManagerId");
                e.Property(e => e.ProjectName).HasColumnName("ProjectName");
                e.Property(e => e.Description).HasColumnName("Description");
                e.Property(e => e.CategoryId).HasColumnName("CategoryId");
                e.Property(e => e.Duration).HasColumnName("Duration");
                e.Property(e => e.StartDate).HasColumnName("StartDate");
                e.Property(e => e.EndDate).HasColumnName("EndDate");
                e.Property(e => e.HourlyBillingRate).HasColumnName("HourlyBillingRate");
                e.Property(e => e.MonthlyBillingRate).HasColumnName("MonthlyBillingRate");
                e.Property(e => e.NumberOfResorces).HasColumnName("NumberOfResorces");
                e.Property(e => e.ProjectStatusId).HasColumnName("ProjectStatusId");
                e.Property(e => e.ProjectSubTypeId).HasColumnName("ProjectSubTypeId");
                e.Property(e => e.ProjectTypeId).HasColumnName("ProjectTypeId");
                e.Property(e => e.ReviewCycleId).HasColumnName("ReviewCycleId");
                e.Property(e => e.WeeklyBillingRate).HasColumnName("WeeklyBillingRate");
                e.Property(e => e.ActualEndDate).HasColumnName("ActualEndDate");
                e.Property(e => e.ActualStartDate).HasColumnName("ActualStartDate");
            });

            modelBuilder.HasDefaultSchema("dbo").Entity<BusinessUnit>(e =>
            {
                e.ToTable("BusinessUnit");
                e.HasKey(x => x.BusinessUnitId);
                e.Property(e => e.BusinessUnitId).HasColumnName("BusinessUnitId");
                e.Property(e => e.BusinessUnitName).HasColumnName("BusinessUnitName");
                e.Property(e => e.Remark).HasColumnName("Remark");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
                e.Property(e => e.ReferenceId).HasColumnName("ReferenceId");
                e.Property(e => e.TechFunctionId).HasColumnName("TechFunctionId");
            });

            modelBuilder.HasDefaultSchema("dbo").Entity<TechFunction>(e =>
            {
                e.ToTable("TechFunction");
                e.HasKey(x => x.FunctionId);
                e.Property(e => e.FunctionName).HasColumnName("FunctionName");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
            });

            modelBuilder.HasDefaultSchema("dbo").Entity<Client>(e =>
            {
                e.ToTable("Client");
                e.HasKey(x => x.ClientId);
                e.Property(e => e.ClientId).HasColumnName("ClientId");
                e.Property(e => e.ClientCode).HasColumnName("ClientCode");
                e.Property(e => e.ClientCodeReference).HasColumnName("ClientCodeReference");
                e.Property(e => e.ClientName).HasColumnName("ClientName");
                e.Property(e => e.EmailId).HasColumnName("EmailId");
                e.Property(e => e.Description).HasColumnName("Description");

                //Full Details
                e.Property(e => e.DomainId).HasColumnName("DomainId");
                e.Property(e => e.CompanyFoundedIn).HasColumnName("CompanyFoundedIn");
                e.Property(e => e.CompanyTypeId).HasColumnName("CompanyTypeId");
                e.Property(e => e.CompanyURL).HasColumnName("CompanyURL");
                e.Property(e => e.ActualStartDate).HasColumnName("ActualStartDate");
                e.Property(e => e.ActualEndDate).HasColumnName("ActualEndDate");
                e.Property(e => e.AgreementDuration).HasColumnName("AgreementSignedByClient");
                e.Property(e => e.AgreementSignedByClient).HasColumnName("AgreementSignedByClient");
                e.Property(e => e.AgreementSignedByDF).HasColumnName("AgreementSignedByDF");
                e.Property(e => e.IsActive).HasColumnName("IsActive");

                // Address Details
                e.Property(e => e.ContactNumber).HasColumnName("ContactNumber");
                e.Property(e => e.Address).HasColumnName("Address");
                e.Property(e => e.CountryId).HasColumnName("CountryId");
                e.Property(e => e.CityId).HasColumnName("CityId");
                e.Property(e => e.StateId).HasColumnName("StateId");
                e.Property(e => e.Zip).HasColumnName("Zip");

                //Leads
                e.Property(e => e.DeliveryManagerId).HasColumnName("DeliveryManagerId");
                e.Property(e => e.SalesPersonId).HasColumnName("SalesPersonId");

                //Billing
                e.Property(e => e.CurrencyId).HasColumnName("CurrencyId");
                e.Property(e => e.HourlyBillingRate).HasColumnName("HourlyBillingRate");
                e.Property(e => e.WeeklyBillingRate).HasColumnName("HourlyBillingRate");
                e.Property(e => e.MonthlyBillingRate).HasColumnName("MonthlyBillingRate");
                e.Property(e => e.PaymentCycleId).HasColumnName("PaymentCycleId");
                e.Property(e => e.NoOfYearsInBusiness).HasColumnName("NoOfYearsInBusiness");
                e.Property(e => e.PracticeId).HasColumnName("PracticeId");
                e.Property(e => e.AgreementValue).HasColumnName("AgreementValue");
                e.Property(e => e.StatusId).HasColumnName("StatusId");
                e.Property(e => e.ApproxNoOfPeople).HasColumnName("ApproxNoOfPeople");
                e.Property(e => e.ApproxRevenue).HasColumnName("ApproxRevenue");
                e.Property(e => e.BDEId).HasColumnName("BDEId");

                // Technologies
                e.Property(e => e.PrimaryTechnologyId).HasColumnName("PrimaryTechnologyId");

                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
            });

            modelBuilder.Entity<KRAWeightage>(e =>
            {
                e.ToTable("KRAWeightage", "dbo");
                e.Property(e => e.Id).HasColumnName("Id");
                e.Property(e => e.Name).HasColumnName("Name");
                e.Property(e => e.DisplayName).HasColumnName("DisplayName");
                e.Property(e => e.Description).HasColumnName("Description");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.IsDeleted).HasColumnName("IsDeleted");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
            });

            modelBuilder.Entity<QuarterDetails>(e =>
            {
                e.ToTable("QuarterDetails", "dbo");
                e.Property(e => e.Id).HasColumnName("Id");
                e.Property(e => e.QuarterName).HasColumnName("QuarterName");
                e.Property(e => e.QuarterYear).HasColumnName("QuarterYear");
                e.Property(e => e.Description).HasColumnName("Description");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.IsDeleted).HasColumnName("IsDeleted");
                e.Property(e => e.StatusId).HasColumnName("StatusId");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
                e.Property(e => e.QuarterYearRange).HasColumnName("QuarterYearRange");
            });

            modelBuilder.Entity<StatusLibrary>(e =>
            {
                e.ToTable("StatusLibrary", "dbo");
                e.Property(e => e.Id).HasColumnName("Id");
                e.Property(e => e.StatusName).HasColumnName("StatusName");
                e.Property(e => e.StatusType).HasColumnName("StatusType");
                e.Property(e => e.Description).HasColumnName("Description");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.IsDeleted).HasColumnName("IsDeleted");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
            });

            modelBuilder.Entity<UserApproval>(e =>
            {
                e.ToTable("UserApproval", "dbo");
                e.Property(e => e.Id).HasColumnName("Id");
                e.Property(e => e.ApprovalStatus).HasColumnName("ApprovalStatus");
                e.Property(e => e.AppraisalRange).HasColumnName("AppraisalRange");
                e.Property(e => e.ApprovedBy).HasColumnName("ApprovedBy");
                e.Property(e => e.RejectedBy).HasColumnName("RejectedBy");
                e.Property(e => e.Reason).HasColumnName("Reason");
                e.Property(e => e.Comment).HasColumnName("Comment");
                e.Property(e => e.UserId).HasColumnName("UserId");
                e.Property(e => e.KRAId).HasColumnName("KraId");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.IsDeleted).HasColumnName("IsDeleted");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
            });

            modelBuilder.Entity<UserKRA>(e =>
            {
                e.ToTable("UserKRAs", "dbo");
                e.Property(e => e.Id).HasColumnName("Id");
                e.Property(e => e.DeveloperComment).HasColumnName("DeveloperComment");
                e.Property(e => e.ManagerComment).HasColumnName("ManagerComment");
                e.Property(e => e.DeveloperRating).HasColumnName("DeveloperRating");
                e.Property(e => e.ManagerRating).HasColumnName("ManagerRating");
                e.Property(e => e.FinalRating).HasColumnName("FinalRating");
                e.Property(e => e.Score).HasColumnName("Score");
                e.Property(e => e.Status).HasColumnName("Status");
                e.Property(e => e.KRAId).HasColumnName("KraId");
                e.Property(e => e.UserId).HasColumnName("UserId");
                e.Property(e => e.QuarterId).HasColumnName("QuarterId");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.IsDeleted).HasColumnName("IsDeleted");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
                e.Property(e => e.IsApproved).HasColumnName("IsApproved");
            });

            modelBuilder.Entity<KRALibrary>(e =>
            {
                e.ToTable("KRALibrary", "dbo");
                e.Property(e => e.Id).HasColumnName("Id");
                e.Property(e => e.Name).HasColumnName("Name");
                e.Property(e => e.DisplayName).HasColumnName("DisplayName");
                e.Property(e => e.Description).HasColumnName("Description");
                e.Property(e => e.Entity).HasColumnName("Entity");
                e.Property(e => e.Entity2).HasColumnName("Entity2");
                e.Property(e => e.ApprovedBy).HasColumnName("ApprovedBy");
                e.Property(e => e.RejectedBy).HasColumnName("RejectedBy");
                e.Property(e => e.Reason).HasColumnName("Reason");
                e.Property(e => e.Comment).HasColumnName("Comment");
                e.Property(e => e.IsSpecial).HasColumnName("IsSpecial");
                e.Property(e => e.IsDefault).HasColumnName("IsDefault");
                e.Property(e => e.WeightageId).HasColumnName("WeightageId");
                e.Property(e => e.Weightage).HasColumnName("Weightage");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.IsDeleted).HasColumnName("IsDeleted");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
                e.Property(e => e.IsDescriptionRequired).HasColumnName("IsDescriptionRequired");
                e.Property(e => e.MinimumRatingForDescription).HasColumnName("MinimumRatingForDescription");
                e.Property(e => e.FunctionId).HasColumnName("FunctionId");
                e.Property(e => e.BusinessUnitId).HasColumnName("BusinessUnitId");
                
            });

            modelBuilder.Entity<Role>(e =>
            {
                e.ToTable("Role", "dbo");
                e.Property(e => e.RoleId).HasColumnName("RoleId");
                e.Property(e => e.RoleName).HasColumnName("RoleName");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
            });

            modelBuilder.Entity<SubmissionStatus>(e =>
            {
                e.ToTable("SubmissionStatus", "dbo");
                e.Property(e => e.Id).HasColumnName("Id");
                e.Property(e => e.SubmissionName).HasColumnName("SubmissionName");
                e.Property(e => e.Description).HasColumnName("Description");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.IsDeleted).HasColumnName("IsDeleted");
                e.Property(e => e.StatusId).HasColumnName("StatusId");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
            });

            modelBuilder.Entity<AppraisalHistory>(e =>
            {
                e.ToTable("AppraisalHistory", "dbo");
                e.Property(e => e.Id).HasColumnName("Id");
                e.Property(e => e.Percentage).HasColumnName("Percentage");
                e.Property(e => e.LastAppraisal).HasColumnName("LastAppraisal");
                e.Property(e => e.LastAppraisalDate).HasColumnName("LastAppraisalDate");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.IsDeleted).HasColumnName("IsDeleted");
                e.Property(e => e.UserId).HasColumnName("UserId");
                e.Property(e => e.StatusId).HasColumnName("StatusId");
                e.Property(e => e.QuarterId).HasColumnName("QuarterId");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
            });

            modelBuilder.HasDefaultSchema("dbo").Entity<Designation>(e =>
            {
                e.ToTable("Designation", "dbo");
                e.Property(e => e.DesignationId).HasColumnName("DesignationId");
                e.Property(e => e.DesignationName).HasColumnName("DesignationName");
                e.Property(e => e.ReferenceId).HasColumnName("ReferenceId");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
            });
            
            modelBuilder.Entity<RoleMapping>(e =>
            {
                e.ToTable("PA_RoleMappings", "dbo");
                e.Property(e => e.RoleMappingId).HasColumnName("RoleMappingId");
                e.Property(e => e.RoleId).HasColumnName("RoleId");
                e.Property(e => e.ModuleName).HasColumnName("ModuleName");
                e.Property(e => e.CanRead).HasColumnName("CanRead");
                e.Property(e => e.CanWrite).HasColumnName("CanWrite");
                e.Property(e => e.CanDelete).HasColumnName("CanDelete");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
               
            });

            modelBuilder.Entity<Notification>(e =>
            {
                e.ToTable("PA_Notifications", "dbo");
                e.Property(e => e.Id).HasColumnName("Id");
                e.Property(e => e.ResourceId).HasColumnName("ResourceId");
                e.Property(e => e.Title).HasColumnName("Title");
                e.Property(e => e.Description).HasColumnName("Description");
                e.Property(e => e.IsRead).HasColumnName("IsRead");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.CreateAt).HasColumnName("CreatedAt");
                e.Property(e => e.UpdateAt).HasColumnName("UpdatedAt");
            });

            modelBuilder.Entity<PATemplate>(e =>
            {
                e.ToTable("PA_Templates", "dbo");
                e.Property(e => e.TemplateId).HasColumnName("TemplateId");
                e.Property(e => e.Name).HasColumnName("Name");
                e.Property(e => e.Description).HasColumnName("Description");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
                e.Property(e => e.FunctionId).HasColumnName("FunctionId");
                e.Property(e => e.BusinessUnitId).HasColumnName("BusinessUnitId");
                

            });

            modelBuilder.Entity<PATemplateDesignation>(e =>
            {
                e.ToTable("PA_TemplateDesignation", "dbo");
                e.Property(e => e.TemplateDesignationId).HasColumnName("TemplateDesignationId");
                e.Property(e => e.TemplateId).HasColumnName("TemplateId");
                e.Property(e => e.DesignationId).HasColumnName("DesignationId");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");

            });

            modelBuilder.Entity<PATemplateKra>(e =>
            {
                e.ToTable("PA_TemplateKras", "dbo");
                e.Property(e => e.TemplateKrasId).HasColumnName("TemplateKrasId");
                e.Property(e => e.TemplateId).HasColumnName("TemplateId");
                e.Property(e => e.KraId).HasColumnName("KraId");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");

            });

            modelBuilder.Entity<Skill>(e =>
            {
                e.ToTable("PA_Skills", "dbo");
                e.Property(e => e.SkillId).HasColumnName("SkillId");
                e.Property(e => e.Name).HasColumnName("Name");
                e.Property(e => e.Description).HasColumnName("Description");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
                e.Property(e => e.CategoryId).HasColumnName("CategoryId");
              
            });

            modelBuilder.Entity<SubSkill>(e =>
            {
                e.ToTable("PA_SubSkills", "dbo");
                e.Property(e => e.SubSkillId).HasColumnName("SubSkillId");
                e.Property(e => e.SkillId).HasColumnName("SkillId");
                e.Property(e => e.Name).HasColumnName("Name");
                e.Property(e => e.Description).HasColumnName("Description");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
             
            });

            modelBuilder.Entity<ResourceSkill>(e =>
            {
                e.ToTable("PA_ResourceSkills", "dbo");
                e.Property(e => e.ResourceSkillId).HasColumnName("ResourceSkillId");
                e.Property(e => e.SkillId).HasColumnName("SkillId");
                e.Property(e => e.SubSkillId).HasColumnName("SubSkillId");
                e.Property(e => e.ResourceId).HasColumnName("ResourceId");
                e.Property(e => e.Experience).HasColumnName("Experience");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
                e.Property(e => e.SkillExperience).HasColumnName("SkillExperience");
                e.Property(e => e.SubSkillExperience).HasColumnName("SubSkillExperience");
                e.Property(e => e.SkillVersion).HasColumnName("SkillVersion");
                e.Property(e => e.SkillDescription).HasColumnName("SkillDescription");
                e.Property(e => e.SubSkillVersion).HasColumnName("SubSkillVersion");
                e.Property(e => e.SubSkillDescription).HasColumnName("SubSkillDescription");
                e.Property(e => e.IsApproved).HasColumnName("IsApproved");
                e.Property(e => e.ApprovedBy).HasColumnName("ApprovedBy");
                e.Property(e => e.RejectedBy).HasColumnName("RejectedBy");
                e.Property(e => e.RejectedComment).HasColumnName("RejectedComment");
                e.Property(e => e.IsDeleted).HasColumnName("IsDeleted");

            });

            modelBuilder.Entity<Category>(e =>
            {
                e.ToTable("PA_Categories", "dbo");
                e.Property(e => e.CategoryId).HasColumnName("CategoryId");
                e.Property(e => e.CategoryName).HasColumnName("CategoryName");
                e.Property(e => e.Description).HasColumnName("Description");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");

            });

            modelBuilder.Entity<RoleMaster>(e =>
            {
                e.ToTable("PA_Roles", "dbo");
                e.Property(e => e.RoleId).HasColumnName("RoleId");
                e.Property(e => e.RoleName).HasColumnName("RoleName");
                e.Property(e => e.Description).HasColumnName("Description");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
                e.Property(e => e.IsDefault).HasColumnName("IsDefault");
                e.Property(e => e.IsAdmin).HasColumnName("IsAdmin");

            });

            modelBuilder.Entity<UserRole>(e =>
            {
                e.ToTable("PA_UserRoles", "dbo");
                e.Property(e => e.Id).HasColumnName("Id");
                e.Property(e => e.UserId).HasColumnName("UserId");
                e.Property(e => e.RoleId).HasColumnName("RoleId");              

            });

            modelBuilder.Entity<DesignatedRole>(e =>
            {
                e.ToTable("DesignatedRole", "dbo");
                e.Property(e => e.DesignatedRoleId).HasColumnName("DesignatedRoleId");
                e.Property(e => e.DesignatedRoleName).HasColumnName("DesignatedRoleName");
                e.Property(e => e.Description).HasColumnName("Description");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
            });

            modelBuilder.Entity<UserQuarterlyAchievement>(e =>
            {
                e.ToTable("PA_UserQuarterlyAchievements", "dbo");
                e.Property(e => e.Id).HasColumnName("Id");
                e.Property(e => e.UserId).HasColumnName("UserId");
                e.Property(e => e.QuarterId).HasColumnName("QuarterId");
                e.Property(e => e.UserAchievement).HasColumnName("UserAchievement");
                e.Property(e => e.ManagerQuartelyComment).HasColumnName("ManagerComment");
                e.Property(e => e.CreateBy).HasColumnName("CreateBy");
                e.Property(e => e.CreateDate).HasColumnName("CreateDate");
                e.Property(e => e.UpdateBy).HasColumnName("UpdateBy");
                e.Property(e => e.UpdateDate).HasColumnName("UpdateDate");
                e.Property(e => e.IsActive).HasColumnName("IsActive");
            });
        }
    }
}