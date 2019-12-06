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
using System.Collections.Generic;

namespace BrainTreeTestService
{
    public static class Refund
    {
        [FunctionName("Refund")]
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

            string transactionId = req.Query["transactionId"];
            
            Result<Transaction> result = gateway.Transaction.Refund(transactionId, new TransactionRefundRequest { });

            if (!result.IsSuccess())
            {
                var errorsStr = String.Join(", ", result.Errors.DeepAll());
                return new BadRequestObjectResult("Cannot refund " + result.Message + " " + errorsStr);
            }
            else
            {
                Transaction refund = result.Target;
                return (ActionResult)new OkObjectResult($"{refund.Amount}");
            }
        }
    }
}
