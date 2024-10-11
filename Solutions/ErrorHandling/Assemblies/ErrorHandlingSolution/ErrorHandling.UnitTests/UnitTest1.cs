using ErrorHandlingSolution.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace ErrorHandling.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private CrmServiceClient _service = null;

        public UnitTest1()
        {
            _service = new CrmServiceClient("AuthType=OAuth;Url=https://glenn-dev.crm4.dynamics.com/;ClientId={51f81489-12ee-4a9e-aaae-a2591f45987d};LoginPrompt=Auto;RedirectUri=app://58145b91-0c36-4500-8554-080854f2ac97/");
        }

        [TestMethod]
        public void ProcesError()
        {
            string json =
                @"[
                  {
                    ""name"": ""Compose_3"",
                    ""inputs"": ""test"",
                    ""outputs"": ""test"",
                    ""startTime"": ""2024-05-03T14:38:58.3343235Z"",
                    ""endTime"": ""2024-05-03T14:38:58.3350451Z"",
                    ""trackingId"": ""5c0ae863-b287-4122-8be7-7a524c20f91e"",
                    ""clientTrackingId"": ""08584868597476326725203738886CU107"",
                    ""clientKeywords"": [
                      ""testFlow""
                    ],
                    ""code"": ""OK"",
                    ""status"": ""Succeeded""
                  },
                  {
                    ""name"": ""Update_a_row"",
                    ""inputs"": {
                                    ""host"": {
                                        ""apiId"": ""subscriptions/c6c6344b-0825-4478-a78d-c56f2e99ebe1/providers/Microsoft.Web/locations/westeurope/runtimes/europe-002/apis/commondataserviceforapps"",
                        ""connectionReferenceName"": ""shared_commondataserviceforapps"",
                        ""operationId"": ""UpdateRecord""
                                    },
                      ""parameters"": {
                                        ""entityName"": ""contacts"",
                        ""recordId"": ""162b6a27-89fd-e711-a836-000d3a33a7cb"",
                        ""item/parentcustomerid_account@odata.bind"": ""account(test)""
                      }
                                },
                    ""outputs"": {
                                    ""statusCode"": 400,
                      ""headers"": {
                                        ""Cache-Control"": ""no-cache"",
                        ""x-ms-service-request-id"": ""ee87c96c-b621-4e8c-99a2-46cb70707c1a,cc3bf95d-8f4e-434d-9531-ed2dfcf893e5"",
                        ""Set-Cookie"": ""ARRAffinity=344ca267d2f3ac1b94dd3c31b69816c1c6ef7949e5093fd359f75a123339e3b915134d20c556b0b34b9b6ae43ec3f5dcdad61788de889ffc592af7aca85fc1c508DC6B82F5432F00217782583; path=/; secure; HttpOnly,ReqClientId=771f516d-065a-44da-8327-f81eaecf0fee; expires=Thu, 03-May-2074 14:38:58 GMT; path=/; secure; HttpOnly,ARRAffinity=344ca267d2f3ac1b94dd3c31b69816c1c6ef7949e5093fd359f75a123339e3b915134d20c556b0b34b9b6ae43ec3f5dcdad61788de889ffc592af7aca85fc1c508DC6B82F5432F00217782583; path=/; secure; HttpOnly"",
                        ""Strict-Transport-Security"": ""max-age=31536000; includeSubDomains"",
                        ""REQ_ID"": ""cc3bf95d-8f4e-434d-9531-ed2dfcf893e5,cc3bf95d-8f4e-434d-9531-ed2dfcf893e5"",
                        ""CRM.ServiceId"": ""CRMAppPool"",
                        ""AuthActivityId"": ""1ec40236-005b-47bc-b68e-27fa82443dda"",
                        ""x-ms-dop-hint"": ""4"",
                        ""x-ms-ratelimit-time-remaining-xrm-requests"": ""1,200.00"",
                        ""x-ms-ratelimit-burst-remaining-xrm-requests"": ""7999"",
                        ""OData-Version"": ""4.0"",
                        ""X-Source"": ""1631501881372712515323459212108843012712723089739145382371222064519913788161919874,870732501291201342128885013825212819156363234221202492518014025123424265968232"",
                        ""Public"": ""OPTIONS,GET,HEAD,POST"",
                        ""Date"": ""Fri, 03 May 2024 14:38:59 GMT"",
                        ""Allow"": ""OPTIONS,GET,HEAD,POST"",
                        ""Content-Type"": ""application/json; odata.metadata=full"",
                        ""Expires"": ""-1"",
                        ""Content-Length"": ""160""
                      },
                      ""body"": {
                                        ""error"": {
                                            ""code"": ""0x80060888"",
                          ""message"": ""URL was not parsed due to an ODataUnrecognizedPathException. Resource not found for the segment provided in the URL.""
                                        }
                                    }
                                },
                    ""startTime"": ""2024-05-03T14:38:58.3915038Z"",
                    ""endTime"": ""2024-05-03T14:38:59.893752Z"",
                    ""trackingId"": ""2d6f33b5-4203-4651-b446-e36b97a82938"",
                    ""clientTrackingId"": ""08584868597476326725203738886CU107"",
                    ""clientKeywords"": [
                      ""testFlow""
                    ],
                    ""code"": ""BadRequest"",
                    ""status"": ""Failed"",
                    ""failureCause"": """"
                  },
                  {
                                ""name"": ""compose_taht_is_skipped"",
                    ""startTime"": ""2024-05-03T14:38:59.9374473Z"",
                    ""endTime"": ""2024-05-03T14:38:59.9384622Z"",
                    ""trackingId"": ""52f2b240-23f7-4b04-8b67-2527419093fa"",
                    ""clientTrackingId"": ""08584868597476326725203738886CU107"",
                    ""clientKeywords"": [
                      ""testFlow""
                    ],
                    ""code"": ""ActionSkipped"",
                    ""status"": ""Skipped"",
                    ""error"": {
                                    ""code"": ""ActionConditionFailed"",
                      ""message"": ""The execution of template action 'compose_taht_is_skipped' is skipped: the 'runAfter' condition for action 'Update_a_row' is not satisfied. Expected status values 'Succeeded' and actual value 'Failed'.""
                    }
                            },
                  {
                                ""name"": ""Condition_2"",
                    ""inputs"": {
                                    ""expressionResult"": true
                    },
                    ""startTime"": ""2024-05-03T14:38:58.3364277Z"",
                    ""endTime"": ""2024-05-03T14:38:58.3901741Z"",
                    ""trackingId"": ""aa707954-78df-444e-afae-4bf7d270b90e"",
                    ""clientTrackingId"": ""08584868597476326725203738886CU107"",
                    ""clientKeywords"": [
                      ""testFlow""
                    ],
                    ""status"": ""Succeeded""
                  }
                ]";

            ErrorService service = new ErrorService(_service);
            service.ProcessError(json, null);



        }
    }
}
