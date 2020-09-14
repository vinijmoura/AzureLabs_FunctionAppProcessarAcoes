using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using FunctionAppProcessarAcoes.Models;
using FunctionAppProcessarAcoes.Validators;
using FunctionAppProcessarAcoes.Data;

namespace FunctionAppProcessarAcoes
{
    public class ProcessarAcoes
    {
        private readonly AcoesRepository _repository;

        public ProcessarAcoes(AcoesRepository repository)
        {
            _repository = repository;
        }

        [FunctionName("ProcessarAcoes")]
        public void Run([ServiceBusTrigger("queue-acoes", Connection = "AzureServiceBus")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"ProcessarAcoes V2 - Dados: {myQueueItem}");

            Acao acao = null;
            try
            {
                acao = JsonSerializer.Deserialize<Acao>(myQueueItem,
                    new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch
            {
                log.LogError("ProcessarAcoes V2 - Erro durante a deserializacao!");
            }

            if (acao != null)
            {
                var validationResult = new AcaoValidator().Validate(acao);
                if (validationResult.IsValid)
                {
                    log.LogInformation($"ProcessarAcoes V2 - Dados pos formatacao: {JsonSerializer.Serialize(acao)}");
                    _repository.Save(acao);
                    log.LogInformation("ProcessarAcoes V2 - Acao registrada com sucesso!");
                }
                else
                {
                    log.LogError("ProcessarAcoes V2 - Dados invalidos para a Acao");
                    foreach (var error in validationResult.Errors)
                        log.LogError($"ProcessarAcoes V2 - {error.ErrorMessage}");
                }
            }
        }
    }
}
