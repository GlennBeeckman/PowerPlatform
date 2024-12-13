using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHandlingSolution.Services
{
    public partial class ServiceBase
    {
        #region PRIVATE properties

        private const int default_httpRequestTimeoutInSeconds = 15;
        private readonly Dictionary<Guid, int> _timezonecodes;
        private const int defaultTimeZoneCode = 105; // Romance Standard Time (GMT+01:00)	Brussels, Copenhagen, Madrid, Paris
        private Dictionary<string, Entity> _credentials { get; set; }
        private IOrganizationService _organizationService { get; set; }
        private IOrganizationServiceFactory _organizationServiceFactory { get; set; }
        private IOrganizationService _systemService { get; set; }
        private StringBuilder _traceBuffer { get; set; }
        private ITracingService _tracingService { get; set; }
        private bool _useCurrentUser { get; set; }

        #endregion PRIVATE properties

        #region PUBLIC properties

        public Guid ExecutionUserId { get; set; }

        public Guid InitiatingUserId { get; private set; }
        public string OrganizationName { get; private set; }

        public IOrganizationService OrganizationService
        {
            get
            {
                if (_organizationService == null)
                {
                    if (_useCurrentUser && !Guid.Empty.Equals(ExecutionUserId))
                    {
                        _organizationService = _organizationServiceFactory.CreateOrganizationService(ExecutionUserId);
                    }
                    else
                    {
                        _organizationService = _organizationServiceFactory.CreateOrganizationService(null);
                    }
                }

                return _organizationService;
            }
        }

        public IOrganizationService SystemService
        {
            get
            {
                if (_systemService == null)
                {
                    _systemService = _organizationServiceFactory.CreateOrganizationService(null);
                }

                return _systemService;
            }
        }

        #endregion PUBLIC properties

        #region Constructor

        /// <summary>
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="useCurrentUser"></param>
        /// <param name="httpRequestTimeoutInSeconds">Default timeout is 15 seconds</param>
        public ServiceBase(
            IServiceProvider serviceProvider,
            bool useCurrentUser,
            int httpRequestTimeoutInSeconds = default_httpRequestTimeoutInSeconds)
        {
            _useCurrentUser = useCurrentUser;
            _organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            _tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            var pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ExecutionUserId = pluginExecutionContext.UserId;
            InitiatingUserId = pluginExecutionContext.InitiatingUserId;
            OrganizationName = pluginExecutionContext.OrganizationName;

            _credentials = new Dictionary<string, Entity>();
            _timezonecodes = new Dictionary<Guid, int>();

            Trace("ServiceBase initialized [{0};{1};{2}]", ExecutionUserId, InitiatingUserId, OrganizationName);
        }

        /// <summary>
        /// </summary>
        /// <param name="service"></param>
        /// <param name="userId"></param>
        /// <param name="orgName"></param>
        /// <param name="httpRequestTimeoutInSeconds">Default timeout is 15 seconds</param>
        public ServiceBase(
            IOrganizationService service,
            Guid? userId = null,
            string orgName = null,
            int httpRequestTimeoutInSeconds = default_httpRequestTimeoutInSeconds)
        {
            _organizationService = service;
            _systemService = _organizationService;

            if (userId != null)
            {
                ExecutionUserId = userId.Value;
                InitiatingUserId = userId.Value;
            }
            OrganizationName = orgName;

            _credentials = new Dictionary<string, Entity>();
            _timezonecodes = new Dictionary<Guid, int>();

            Trace("ServiceBase initialized [{0};{1};{2}]", ExecutionUserId, InitiatingUserId, OrganizationName);
        }

        /// <summary>
        /// </summary>
        /// <param name="executionContext"></param>
        /// <param name="executionUserId"></param>
        /// <param name="initiatingUserId"></param>
        /// <param name="orgName"></param>
        /// <param name="httpRequestTimeoutInSeconds">Default timeout is 15 seconds</param>
        /// <param name="useCurrentUser"></param>
        public ServiceBase(
            CodeActivityContext executionContext,
            Guid executionUserId,
            Guid? initiatingUserId = null,
            string orgName = null,
            int httpRequestTimeoutInSeconds = default_httpRequestTimeoutInSeconds, bool? useCurrentUser = null)
        {
            _organizationServiceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            _tracingService = executionContext.GetExtension<ITracingService>();

            ExecutionUserId = executionUserId;
            InitiatingUserId = initiatingUserId.HasValue ? initiatingUserId.Value : Guid.Empty;
            OrganizationName = orgName;

            if (useCurrentUser.HasValue) _useCurrentUser = useCurrentUser.Value;

            _credentials = new Dictionary<string, Entity>();
            _timezonecodes = new Dictionary<Guid, int>();

            Trace("ServiceBase initialized [{0};{1};{2}]", ExecutionUserId, InitiatingUserId, OrganizationName);
        }

        #endregion Constructor

        #region Helper Methods

        public AssignResponse Assign(EntityReference target, EntityReference assignee)
        {
            // Create the Request Object and Set the Request Object's Properties
            var req = new AssignRequest
            {
                Assignee = assignee,
                Target = target
            };

            // Execute the Request
            return (AssignResponse)OrganizationService.Execute(req);
        }

        public ExecuteMultipleResponseItem[] ExecuteMultiple(OrganizationRequest[] requests)
        {
            var responses = new List<ExecuteMultipleResponseItem>();

            var emr = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };

            OrganizationRequest[][] chunks = requests
                    .Select((s, i) => new { Value = s, Index = i })
                    .GroupBy(x => x.Index / 100)
                    .Select(grp => grp.Select(x => x.Value).ToArray())
                    .ToArray();

            var nrprocessed = 0;
            foreach (var chunck in chunks)
            {
                Console.Write("\r{0}/{1}", nrprocessed, requests.Length);
                emr.Requests.AddRange(chunck);

                var response = (ExecuteMultipleResponse)OrganizationService.Execute(emr);
                foreach (var r in response.Responses)
                {
                    r.RequestIndex += nrprocessed;
                    responses.Add(r);
                }

                nrprocessed += chunck.Length;
                emr.Requests = new OrganizationRequestCollection();
            }
            Console.WriteLine();

            return responses.ToArray();
        }
        public EntityReference GetDefaultCurrency()
        {
            EntityReference defaultcurrency = null;

            QueryExpression queryCurrency = new QueryExpression("currency")
            {
                EntityName = "transactioncurrency",
                ColumnSet = new ColumnSet(new string[] { "transactioncurrencyid" }),
                Criteria = new FilterExpression()
            };

            try
            {
                var entityData = OrganizationService.RetrieveMultiple(queryCurrency);

                if (entityData != null && entityData.Entities.Count > 0)
                    defaultcurrency = entityData.Entities[0].ToEntityReference();
            }
            catch (Exception)
            {
                throw;
            }

            return defaultcurrency;
        }

        public string GetObjectRecordUrl(string logicalname, string id, string randomrecordurl)
        {
            if (!string.IsNullOrEmpty(logicalname) && !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(randomrecordurl))
            {
                var randomrecorduri = new Uri(randomrecordurl);

                var querystring = "/main.aspx?etn=" + logicalname + "&id=" + id + "&pagetype=entityrecord";
                var url = randomrecorduri.AbsoluteUri.Substring(0, randomrecorduri.AbsoluteUri.IndexOf("main.aspx")) + querystring;
                return url;
            }

            return null;
        }

        public string GetParameter(string name, bool useSystemService = false)
        {
            var qry = new QueryExpression("ccp_parameter");
            qry.ColumnSet = new ColumnSet("ccp_value");
            qry.Criteria.AddCondition("ccp_name", ConditionOperator.Equal, name);

            // Get newest first
            qry.AddOrder("createdon", OrderType.Descending);

            // Only retrieve one record
            qry.PageInfo = new PagingInfo();
            qry.PageInfo.Count = 1;
            qry.PageInfo.PageNumber = 1;

            EntityCollection result;
            if (useSystemService) result = SystemService.RetrieveMultiple(qry);
            else result = OrganizationService.RetrieveMultiple(qry);

            if (result != null && result.Entities.Count > 0)
                return result.Entities[0].GetAttributeValue<string>("ccp_value");
            else
                return null;
        }

        public EntityCollection GetParameterGroup(string prefix, bool useSystemService = false)
        {
            var qry = new QueryExpression("ccp_parameter");
            qry.ColumnSet = new ColumnSet("ccp_name", "ccp_value");
            qry.Criteria.AddCondition("ccp_name", ConditionOperator.BeginsWith, prefix);

            return useSystemService ? SystemService.RetrieveMultiple(qry) : OrganizationService.RetrieveMultiple(qry);
        }

        public EntityCollection GetUserSecurityRoles(Guid userid)
        {
            EntityCollection UserRoles()
            {
                var qry = new QueryExpression("role")
                {
                    //Setting the link entity condition and filter condition criteria/
                    LinkEntities =
                        {
                            new LinkEntity
                            {
                                LinkFromEntityName = "role",
                                LinkFromAttributeName = "roleid",
                                LinkToEntityName = "systemuserroles",
                                LinkToAttributeName = "roleid",
                                LinkCriteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression
                                        {
                                            AttributeName = "systemuserid",
                                            Operator = ConditionOperator.Equal,
                                            Values = { userid }
                                        }
                                    }
                                }
                            }
                        }
                };

                qry.ColumnSet = new ColumnSet("roleid", "name");

                // Obtain results from the query expression.
                return OrganizationService.RetrieveMultiple(qry);
            }

            EntityCollection TeamRoles()
            {
                var query = new QueryExpression("role");
                query.ColumnSet = new ColumnSet("roleid", "name");

                var query_teamroles = query.AddLink("teamroles", "roleid", "roleid");
                var query_teamroles_team = query_teamroles.AddLink("team", "teamid", "teamid");
                var query_teamroles_team_teammembership = query_teamroles_team.AddLink("teammembership", "teamid", "teamid");
                query_teamroles_team_teammembership.LinkCriteria.AddCondition("systemuserid", ConditionOperator.Equal, userid);

                // Obtain results from the query expression.
                return OrganizationService.RetrieveMultiple(query);
            }

            var userRoles = UserRoles();
            var teamRoles = TeamRoles();

            var merged = userRoles.Entities.Concat(teamRoles.Entities);

            EntityCollection col = new EntityCollection();
            col.Entities.AddRange(merged.Distinct());

            return col;
        }

        /// <summary>
        /// Gets the user timezonecode
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetUserTimeZoneCode(Guid userId)
        {
            if (_timezonecodes.ContainsKey(userId)) return _timezonecodes[userId];

            int timezonecode = defaultTimeZoneCode;
            try
            {
                timezonecode = OrganizationService.Retrieve("usersettings", userId, new ColumnSet("timezonecode"))
                    .GetAttributeValue<int>("timezonecode");
            }
            catch (Exception ex)
            {
                Trace($"Failed getting usersettings: {ex.Message}");
            }

            _timezonecodes.Add(userId, timezonecode);

            return timezonecode;
        }

        /// <summary>
        /// Converts UTC time to local time. Default value is 105 Romance Standard Time (GMT+01:00)	Brussels, Copenhagen, Madrid, Paris
        /// </summary>
        /// <param name="utcTime"></param>
        /// <param name="timeZoneCode"></param>
        /// <returns></returns>
        public DateTime RetrieveLocalTimeFromUTCTime(DateTime utcTime, int timeZoneCode = 105)
        {
            return ((LocalTimeFromUtcTimeResponse)OrganizationService.Execute(new LocalTimeFromUtcTimeRequest
            {
                TimeZoneCode = timeZoneCode,
                UtcTime = utcTime.ToUniversalTime()
            })).LocalTime;
        }

        public bool HasSysAdminRole(Guid userid)
        {
            // Get User admin roles
            var query = new QueryExpression("role");
            query.TopCount = 1; // We need atleast 1 result to be an admin

            var sur = query.AddLink("systemuserroles", "roleid", "roleid");
            sur.EntityAlias = "sur";

            // Define filter query.Criteria
            var adminFilter = new FilterExpression(LogicalOperator.Or);
            adminFilter.AddCondition("name", ConditionOperator.Equal, "System Administrator");
            adminFilter.AddCondition("name", ConditionOperator.Equal, "Systeembeheerder");
            query.Criteria.AddFilter(adminFilter);

            var userFilter = new FilterExpression();
            userFilter.AddCondition("sur", "systemuserid", ConditionOperator.Equal, userid);
            query.Criteria.AddFilter(userFilter);

            var adminRoles = OrganizationService.RetrieveMultiple(query);

            return adminRoles?.Entities?.Count > 0;
        }

        public EntityCollection RetrieveAll(QueryExpression query, int pageSize = 250)
        {
            var collection = new EntityCollection();
            query.PageInfo.PageNumber = 1;
            query.PageInfo.Count = pageSize;
            query.PageInfo.PagingCookie = null;

            EntityCollection tempCollection;
            do
            {
                tempCollection = OrganizationService.RetrieveMultiple((QueryBase)query);
                PagingInfo pageInfo = query.PageInfo;
                int num = pageInfo.PageNumber + 1;
                pageInfo.PageNumber = num;
                query.PageInfo.PagingCookie = tempCollection.PagingCookie;
                collection.Entities.AddRange(tempCollection.Entities);
            }
            while (tempCollection.MoreRecords);

            collection.EntityName = query.EntityName;
            collection.MoreRecords = false;
            collection.TotalRecordCount = collection.Entities.Count;

            return collection;
        }

        public EntityCollection RetrieveAll(QueryByAttribute query, int pageSize = 250)
        {
            var collection = new EntityCollection();
            query.PageInfo = new PagingInfo();
            query.PageInfo.PageNumber = 1;
            query.PageInfo.Count = pageSize;
            query.PageInfo.PagingCookie = null;

            EntityCollection entityCollection2;
            do
            {
                entityCollection2 = OrganizationService.RetrieveMultiple((QueryBase)query);
                PagingInfo pageInfo = query.PageInfo;
                int num = pageInfo.PageNumber + 1;
                pageInfo.PageNumber = num;
                query.PageInfo.PagingCookie = entityCollection2.PagingCookie;
                collection.Entities.AddRange(entityCollection2.Entities);
            }
            while (entityCollection2.MoreRecords);

            collection.EntityName = query.EntityName;
            collection.MoreRecords = false;
            collection.TotalRecordCount = collection.Entities.Count;

            return collection;
        }

        public SetStateResponse SetState(EntityReference entity, int state, int status)
        {
            Trace("Set {0} \"{1}\" state to {2}-{3}", entity.LogicalName, entity.Id, state, status);

            // Create the Request Object
            var req = new SetStateRequest();

            // Set the Request Object's Properties
            req.State = new OptionSetValue(state);
            req.Status = new OptionSetValue(status);

            // Point the Request to the case whose state is being changed
            req.EntityMoniker = entity;

            // Execute the Request
            return (SetStateResponse)OrganizationService.Execute(req);
        }

        public void Trace(string format, params object[] args)
        {
            if (_traceBuffer == null) _traceBuffer = new StringBuilder();

            var txt = (args == null || args.Length == 0) ? format : string.Format(format, args);

            _traceBuffer.AppendLine(txt);

            if (_tracingService == null)
                Console.WriteLine(txt);
            else
                _tracingService.Trace(txt);
        }

        public void Update(string entitylogicalname, Guid id, string attribute, object value)
        {
            var entity = new Entity(entitylogicalname, id);
            entity.Attributes[attribute] = value;
            this.OrganizationService.Update(entity);
        }

        #endregion Helper Methods
    }
}
