using ErrorHandlingSolution.Models;
using ErrorHandlingSolution.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Newtonsoft.Json;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHandlingSolution
{
    public class ProcessError : CodeActivity
    {
        [Input("ScopeResult"), RequiredArgument]
        public InArgument<string> ScopeResult { get; set; }       
        
        [Input("WorkflowResult"), RequiredArgument]
        public InArgument<string> WorkFlowResult { get; set; }

        [Output("ErrorRecordId")]
        public OutArgument<string> ErrorRecordId { get; set; } 
        


        protected override void Execute(CodeActivityContext executionContext)
        {
            string scopeResult = ScopeResult.Get<string>(executionContext);
            string workflowResult = WorkFlowResult.Get<string>(executionContext);

            var context = executionContext.GetExtension<IWorkflowContext>();
            ErrorService service = new ErrorService(executionContext, context.UserId, context.OrganizationName);
            service.Trace("Service Created, starting processing error");
            string recordId = service.ProcessError(scopeResult, workflowResult);

            ErrorRecordId.Set(executionContext, recordId);
        }
    }
}
