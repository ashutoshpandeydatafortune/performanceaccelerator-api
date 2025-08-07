using System.Collections.Generic;
using System.Threading.Tasks;
using DF_PA_API.Models;
using DF_EvolutionAPI.Models;
using System.Data;

namespace DF_EvolutionAPI.Services
{
    public interface IResourceService
    {
        Task<List<Resource>> GetAllResources();
        Task<string> GetMyTeamDetails(int userId);
        Task<string> GetChildResources(string userName);
        Task<Resource> GetProfileDetails(int? resourceId);
        Task<Resource> GetResourceByEmailId(string EmailId);
        Task<List<Resource>> GetAllResourceDetailsByResourceId(int? resourceId);
        Task<List<FunctionsDesignations>> GetDesignationsByFunctionId(int functionId);
        Task<List<FunctionsDesignations>> GetDesignatedRolesByFunctionId(int functionId);
        Task<List<ResourceKrasSatus>> GetResourcesKrasStatus(SearchKraStatus searchKraStatus);
        Task<ReportingToName> GetUserManagerName(int userId);
        Task<ResourceEvaluationResponse> GetPendingResourceEvaluations(int? userId);
        Task<ResourceEvaluationResponse> GetCompletedResourceEvaluations(int? userId);
        Task<ResourceEvaluationResponse> GetPendingSelfEvaluations(int? userId);
        Task<QuarterDetails> GetCurrentQuarter();
        Task<List<ApprovalResources>> GetPendingKrasApprovalResources( int quarteId, int userId);
        Task<bool> ResourceUpdateKraApproval(List<ResourceKraApprovalUpdate> resourceKraApprovalUpdate);

        Task<List<ResourceProjectAssignment>> ResourceProjectAssignment(int resourceId);
    }
}
