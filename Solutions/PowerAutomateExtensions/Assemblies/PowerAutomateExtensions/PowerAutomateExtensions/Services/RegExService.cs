using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorHandlingSolution.Services;
using Microsoft.Xrm.Sdk;

namespace PowerAutomateExtensions.Services
{
    public class RegExService: ServiceBase
    {
        public RegExService(IServiceProvider serviceProvider, bool useCurrentUser, int httpRequestTimeoutInSeconds = 15) : base(serviceProvider, useCurrentUser, httpRequestTimeoutInSeconds)
        { }

        public RegExService(IOrganizationService service) : base(service)
        { }

        public RegExService(CodeActivityContext executionContext, Guid userId, string orgName) : base(executionContext, userId, null, orgName: orgName)
        { }

        public bool IsMatch(string inputText, string regEx)
        {
            Trace("Start IsMatch");
            return System.Text.RegularExpressions.Regex.IsMatch(inputText, regEx);
        }
    }
}
