using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowEngine.Models;

namespace WorkflowEngine.Services
{
    public interface IWorkflowDefinitionService
    {
        Task<WorkflowDefinition> CreateDefinitionAsync(WorkflowDefinition definition);
        Task<WorkflowDefinition> GetDefinitionAsync(string definitionId);
        Task<IEnumerable<WorkflowDefinition>> GetAllDefinitionsAsync();
        Task<bool> ValidateDefinitionAsync(WorkflowDefinition definition);
    }
}
