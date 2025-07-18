using System;
using System.Collections.Generic;

namespace WorkflowEngine.Models
{
    public class WorkflowHistoryEntry
    {
        public string ActionId { get; set; }
        public string FromStateId { get; set; }
        public string ToStateId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class WorkflowInstance
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string DefinitionId { get; set; }
        public string CurrentStateId { get; set; }
        public List<WorkflowHistoryEntry> History { get; set; } = new List<WorkflowHistoryEntry>();
    }
}
