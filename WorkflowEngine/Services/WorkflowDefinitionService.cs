using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowEngine.Models;

namespace WorkflowEngine.Services
{
    public class WorkflowDefinitionService : IWorkflowDefinitionService
    {
        private readonly Dictionary<string, WorkflowDefinition> _definitions = new Dictionary<string, WorkflowDefinition>();

        public Task<WorkflowDefinition> CreateDefinitionAsync(WorkflowDefinition definition)
        {
            // Validate the definition first
            if (!ValidateDefinition(definition))
            {
                throw new ArgumentException("Invalid workflow definition");
            }

            // Generate an ID if none is provided
            if (string.IsNullOrEmpty(definition.Id))
            {
                definition.Id = Guid.NewGuid().ToString();
            }
            else if (_definitions.ContainsKey(definition.Id))
            {
                throw new ArgumentException($"Workflow definition with ID {definition.Id} already exists");
            }

            _definitions[definition.Id] = definition;
            return Task.FromResult(definition);
        }

        public Task<WorkflowDefinition> GetDefinitionAsync(string definitionId)
        {
            if (!_definitions.TryGetValue(definitionId, out var definition))
            {
                throw new KeyNotFoundException($"Workflow definition with ID {definitionId} not found");
            }
            return Task.FromResult(definition);
        }

        public Task<IEnumerable<WorkflowDefinition>> GetAllDefinitionsAsync()
        {
            return Task.FromResult(_definitions.Values.AsEnumerable());
        }

        public Task<bool> ValidateDefinitionAsync(WorkflowDefinition definition)
        {
            return Task.FromResult(ValidateDefinition(definition));
        }

        private bool ValidateDefinition(WorkflowDefinition definition)
        {
            // Check for null or empty states/actions
            if (definition == null || definition.States == null || definition.Actions == null)
            {
                return false;
            }

            // Check for duplicate state IDs
            if (definition.States.Select(s => s.Id).Distinct().Count() != definition.States.Count)
            {
                return false;
            }

            // Check for duplicate action IDs
            if (definition.Actions.Select(a => a.Id).Distinct().Count() != definition.Actions.Count)
            {
                return false;
            }

            // Check for exactly one initial state
            if (definition.States.Count(s => s.IsInitial) != 1)
            {
                return false;
            }

            // Check that all states referenced in actions exist
            var stateIds = definition.States.Select(s => s.Id).ToHashSet();
            foreach (var action in definition.Actions)
            {
                if (!stateIds.Contains(action.ToState))
                {
                    return false;
                }

                foreach (var fromState in action.FromStates)
                {
                    if (!stateIds.Contains(fromState))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
