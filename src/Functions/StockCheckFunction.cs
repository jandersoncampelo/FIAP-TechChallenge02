using FIAP_TechChallenge02.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FIAP_TechChallenge02.Functions
{
    public class StockCheckFunction
    {

        [Function(nameof(StockCheckFunction))]
        public async Task<bool> Run([ActivityTrigger] Order order, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("StockCheckFunction");
            logger.LogInformation("Checking stock for produc {name}.", order.ProductName);

            // Simulate stock check

            await Task.Delay(500);

            return true;
        }
    }
}