using System;
using System.Threading.Tasks;
using Xunit;
using WorkflowEngine.Models;
using WorkflowEngine.Services;
using System.Collections.Generic;
using System.Linq;

namespace WorkflowEngine.Tests
{
    public class WorkflowDefinitionServiceTests
    {
        [Fact]
        public async Task CreateDefinition_ValidDefinition_ReturnsCreatedDefinition()
        {
            // Arrange
            var service = new WorkflowDefinitionService();
            var definition = CreateValidWorkflowDefinition();

            // Act
            var result = await service.CreateDefinitionAsync(definition);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(definition.Id, result.Id);
        }

        [Fact]
        public async Task CreateDefinition_DuplicateStateIds_ThrowsArgumentException()
        {
            // Arrange
            var service = new WorkflowDefinitionService();
            var definition = new WorkflowDefinition
            {
                Id = "test",
                Name = "Test Workflow",
                States = new List<State>
                {
                    new State { Id = "state1", Name = "State 1", IsInitial = true },
                    new State { Id = "state1", Name = "Duplicate State" }
                },
                Actions = new List<Action>
                {
                    new Action { Id = "action1", Name = "Action 1", FromStates = new List<string> { "state1" }, ToState = "state1" }
                }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateDefinitionAsync(definition));
        }

        [Fact]
        public async Task CreateDefinition_NoInitialState_ThrowsArgumentException()
        {
            // Arrange
            var service = new WorkflowDefinitionService();
            var definition = new WorkflowDefinition
            {
                Id = "test",
                Name = "Test Workflow",
                States = new List<State>
                {
                    new State { Id = "state1", Name = "State 1", IsInitial = false },
                    new State { Id = "state2", Name = "State 2", IsInitial = false }
                },
                Actions = new List<Action>
                {
                    new Action { Id = "action1", Name = "Action 1", FromStates = new List<string> { "state1" }, ToState = "state2" }
                }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateDefinitionAsync(definition));
        }

        [Fact]
        public async Task GetDefinition_ExistingDefinition_ReturnsDefinition()
        {
            // Arrange
            var service = new WorkflowDefinitionService();
            var definition = CreateValidWorkflowDefinition();
            await service.CreateDefinitionAsync(definition);

            // Act
            var result = await service.GetDefinitionAsync(definition.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(definition.Id, result.Id);
        }

        [Fact]
        public async Task GetDefinition_NonExistentDefinition_ThrowsKeyNotFoundException()
        {
            // Arrange
            var service = new WorkflowDefinitionService();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetDefinitionAsync("nonexistent"));
        }

        private static WorkflowDefinition CreateValidWorkflowDefinition()
        {
            return new WorkflowDefinition
            {
                Id = "test",
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
        }
    }
}
