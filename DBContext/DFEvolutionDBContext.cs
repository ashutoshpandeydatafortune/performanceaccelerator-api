using DF_EvolutionAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DF_EvolutionAPI
{
    public class DFEvolutionDBContext : IdentityDbContext<IdentityUser>
    {
        public DFEvolutionDBContext()
        {
        }
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
        public virtual DbSet<Designation> Designation { get; set; }
        public virtual DbSet<RoleMapping> RoleMapping { get; set; }
        public virtual DbSet<UserApproval> UserApproval { get; set; }
        public virtual DbSet<BusinessUnit> BusinessUnits { get; set; }
        public virtual DbSet<KRAWeightage> KRAWeightages { get; set; }
        public virtual DbSet<StatusLibrary> StatusLibrary { get;  set; }
        public virtual DbSet<QuarterDetails> QuarterDetails { get; set; }
        public virtual DbSet<ProjectResource> ProjectResources { get; set; }
        public virtual DbSet<AppraisalHistory> AppraisalHistory { get; set; }
        public virtual DbSet<SubmissionStatus> SubmissionStatus { get; set; }
        public virtual DbSet<ResourceFunction> ResourceFunctions { get; set; }
        public virtual DbSet<RoleMapping> PA_RoleMappings { get; set; }


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
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("AspNetUserRoles","dbo");
            });

            modelBuilder.HasDefaultSchema("dbo");

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
            });

            modelBuilder.HasDefaultSchema("dbo").Entity<ResourceFunction>(e =>
            {
                e.ToTable("ResourceFunctions");
                e.HasKey(x => x.ResourceFunctionId);
                e.Property(e => e.ResourceFunctionName).HasColumnName("ResourceFunctionName");
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
            ////RoleMapping 
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
            });

        }
    }
}