using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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

        public string GetMatches(string inputText, string regEx, bool includeGroups)
        {
            Trace("Start GetMatches");

            // Define a list to hold match objects
            var matchList = new List<object>();

            // Find matches using the provided regex
            MatchCollection matches = Regex.Matches(inputText, regEx);
            Trace($"Found {matches.Count} matches");

            foreach (Match match in matches)
            {
                if (includeGroups)
                {
                    var groups = new List<string>();
                    for (int i = 1; i < match.Groups.Count; i++) // Skip Groups[0], is the full match
                    {
                        groups.Add(match.Groups[i].Value);
                    }

                    matchList.Add(new
                    {
                        Match = match.Value,
                        Groups = groups
                    });
                }
                else
                {
                    matchList.Add(new { Match = match.Value });
                }
            }

            // Serialize the match list to JSON
            string jsonResult = JsonSerializer.Serialize(matchList, new JsonSerializerOptions
            {
                WriteIndented = false
            });

            return jsonResult;
        }

    }
}
