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
            var statusEstoque = await context.CallActivityAsync<bool>(nameof(StockCheckFunction), order);

            // Etapa 2: Confirmar pagamento
            var pagamentoConfirmado = await context.CallActivityAsync<bool>(nameof(PaymentConfirmFunction), order);

            // Etapa 3: Faturar o pedido
            var pedidoFaturado = await context.CallActivityAsync<bool>(nameof(InvoiceOrderFunction), order);

            // Etapa 4: Despachar produto
            var produtoDespachado = await context.CallActivityAsync<bool>(nameof(DispatchProductFunction), order);
        }

        [Function("OrderApprovalDurableFunctions_HttpStart")]
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

            var response = req.CreateResponse(HttpStatusCode.Created);

            return response;
        }
    }
}
