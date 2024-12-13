using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Workflow;
using PowerAutomateExtensions.Services;

namespace PowerAutomateExtensions.Extensions.RegEx
{
    public class GetMatches: CodeActivity
    {
        [Input("InputText"), RequiredArgument]
        public InArgument<string> InputText { get; set; }

        [Input("RegEx"), RequiredArgument]
        public InArgument<string> RegEx { get; set; }

        [Input("IncludeGroups"), RequiredArgument]
        public InArgument<bool> IncludeGroups { get; set; }

        [Output("Matches")]
        public OutArgument<string> Matches { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            string inputText = InputText.Get<string>(executionContext);
            string regEx = RegEx.Get<string>(executionContext);
            bool includeGroups = IncludeGroups.Get(executionContext);
            var context = executionContext.GetExtension<IWorkflowContext>();

            RegExService regExService = new RegExService(executionContext, context.InitiatingUserId, context.OrganizationName);
            string matches = regExService.GetMatches(inputText, regEx, includeGroups);
            Matches.Set(executionContext, matches);
        }
    }
}
