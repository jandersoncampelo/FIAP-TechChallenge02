using FIAP_TechChallenge02.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FIAP_TechChallenge02.Functions
{
    public class DispatchProductFunction
    {
        [Function(nameof(DispatchProductFunction))]
        public async Task<bool> Run([ActivityTrigger] Order order, FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger(nameof(DispatchProductFunction));
            logger.LogInformation("Dispatching product {ProductName}!", order.ProductName);

            // Simulate

            // Etapa 6: Enviar e-mail informando o despacho

            await Task.Delay(500);

            return true;
        }
    }
}
