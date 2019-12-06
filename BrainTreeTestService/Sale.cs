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
    public static class Sale
    {
        [FunctionName("Sale")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var gateway = new BraintreeGateway
            {
                Environment = Braintree.Environment.SANDBOX,
                MerchantId = "zk2jptgp84gnv46n",
                PublicKey = "q3324q3v7xhmhsn6",
                PrivateKey = "e4ba080f8e1c69b8bd1d819fbd9af48f"
            };


            var request = new TransactionRequest
            {
                Amount = 20.00M,
                PaymentMethodNonce = "fake-valid-nonce",
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = gateway.Transaction.Sale(request);
            if (result.IsSuccess())
            {
                Transaction transaction = result.Target;

                return (ActionResult)new OkObjectResult($"{transaction.Id}");
            }
            else
            {
                return new BadRequestObjectResult("Cannot process payment " + result.Message);
            }
        }
    }
}
