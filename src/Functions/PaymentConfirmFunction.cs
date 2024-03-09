using FIAP_TechChallenge02.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FIAP_TechChallenge02.Functions
{
    public class PaymentConfirmFunction
    {

        [Function(nameof(PaymentConfirmFunction))]
        public async Task<bool> Run([ActivityTrigger] Order order, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("PaymentConfirmFunction");
            logger.LogInformation("Confirming payment for order {id}.", order.Id);

            // Simulate

            await Task.Delay(500);

            return true;
        }
    }
}