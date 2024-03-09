using FIAP_TechChallenge02.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FIAP_TechChallenge02.Functions
{
    public class InvoiceOrderFunction
    {
        [Function(nameof(InvoiceOrderFunction))]
        public async Task<bool> Run([ActivityTrigger] Order order, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("InvoiceOrderFunction");
            logger.LogInformation("Invoicing Order {Id}.", order.Id);

            // Simulate

            // Etapa 4: Enviar e-mail com a nota fiscal

            await Task.Delay(500);

            return true;
        }
    }
}
