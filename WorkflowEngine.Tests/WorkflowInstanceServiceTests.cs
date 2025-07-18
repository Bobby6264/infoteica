using System;
using System.Threading.Tasks;
using Xunit;
using WorkflowEngine.Models;
using WorkflowEngine.Services;
using System.Collections.Generic;
using System.Linq;

namespace WorkflowEngine.Tests
{
    public class WorkflowInstanceServiceTests
    {
        private readonly IWorkflowDefinitionService _definitionService;
        private readonly IWorkflowInstanceService _instanceService;
        private readonly string _definitionId;

        public WorkflowInstanceServiceTests()
        {
            _definitionService = new WorkflowDefinitionService();
            _instanceService = new WorkflowInstanceService(_definitionService);
            _definitionId = SetupTestWorkflow().Result;
        }

        [Fact]
        public async Task StartInstance_ValidDefinition_ReturnsNewInstance()
        {
            // Act
            var instance = await _instanceService.StartInstanceAsync(_definitionId);

            // Assert
            Assert.NotNull(instance);
            Assert.Equal(_definitionId, instance.DefinitionId);
            Assert.Equal("pending", instance.CurrentStateId);
            Assert.Single(instance.History);
        }

        [Fact]
        public async Task ExecuteAction_ValidAction_TransitionsState()
        {
            // Arrange
            var instance = await _instanceService.StartInstanceAsync(_definitionId);

            // Act
            var updatedInstance = await _instanceService.ExecuteActionAsync(instance.Id, "approve");

            // Assert
            Assert.NotNull(updatedInstance);
            Assert.Equal("approved", updatedInstance.CurrentStateId);
            Assert.Equal(2, updatedInstance.History.Count);
            Assert.Equal("approve", updatedInstance.History.Last().ActionId);
        }

        [Fact]
        public async Task ExecuteAction_InvalidAction_ThrowsInvalidOperationException()
        {
            // Arrange
            var instance = await _instanceService.StartInstanceAsync(_definitionId);
            var updatedInstance = await _instanceService.ExecuteActionAsync(instance.Id, "approve");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _instanceService.ExecuteActionAsync(updatedInstance.Id, "reject"));
        }

        [Fact]
        public async Task ExecuteAction_FinalState_ThrowsInvalidOperationException()
        {
            // Arrange
            var instance = await _instanceService.StartInstanceAsync(_definitionId);
            await _instanceService.ExecuteActionAsync(instance.Id, "reject");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _instanceService.ExecuteActionAsync(instance.Id, "approve"));
        }

        private async Task<string> SetupTestWorkflow()
        {
            var definition = new WorkflowDefinition
            {
                Id = "test-workflow",
                Name = "Test Workflow",
                States = new List<State>
                {
                    new State { Id = "pending", Name = "Pending", IsInitial = true },
                    new State { Id = "approved", Name = "Approved" },
                    new State { Id = "rejected", Name = "Rejected", IsFinal = true }
                },
                Actions = new List<Action>
                {
                    new Action {
                        Id = "approve",
                        Name = "Approve",
                        FromStates = new List<string> { "pending" },
                        ToState = "approved"
                    },
                    new Action {
                        Id = "reject",
                        Name = "Reject",
                        FromStates = new List<string> { "pending" },
                        ToState = "rejected"
                    }
                }
            };

            var result = await _definitionService.CreateDefinitionAsync(definition);
            return result.Id;
        }
    }
}
