using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DF_EvolutionAPI.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "pa");

            migrationBuilder.CreateTable(
                name: "AppraisalHistory",
                schema: "pa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),

                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    displayname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    LastApprisal = table.Column<int>(type: "int", nullable: true),
                    Percentage = table.Column<int>(type: "int", nullable: true),
                    LastApprisalDate = table.Column<DateTime>(type: "datetime2", nullable: false),

                    IsDeleted = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: false),
                    UpdateBy = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppraisalHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessUnits",
                schema: "pa",
                columns: table => new
                {
                    BusinessUnitId = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    
                    BusinessUnitName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: false),

                    IsDeleted = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: false),
                    UpdateBy = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessUnits", x => x.BusinessUnitId);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                schema: "pa",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),

                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgreementDuration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgreementSignedByClient = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgreementSignedByDF = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgreementSignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AgreementValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApproxNoOfPeople = table.Column<int>(type: "int", nullable: false),
                    AgreementRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApproxRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),

                    BDEId = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientCodeReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyFoundedIn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyTypeId = table.Column<int>(type: "int", nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryId = table.Column<string>(type: "nvarchar(max)", nullable: true),

                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    DeliveryManagerId = table.Column<int>(type: "int", nullable: false),
                    DomainId = table.Column<int>(type: "int", nullable: false),

                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourlyBillingRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WeeklyBillingRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthlyBillingRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentCycleId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    PracticeId = table.Column<int>(type: "int", nullable: false),
                    PrimaryTechnologyId = table.Column<int>(type: "int", nullable: false),
                    NumberOfYearsInBusiness = table.Column<int>(type: "int", nullable: false),
                    SalesPersonId = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Zip = table.Column<string>(type: "nvarchar(max)", nullable: true),

                    ApproxDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),

                    IsDeleted = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: false),
                    UpdateBy = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "KRALibrary",
                schema: "pa",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),

                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<int>(type: "int", nullable: false),
                    RejectedBy = table.Column<int>(type: "int", nullable: false),
                    Entity = table.Column<int>(type: "int", nullable: true),
                    WeightageId = table.Column<int>(type: "int", nullable: true),
                    IsDefault = table.Column<int>(type: "int", nullable: true),
                    IsSpecial = table.Column<int>(type: "int", nullable: true),

                    IsDeleted = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: false),
                    UpdateBy = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KRALibrary", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "KRAWeightages",
                schema: "pa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),

                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),

                    IsDeleted = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: false),
                    UpdateBy = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KRAWeightages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "pa",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),

                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    ProjectLeadId = table.Column<int>(type: "int", nullable: false),
                    ProjectManagerId = table.Column<int>(type: "int", nullable: false),
                    ProjectStatusId = table.Column<int>(type: "int", nullable: false),
                    ProjectTypeId = table.Column<int>(type: "int", nullable: false),
                    ProjectSubTypeId = table.Column<int>(type: "int", nullable: false),
                    ReviewCycleId = table.Column<int>(type: "int", nullable: false),
                    NumberOfResources = table.Column<int>(type: "int", nullable: false),
                    HourlyBillingRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WeeklyBillingRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthlyBillingRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),

                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),

                    IsDeleted = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: false),
                    UpdateBy = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectId);
                });

            migrationBuilder.CreateTable(
                name: "ProjectResources",
                schema: "pa",
                columns: table => new
                {
                    ProjectResourceId = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),

                    Billable = table.Column<bool>(type: "bit", nullable: false),
                    BillingCycleId = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: true),
                    PercentageAllocation = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    ResourceId = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<Decimal>(type: "decimal(18,2)", nullable: false),
                    Remark = table.Column<string>(type: "varchar(max)", nullable: false),
                    ResourceRole = table.Column<string>(type: "varchar(max)", nullable: false),
                    Shadow = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),

                    IsDeleted = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: false),
                    UpdateBy = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectResources", x => x.ProjectResourceId);
                });

            migrationBuilder.CreateTable(
                name: "QuaterDetails",
                schema: "pa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),

                    QuarterName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    QuarterYear = table.Column<int>(type: "int", nullable: false),

                    IsDeleted = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: false),
                    UpdateBy = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuaterDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                schema: "pa",
                columns: table => new
                {
                    ResourceId = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),

                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlternateNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CityId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Zip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimarySkill = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResourceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondarySkill = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Strengths = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<string>(type: "datetime2", nullable: true),
                    DateOfJoin = table.Column<string>(type: "datetime2", nullable: true),
                    Designation = table.Column<int>(type: "int", nullable: true),
                    ReportingTo = table.Column<int>(type: "int", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    TechCategoryId = table.Column<int>(type: "int", nullable: true),
                    TenureInMonths = table.Column<float>(type: "float", nullable: true),
                    TenureInYears = table.Column<float>(type: "float", nullable: true),
                    TotalYears = table.Column<float>(type: "float", nullable: true),

                    IsDeleted = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: false),
                    UpdateBy = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.ResourceId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "pa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),

                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),

                    IsDeleted = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: false),
                    UpdateBy = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleMappings",
                schema: "pa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    displayname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    roleid = table.Column<int>(type: "int", nullable: false),
                    userid = table.Column<int>(type: "int", nullable: false),

                    IsDeleted = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: false),
                    UpdateBy = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusLibrary",
                schema: "pa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),

                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
 
                    IsDeleted = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: false),
                    UpdateBy = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionStatus",
                schema: "pa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),

                    SubmissionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),

                    IsDeleted = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: false),
                    UpdateBy = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserApprovals",
                schema: "pa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),

                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppraisalRange = table.Column<int>(type: "int", nullable: false),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<int>(type: "int", nullable: false),
                    RejectedBy = table.Column<int>(type: "int", nullable: false),
                    KraId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),

                    IsDeleted = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: false),
                    UpdateBy = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserApprovals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserKRA",
                schema: "pa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),

                    ManagerComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeveloperComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeveloperRating = table.Column<int>(type: "int", nullable: false),
                    ManagerRating = table.Column<int>(type: "int", nullable: false),
                    finalrating = table.Column<int>(type: "int", nullable: false),
                    KraId = table.Column<int>(type: "int", nullable: false),
                    QuarterId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<float>(type: "float", nullable: false),

                    UserId = table.Column<int>(type: "int", nullable: true),

                    IsActive = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<int>(type: "int", nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: false),
                    UpdateBy = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserKRA", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "UserKRA", schema: "pa");
            migrationBuilder.DropTable(name: "UserApprovals", schema: "pa");
            migrationBuilder.DropTable(name: "SubmissionStatus", schema: "pa");
            migrationBuilder.DropTable(name: "StatusLibrary", schema: "pa");
            migrationBuilder.DropTable(name: "RoleMappings", schema: "pa");
            migrationBuilder.DropTable(name: "Roles", schema: "pa");
            migrationBuilder.DropTable(name: "Resources", schema: "pa");
            migrationBuilder.DropTable(name: "QuarterDetails", schema: "pa");
            migrationBuilder.DropTable(name: "ProjectResources", schema: "pa");
            migrationBuilder.DropTable(name: "Projects", schema: "pa");
            migrationBuilder.DropTable(name: "KRAWeightages", schema: "pa");
            migrationBuilder.DropTable(name: "KRALibrary", schema: "pa");
            migrationBuilder.DropTable(name: "Clients", schema: "pa");
            migrationBuilder.DropTable(name: "BusinessUnits", schema: "pa");
            migrationBuilder.DropTable(name: "AppraisalHistory", schema: "pa");
        }
    }
}
