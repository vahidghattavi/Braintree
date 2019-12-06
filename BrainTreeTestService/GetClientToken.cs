using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Braintree;

namespace BrainTreeTestService
{
    public static class GetClientToken
    {
        [FunctionName("GetClientToken")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                string username = req.Query["username"];
                var gateway = new BraintreeGateway
                {
                    Environment = Braintree.Environment.SANDBOX,
                    MerchantId = "zk2jptgp84gnv46n",
                    PublicKey = "q3324q3v7xhmhsn6",
                    PrivateKey = "e4ba080f8e1c69b8bd1d819fbd9af48f"
                };
                var createReq = new CustomerRequest
                {
                    CustomerId = username
                };

                var custResponse = gateway.Customer.Create(createReq);

                var customer = gateway.Customer.Find(custResponse.Target.Id);

               //If this fails and say "CustomerId" is not found, call it with no customerID
                var clientToken = gateway.ClientToken.Generate(new ClientTokenRequest
                                                                    {
                                                                        CustomerId = customer.Id
                });

                
                return clientToken != null
                    ? (ActionResult)new OkObjectResult($"{clientToken}")
                    : new BadRequestObjectResult("Cannot get a client Token");
            }
            catch (Exception ex)
            {
               
                return new BadRequestObjectResult(ex.Message);
            }

        }
    }
}
