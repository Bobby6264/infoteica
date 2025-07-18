using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowEngine.Models;

namespace WorkflowEngine.Services
{
    public interface IWorkflowInstanceService
    {
        Task<WorkflowInstance> StartInstanceAsync(string definitionId);
        Task<WorkflowInstance> GetInstanceAsync(string instanceId);
        Task<IEnumerable<WorkflowInstance>> GetAllInstancesAsync();
        Task<WorkflowInstance> ExecuteActionAsync(string instanceId, string actionId);
    }
}
