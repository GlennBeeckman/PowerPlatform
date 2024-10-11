using ErrorHandlingSolution.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHandlingSolution.Services
{
    public class ErrorService:ServiceBase
    {
        public ErrorService(IServiceProvider serviceProvider, bool useCurrentUser, int httpRequestTimeoutInSeconds = 15) : base(serviceProvider, useCurrentUser, httpRequestTimeoutInSeconds)
        {}

        public ErrorService(IOrganizationService service) : base(service)
        {}

        public ErrorService(CodeActivityContext executionContext, Guid userId, string orgName) : base(executionContext, userId, null, orgName: orgName)
        {}


        public string ProcessError(string resultInput, string workflowInput)
        {
            Trace("In ProcessError");
            // find first error
            List<FlowResult> results = JsonConvert.DeserializeObject<List<FlowResult>>(resultInput);
            FlowResult errorResult = results.Where(r => r.status == "Failed").FirstOrDefault();
            string recordId = null;

            if(errorResult != null)
            {
                string errorCode = "";
                string errorMessage = "";
                string actionName = errorResult.name;

                if(errorResult.error == null)
                {
                    string json = JsonConvert.SerializeObject(errorResult.outputs);
                    Outputs errorOutput = JsonConvert.DeserializeObject<Outputs>(json);

                    errorCode = errorOutput.body.error.code;
                    errorMessage = errorOutput.body.error.message;
                }
                else if(errorResult.error != null)
                {
                    errorCode = errorResult.error.code;
                    errorMessage = errorResult.error.message;
                }
                recordId = CreateErrorRecord(errorCode, errorMessage, actionName, workflowInput, resultInput);
            }

            return recordId;
        }

        private string CreateErrorRecord(string errorcode, string errorMessage, string actionName, string workflowInput, string resultInput)
        {
            Trace("in CreateErrorRecord");

            WorkflowResult workflowResult = JsonConvert.DeserializeObject<WorkflowResult>(workflowInput);
            Entity flow = UpsertFlowRecord(workflowResult);

            Trace("Creating error record");

            Entity errorRecord = new Entity("ord_flowerrorlog");
            errorRecord["ord_name"] = "ErrorLog - " + DateTime.Now;
            errorRecord["ord_actionname"] = actionName;
            errorRecord["ord_errorcode"] = errorcode;
            errorRecord["ord_errormessage"] = errorMessage;
            errorRecord["ord_flowid"] = flow.ToEntityReference() ;
            errorRecord["ord_runid"] = workflowResult.run.name;
            errorRecord["ord_flowrunurl"] = "https://flow.microsoft.com/manage/environments/" + workflowResult.tags.environmentName + "/flows/" + flow.GetAttributeValue<string>("ord_flowidurl") + "/runs/" + workflowResult.run.name;
            errorRecord["ord_resultjson"] = resultInput.Length > 4000 ? resultInput.Substring(0,3999) : resultInput;
            errorRecord["ord_workflowjson"] = workflowInput.Length > 4000 ? workflowInput.Substring(0,3999) : workflowInput;

             Guid recordId = OrganizationService.Create(errorRecord);

            return recordId.ToString();
        }

        private Entity UpsertFlowRecord(WorkflowResult workflowResult)
        {
            Trace("   In UpsertFlowRecord");
            // Deserialize workflow
            string environment = workflowResult.tags.environmentName;
            string workflowId = workflowResult.tags.xrmWorkflowId;

            Trace("      Environment: " + environment);
            Trace("      Workflow Id: " + workflowId);

            Entity flow = new Entity("ord_flow");

            // search for exisiting flow
            var query_ord_environment = environment;
            var query_ord_xrmworkflowid = workflowId;

            var query = new QueryExpression("ord_flow");
            query.ColumnSet.AddColumns("ord_flowid", "ord_name", "createdon");
            query.Criteria.AddCondition("ord_environment", ConditionOperator.Equal, query_ord_environment);
            query.Criteria.AddCondition("ord_xrmworkflowid", ConditionOperator.Equal, query_ord_xrmworkflowid);
            query.AddOrder("ord_name", OrderType.Ascending);

            EntityCollection flowsCollection = OrganizationService.RetrieveMultiple(query);
            Trace("      Flows with same environment and id found: " + flowsCollection.Entities.Count);

            if (flowsCollection.Entities.Count > 0)
            {
                Trace(      "Updating Flow Record with Id: " + flowsCollection.Entities.FirstOrDefault().Id);
                flow.Id = flowsCollection.Entities.FirstOrDefault().Id;
            }
            else
            {
                Trace("      No Flow records found, creating new one");
            }

            flow["ord_name"] = workflowResult.tags.flowDisplayName;
            flow["ord_flowcreatedon"] = Convert.ToDateTime(workflowResult.tags.createdTime);
            flow["ord_lastmodifiedon"] = Convert.ToDateTime(workflowResult.tags.lastModifiedTime);
            flow["ord_latesterror"] = DateTime.Now;
            flow["ord_xrmworkflowid"] = workflowId;
            flow["ord_environment"] = environment;
            flow["ord_flowidurl"] = workflowResult.name;

            UpsertRequest request = new UpsertRequest()
            {
                Target = flow
            };

            UpsertResponse response = (UpsertResponse)OrganizationService.Execute(request);

            if (response.RecordCreated)
            {
                Trace("Record created");
                flow.Id = response.Target.Id;
            }
            else
                Trace("Record updated");

            Trace("      Flow recordId: " + response.Target.Id);
            return flow;
        }
    }
}
