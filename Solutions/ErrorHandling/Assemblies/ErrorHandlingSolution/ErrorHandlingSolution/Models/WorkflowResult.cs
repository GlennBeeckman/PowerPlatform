using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHandlingSolution.Models
{
    public class WorkflowResult
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string location { get; set; }
        public Tags tags { get; set; }
        public Run run { get; set; }
    }

    public class Run
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class Tags
    {
        public string flowDisplayName { get; set; }
        public string capabilities { get; set; }
        public string environmentName { get; set; }
        public string logicAppName { get; set; }
        public string environmentWorkflowId { get; set; }
        public string xrmWorkflowId { get; set; }
        public string environmentFlowSuspensionReason { get; set; }
        public string sharingType { get; set; }
        public string state { get; set; }
        public string createdTime { get; set; }
        public string lastModifiedTime { get; set; }
        public string createdBy { get; set; }
        public string triggerType { get; set; }
    }

}
