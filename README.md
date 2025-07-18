# Configurable Workflow Engine (State-Machine API)

This project implements a configurable workflow engine as a RESTful API that allows defining and executing state machines.

## Project Overview

This is a minimal backend service that lets a client:
1. Define one or more workflows as configurable state machines (states + actions).
2. Start workflow instances from a chosen definition.
3. Execute actions to move an instance between states, with full validation.
4. Inspect / list states, actions, definitions, and running instances.

## Project Structure

```
├── WorkflowEngine/              # Main API project
│   ├── Controllers/             # API controllers
│   ├── Models/                  # Domain models
│   ├── Services/                # Business logic
│   └── README.md                # Project-specific documentation
├── WorkflowEngine.Tests/        # Unit tests for the API
└── WorkflowEngine.sln           # Solution file
```

## Getting Started

### Prerequisites

- .NET 8 SDK

### Running the Project

1. Clone this repository
2. Navigate to the project directory
3. Run the following command:

```
cd WorkflowEngine
dotnet run
```

The API will be accessible at:
- https://localhost:5001
- http://localhost:5000

Swagger UI will be available at https://localhost:5001/swagger to explore the API endpoints.

## Core Concepts

| Concept | Description |
|---------|-------------|
| State | Represents a point in the workflow with properties: id, name, isInitial (bool), isFinal (bool), enabled (bool) |
| Action (transition) | Defines a transition between states with properties: id, name, enabled (bool), fromStates (collection of state IDs), toState (single state ID) |
| Workflow definition | Collection of States and Actions with at least one initial state |
| Workflow instance | Reference to a definition with current state and history |

## API Endpoints

### Workflow Definitions
- Create a new definition: `POST /api/workflowdefinition`
- Get a definition: `GET /api/workflowdefinition/{id}`
- Get all definitions: `GET /api/workflowdefinition`
- Validate a definition: `POST /api/workflowdefinition/validate`

### Workflow Instances
- Start a new instance: `POST /api/workflowinstance?definitionId={id}`
- Get an instance: `GET /api/workflowinstance/{id}`
- Get all instances: `GET /api/workflowinstance`
- Execute an action: `POST /api/workflowinstance/{id}/execute?actionId={actionId}`

## Dependencies

- ASP.NET Core 8.0
- Swashbuckle.AspNetCore (for Swagger documentation)
- xUnit (for unit tests)

## Assumptions and Limitations

1. Data is stored in-memory and will be lost when the application restarts.
2. No authentication or authorization is implemented.
3. Basic error handling is provided.
4. No complex validation rules beyond those required in the specification.

## Future Enhancements

With more time, the following enhancements could be added:
1. Persistent storage (database or file-based)
2. Authentication and authorization
3. Advanced validation rules
4. Logging and monitoring
5. Support for parallel workflows or complex branching logic
