# Workflow Engine API

A configurable workflow engine implemented as a .NET 8 API that lets clients define and execute workflow state machines.

## Overview

This project implements a minimal backend service for configuring and executing workflow state machines, with the following features:

- Define one or more workflows as configurable state machines (states + actions).
- Start workflow instances from a chosen definition.
- Execute actions to move an instance between states, with full validation.
- Inspect / list states, actions, definitions, and running instances.

## Project Structure

```
WorkflowEngine/
├── Controllers/
│   ├── WorkflowDefinitionController.cs
│   └── WorkflowInstanceController.cs
├── Models/
│   ├── State.cs
│   ├── Action.cs
│   ├── WorkflowDefinition.cs
│   └── WorkflowInstance.cs
├── Services/
│   ├── IWorkflowDefinitionService.cs
│   ├── IWorkflowInstanceService.cs
│   ├── WorkflowDefinitionService.cs
│   └── WorkflowInstanceService.cs
├── Program.cs
└── WorkflowEngine.csproj
```

## Getting Started

### Prerequisites

- .NET 8 SDK

### Running the Project

1. Clone the repository
2. Navigate to the project directory
3. Run the following command:

```
dotnet run
```

The API will be accessible at https://localhost:5001 and http://localhost:5000.

Swagger UI will be available at https://localhost:5001/swagger if running in development mode.

## API Endpoints

### Workflow Definitions

- `POST /api/workflowdefinition` - Create a new workflow definition
- `GET /api/workflowdefinition/{id}` - Get a workflow definition by ID
- `GET /api/workflowdefinition` - Get all workflow definitions
- `POST /api/workflowdefinition/validate` - Validate a workflow definition

### Workflow Instances

- `POST /api/workflowinstance?definitionId={definitionId}` - Start a new workflow instance
- `GET /api/workflowinstance/{id}` - Get a workflow instance by ID
- `GET /api/workflowinstance` - Get all workflow instances
- `POST /api/workflowinstance/{id}/execute?actionId={actionId}` - Execute an action on a workflow instance

## Assumptions and Limitations

1. In-memory persistence only - data is lost when the application restarts
2. No authentication or authorization
3. Limited error handling and logging
4. No support for parallel workflows or complex branching logic
5. No persistence to disk (although this could be added with a repository layer)

## Dependencies

- ASP.NET Core 8
- Swashbuckle.AspNetCore (for Swagger documentation)
