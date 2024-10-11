using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHandlingSolution.Models
{
    public class FlowResult
    {
        public string name { get; set; }
        public object inputs { get; set; }
        public object outputs { get; set; }
        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }
        public string trackingId { get; set; }
        public string clientTrackingId { get; set; }
        public List<string> clientKeywords { get; set; }
        public string code { get; set; }
        public string status { get; set; }
        public string failureCause { get; set; }
        public Error error { get; set; }
    }
    public class Error
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class Outputs
    {
        public int? statusCode { get; set; }
        public Headers headers { get; set; }
        public Body body { get; set; }
    }

    public class Headers
    {
        [JsonProperty("Cache-Control")]
        public string CacheControl { get; set; }

        [JsonProperty("x-ms-service-request-id")]
        public string xmsservicerequestid { get; set; }

        [JsonProperty("Set-Cookie")]
        public string SetCookie { get; set; }

        [JsonProperty("Strict-Transport-Security")]
        public string StrictTransportSecurity { get; set; }
        public string REQ_ID { get; set; }

        [JsonProperty("CRM.ServiceId")]
        public string CRMServiceId { get; set; }
        public string AuthActivityId { get; set; }

        [JsonProperty("x-ms-dop-hint")]
        public string xmsdophint { get; set; }

        [JsonProperty("x-ms-ratelimit-time-remaining-xrm-requests")]
        public string xmsratelimittimeremainingxrmrequests { get; set; }

        [JsonProperty("x-ms-ratelimit-burst-remaining-xrm-requests")]
        public string xmsratelimitburstremainingxrmrequests { get; set; }

        [JsonProperty("OData-Version")]
        public string ODataVersion { get; set; }

        [JsonProperty("X-Source")]
        public string XSource { get; set; }
        public string Public { get; set; }
        public string Date { get; set; }
        public string Allow { get; set; }

        [JsonProperty("Content-Type")]
        public string ContentType { get; set; }
        public string Expires { get; set; }

        [JsonProperty("Content-Length")]
        public string ContentLength { get; set; }
    }

    public class Body
    {
        public Error error { get; set; }
    }

    public class Output
    {
        public Outputs outputs { get; set; }
        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }
        public string trackingId { get; set; }
        public string clientTrackingId { get; set; }
        public List<string> clientKeywords { get; set; }
        public string code { get; set; }
        public string status { get; set; }
        public string failureCause { get; set; }
    }



}
