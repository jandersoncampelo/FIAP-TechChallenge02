using FIAP_TechChallenge02.Entities;
using FIAP_TechChallenge02.Functions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace Purchase.OrderApproval
{
    public static class OrderApprovalOrchestration
    {
        [Function(nameof(OrderApprovalOrchestration))]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context, Order order)
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(OrderApprovalOrchestration));
            logger.LogInformation("Iniciando Processamento do pedido");
            
            // Etapa 1: Verificar estoque
            await context.CallActivityAsync<bool>(nameof(StockCheckFunction), order);

            // Etapa 2: Confirmar pagamento
            await context.CallActivityAsync<bool>(nameof(PaymentConfirmFunction), order);

            // Etapa 3: Faturar o pedido
            await context.CallActivityAsync<bool>(nameof(InvoiceOrderFunction), order);

            // Etapa 4: Despachar produto
            await context.CallActivityAsync<bool>(nameof(DispatchProductFunction), order);
        }

        [Function("OrderApprovalStart")]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("OrderApprovalDurableFunctions_HttpStart");
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var order = JsonConvert.DeserializeObject<Order>(requestBody);

            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(OrderApprovalOrchestration), order);

            logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            //var result =  await client.CreateCheckStatusResponse(req, instanceId);

            // Return message to the client Pedido Criado
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(JsonConvert.SerializeObject(new { message = "Pedido Enviado para Aprovação!", instanceId }));
            return response;
        }
    }
}
