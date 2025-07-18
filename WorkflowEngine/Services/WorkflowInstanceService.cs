using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowEngine.Models;

namespace WorkflowEngine.Services
{
    public class WorkflowInstanceService : IWorkflowInstanceService
    {
        private readonly Dictionary<string, WorkflowInstance> _instances = new Dictionary<string, WorkflowInstance>();
        private readonly IWorkflowDefinitionService _definitionService;

        public WorkflowInstanceService(IWorkflowDefinitionService definitionService)
        {
            _definitionService = definitionService;
        }

        public async Task<WorkflowInstance> StartInstanceAsync(string definitionId)
        {
            // Get the workflow definition
            var definition = await _definitionService.GetDefinitionAsync(definitionId);
            
            // Find the initial state
            var initialState = definition.States.FirstOrDefault(s => s.IsInitial);
            if (initialState == null)
            {
                throw new InvalidOperationException($"Workflow definition {definitionId} does not have an initial state");
            }

            // Create a new instance
            var instance = new WorkflowInstance
            {
                Id = Guid.NewGuid().ToString(),
                DefinitionId = definitionId,
                CurrentStateId = initialState.Id,
                History = new List<WorkflowHistoryEntry>
                {
                    new WorkflowHistoryEntry
                    {
                        ActionId = "WORKFLOW_START",
                        ToStateId = initialState.Id,
                        Timestamp = DateTime.UtcNow
                    }
                }
            };

            _instances[instance.Id] = instance;
            return instance;
        }

        public Task<WorkflowInstance> GetInstanceAsync(string instanceId)
        {
            if (!_instances.TryGetValue(instanceId, out var instance))
            {
                throw new KeyNotFoundException($"Workflow instance with ID {instanceId} not found");
            }
            return Task.FromResult(instance);
        }

        public Task<IEnumerable<WorkflowInstance>> GetAllInstancesAsync()
        {
            return Task.FromResult(_instances.Values.AsEnumerable());
        }

        public async Task<WorkflowInstance> ExecuteActionAsync(string instanceId, string actionId)
        {
            // Get the instance
            if (!_instances.TryGetValue(instanceId, out var instance))
            {
                throw new KeyNotFoundException($"Workflow instance with ID {instanceId} not found");
            }

            // Get the workflow definition
            var definition = await _definitionService.GetDefinitionAsync(instance.DefinitionId);
            
            // Find the current state
            var currentState = definition.States.FirstOrDefault(s => s.Id == instance.CurrentStateId);
            if (currentState == null)
            {
                throw new InvalidOperationException($"Current state {instance.CurrentStateId} not found in workflow definition");
            }

            // Check if current state is final
            if (currentState.IsFinal)
            {
                throw new InvalidOperationException($"Cannot execute actions from a final state");
            }

            // Find the action
            var action = definition.Actions.FirstOrDefault(a => a.Id == actionId);
            if (action == null)
            {
                throw new KeyNotFoundException($"Action with ID {actionId} not found in workflow definition");
            }

            // Validate the action
            if (!action.Enabled)
            {
                throw new InvalidOperationException($"Action {actionId} is disabled");
            }

            if (!action.FromStates.Contains(instance.CurrentStateId))
            {
                throw new InvalidOperationException($"Action {actionId} cannot be executed from state {instance.CurrentStateId}");
            }

            // Find the target state
            var targetState = definition.States.FirstOrDefault(s => s.Id == action.ToState);
            if (targetState == null)
            {
                throw new InvalidOperationException($"Target state {action.ToState} not found in workflow definition");
            }

            if (!targetState.Enabled)
            {
                throw new InvalidOperationException($"Target state {action.ToState} is disabled");
            }

            // Execute the transition
            var historyEntry = new WorkflowHistoryEntry
            {
                ActionId = actionId,
                FromStateId = instance.CurrentStateId,
                ToStateId = action.ToState,
                Timestamp = DateTime.UtcNow
            };

            instance.CurrentStateId = action.ToState;
            instance.History.Add(historyEntry);

            return instance;
        }
    }
}
