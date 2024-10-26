Arquitetura orientada a serviços (SOA)

Precisaremos considerar alguns aspectos cruciais, desde a separação dos serviços até a integração com o barramento de comunicação que não depende da internet. Vou detalhar um guia passo a passo para implementar essa estrutura em .NET 8.

Passo 1: Estrutura do Projeto e Serviços
Criação dos Microserviços:

Dividimos o projeto em múltiplos microserviços, conforme os componentes apresentados no diagrama: Contas a Pagar, Contas a Receber, Pedidos, Estoque.
Cada microserviço deve ser autônomo, com sua própria base de dados e logicamente isolado, para seguir os princípios de SOA e microserviços.
Configuração do Projeto SOA Principal:

Crie um projeto principal em .NET 8 que será responsável pelo gerenciamento e comunicação entre os serviços.
Inclua uma camada de Orquestração que gerenciará as interações entre os serviços, utilizando um padrão de barramento de eventos (como um sistema de mensageria que substitua a internet, por exemplo, via RabbitMQ ou Apache Kafka local).

Passo 2: Comunicação e Barramento

Implementação de Comunicação por Mensageria:
Escolha uma ferramenta de mensageria (como RabbitMQ, NATS ou ActiveMQ) para atuar como o barramento de comunicação.
Configure os microserviços para publicar e escutar eventos no barramento. Cada operação relevante (como criação de contas a pagar ou atualização de estoque) deve gerar um evento que será transmitido pelo barramento.
Mensagens de Integração:
Defina mensagens padronizadas em JSON ou outro formato adequado, que cada microserviço entenderá e processará.
Cada microserviço deve implementar handlers para as mensagens que ele precisa escutar e processar. Por exemplo, o microserviço de Pedidos pode escutar uma mensagem de Atualização de Estoque.

Passo 3: Estrutura dos Serviços
Contas a Pagar/Receber:

Configure cada um dos serviços financeiros (Contas a Pagar e Contas a Receber) para operar independentemente.
Esses serviços podem expor APIs RESTful para funcionalidades CRUD, mas preferencialmente, comuniquem-se via mensagens publicadas no barramento.
Cada serviço terá seu próprio banco de dados, permitindo alta independência e escalabilidade.
Estoque e Pedidos:

O serviço de Estoque precisa se comunicar com Pedidos para verificar a disponibilidade dos produtos.
Crie endpoints REST e eventos de mensageria para atualizações de estoque e novos pedidos, de forma que Pedidos e Estoque se comuniquem através do barramento.
Passo 4: Integração com "Mercado Pago"
Serviço de Pagamento:

Crie um serviço simulado para Mercado Pago que possa realizar a operação de pagamento dentro do ambiente SOA.

Este serviço pode ser utilizado tanto por Contas a Pagar quanto por Contas a Receber, publicando uma mensagem no barramento ao concluir uma transação.
Interação e Confirmação de Pagamento:

Ao receber uma solicitação de pagamento, o serviço de Mercado Pago publica uma confirmação no barramento, que Contas a Pagar ou Contas a Receber poderão escutar para confirmar o status do pagamento.

Passo 5: Monitoramento e Logging
Implementação de Monitoramento:
Utilize ferramentas como Prometheus e Grafana para monitoramento de performance dos serviços.
Configure logs estruturados em todos os microserviços, armazenando logs críticos e erros em uma base centralizada para fácil consulta.

Passo 6: Testes e Segurança
Testes:
Crie testes unitários e de integração para cada serviço, assegurando que as mensagens estão sendo publicadas e escutadas corretamente no barramento.
Segurança:
Configure autenticação e autorização entre os serviços, caso necessário, usando autenticação por token JWT se cada serviço precisar verificar a identidade do usuário.
Exemplo de Implementação do Serviço de Contas a Pagar
Estrutura do Projeto

SOA_Project
│
├── ContasAPagarService
│   ├── Controllers
│   ├── Repositories
│   ├── Models
│   ├── Services
│   ├── Program.cs
│   └── appsettings.json
│
├── ContasAReceberService
│   └── ...
├── EstoqueService
│   └── ...
└── PedidosService
    └── ...

Criação da Solução Principal e dos Projetos de Microserviço
Abra o Visual Studio ou o Visual Studio Code.

Crie uma Solução ( VSCode ou VStudio2022 ) para agrupar todos os microserviços e o projeto principal de orquestração.

dotnet new sln -o SOAProject
cd SOAProject

Crie cada microserviço como um Projeto de API.


dotnet new webapi -n ContasAPagarService
dotnet new webapi -n ContasAReceberService
dotnet new webapi -n PedidosService
dotnet new webapi -n EstoqueService


Adicione cada projeto à solução.

dotnet sln add ContasAPagarService/ContasAPagarService.csproj
dotnet sln add ContasAReceberService/ContasAReceberService.csproj
dotnet sln add PedidosService/PedidosService.csproj
dotnet sln add EstoqueService/EstoqueService.csproj

Cada um desses serviços será autônomo e terá seu próprio banco de dados. Isso é essencial para garantir a independência dos serviços na arquitetura SOA.

1.2 Configuração Básica dos Projetos de Microserviço
1.Estrutura de Pastas: Dentro de cada projeto, crie pastas para organizar o código. Por exemplo:

Controllers: Para os controladores de API.
Repositories: Para acesso a dados.
Models: Para definir as entidades e modelos de dados.
Services: Para lógica de negócio.
Exemplo de Estrutura em ContasAPagarService:

Crie as pastas Controllers, Repositories, Models, e Services.
Dentro da pasta Models, adicione um modelo de exemplo, como ContaPagar.cs.

Execução e Testes Iniciais:

Execute o projeto ContasAPagarService para verificar se a API está funcionando corretamente.

Execução e Testes Iniciais:

Execute o projeto ContasAPagarService para verificar se a API está funcionando corretamente.

No terminal, execute:

dotnet run --project ContasAPagarService
Acesse http://localhost:{porta}/api/ContaPagar no navegador para verificar a resposta "Serviço de Contas a Pagar funcionando!".

Configuração do RabbitMQ
Instalação do RabbitMQ:

Se ainda não tem RabbitMQ instalado, você pode usar Docker para rodá-lo rapidamente.

docker run -d --hostname rabbit --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
Isso irá iniciar RabbitMQ no seu localhost, acessível na porta 15672 para a interface de administração e 5672 para as conexões dos serviços.

Configuração dos Projetos para Conexão com RabbitMQ:

Em cada projeto (ContasAPagarService, ContasAReceberService, PedidosService, EstoqueService), instale o pacote RabbitMQ.Client para integração.

dotnet add package RabbitMQ.Client

Exemplo de Publicação de Mensagem em ContasAPagarService:

Crie um serviço de comunicação que enviará uma mensagem para o RabbitMQ.
Adicione o código abaixo em um novo arquivo chamado MensagemService.cs na pasta Services do ContasAPagarService.

using RabbitMQ.Client;
using System.Text;

public class MensagemService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MensagemService()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "contasPagarQueue",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
    }

    public void EnviarMensagem(string mensagem)
    {
        var body = Encoding.UTF8.GetBytes(mensagem);
        _channel.BasicPublish(exchange: "",
                             routingKey: "contasPagarQueue",
                             basicProperties: null,
                             body: body);
    }
}

Uso do Serviço de Mensagem no Controller:

No ContaPagarController, inicie e use o MensagemService para enviar uma mensagem ao criar uma nova conta a pagar.

[HttpPost]

public IActionResult Create(ContaPagar contaPagar)
{
    var mensagemService = new MensagemService();
    mensagemService.EnviarMensagem($"Conta a pagar criada: {contaPagar.Descricao} - {contaPagar.Valor}");

    return Ok("Conta a pagar criada com sucesso!");
}

Próximos Passos :

 Configurar a Escuta das Mensagens nos Outros Microserviços: Nos próximos passos, vamos configurar os outros serviços (Contas a Receber, Pedidos, Estoque) para escutar as mensagens do RabbitMQ e responder ou processar ações conforme necessário.

 Estrutura Completa do Banco de Dados e APIs: Configurar os bancos de dados independentes para cada serviço e estruturar suas operações CRUD.

 Monitoramento e Logs: Implementar um sistema de logs e monitoramento para acompanhar a comunicação entre os serviços e desempenho geral.

