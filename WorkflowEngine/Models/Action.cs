using System;
using System.Collections.Generic;

namespace WorkflowEngine.Models
{
    public class Action
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; } = true;
        public List<string> FromStates { get; set; } = new List<string>();
        public string ToState { get; set; }
    }
}
