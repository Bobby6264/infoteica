using System;
using System.Collections.Generic;

namespace WorkflowEngine.Models
{
    public class WorkflowDefinition
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public List<State> States { get; set; } = new List<State>();
        public List<Action> Actions { get; set; } = new List<Action>();
    }
}
