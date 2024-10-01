using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace ApiHangFire.Api.Controllers
{
    public class ProdutosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("login")]
        public String Login()
        {
            // Enqueue - Executado imediatamente apenas uma vez
            var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("Bem-Vindo a Loja Virtual"));

            return $"Job ID {jobId}. Email de boas Vindas enviado ao cliente!";
        }

        [HttpGet]
        [Route("produtocheckout")]
        public String CheckoutProduto()
        {
            // Schedule - Este job é executado somente uma vez após um tempo pré-determinado
            var jobId = BackgroundJob.Schedule(() =>
                Console.WriteLine("Seu produto foi incluído no checkout"),
                TimeSpan.FromSeconds(20)
            );

            return $"Job ID {jobId}. Produto adicionado ao seu checkout com sucesso!";
        }

        [HttpGet]
        [Route("produtopagamento")]
        public String ProdutoPagamento()
        {
            // Enqueue - Executado apenas uma vez
            var parentJobId = BackgroundJob.Schedule(() => Console.WriteLine("Pagamento enviado para processamento"), TimeSpan.FromSeconds(30));

            // ContinueJobWith - Executado após a conclusão de um outro job "pai"
            var jobId = BackgroundJob.ContinueJobWith(parentJobId, () => Console.WriteLine("Pagamento processado. Email enviado ao cliente"));

            return "Você concluiu o pagamento do seu produto com sucesso!";
        }

        [HttpGet]
        [Route("ofertasdiarias")]
        public String OfertasDiarias()
        {
            // AddOrUpdate - Execução recorrente dentro de um cronograma especificado
            RecurringJob.AddOrUpdate(
                recurringJobId: "EnvioDeOfertas",
                methodCall: () => EnviarOfertas(),
                cronExpression: "* * * * *", // https://crontab.guru/#0_6_*_*_*
                options: new RecurringJobOptions
                {
                    QueueName = "default",
                    TimeZone = TimeZoneInfo.Local
                }
            );

            return "Oferta Enviada!";
        }

        public void EnviarOfertas()
        {
            Console.WriteLine("Envio de produtos similares e sugestões de compras");
        }
    }
}
