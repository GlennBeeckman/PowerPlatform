using System.Activities;
using Microsoft.Xrm.Sdk.Workflow;
using PowerAutomateExtensions.Services;

namespace PowerAutomateExtensions.Extensions.RegEx
{
    public class RegExIsMatch : CodeActivity
    {
        [Input("InputText"), RequiredArgument]
        public InArgument<string> InputText { get; set; }

        [Input("RegEx"), RequiredArgument]
        public InArgument<string> RegEx { get; set; }

        [Output("IsMatch")]
        public OutArgument<string> IsMatch { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            string inputText = InputText.Get<string>(executionContext);
            string regEx = RegEx.Get<string>(executionContext);

            var context = executionContext.GetExtension<IWorkflowContext>();
            RegExService regExService = new RegExService(executionContext, context.InitiatingUserId, context.OrganizationName);

            bool isMatch = regExService.IsMatch(inputText, regEx);
            regExService.Trace("IsMatch", $"inputText: {inputText}, regEx: {regEx}, isMatch: {isMatch}");
            IsMatch.Set(executionContext, isMatch.ToString());
        }
    }
}
