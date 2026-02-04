# Summary – Requisitos do Desafio vs Implementação

Requisitos do desafio (desafio-desenvolvedor-backend-nov25.pdf) mapeados para código e documentação do projeto.

---

## Requisitos de Negócio

| Requisito | Implementação | Referência |
|-----------|---------------|------------|
| **Serviço que faça o controle de lançamentos (Transacional)** | API REST para registrar, listar e obter lançamentos (débitos e créditos). Persistência em PostgreSQL com transação e Outbox na mesma unidade de trabalho. | [LancamentosController](../src/FluxoCaixa.API/Controllers/LancamentosController.cs), [RegistrarLancamentoUseCase](../src/FluxoCaixa.Application/UseCases/Lancamentos/RegistrarLancamento/RegistrarLancamentoUseCase.cs), [docs/flows/POST_lancamentos.md](flows/POST_lancamentos.md) |
| **Serviço do consolidado diário (Relatório)** | API REST para consultar saldo consolidado por data. Dados alimentados de forma assíncrona pelo Worker a partir dos eventos de lançamento. | [ConsolidadoController](../src/FluxoCaixa.API/Controllers/ConsolidadoController.cs), [ObterConsolidadoDiarioUseCase](../src/FluxoCaixa.Application/UseCases/Consolidado/ObterConsolidadoDiario/ObterConsolidadoDiarioUseCase.cs), [docs/flows/GET_consolidado.md](flows/GET_consolidado.md) |

---

## Requisitos Técnicos Obrigatórios

| Requisito | Implementação | Referência |
|-----------|---------------|------------|
| **Desenho da solução (diagrama simples de componentes e interações)** | Diagramas Mermaid no README: componentes, estrutura do projeto e fluxo Outbox + Worker. Diagramas de sequência por endpoint na pasta docs/flows. | [README.md](../README.md) (seção Arquitetura), [docs/flows/](flows/) |
| **Deve ser feito usando C#** | Solução em .NET 8 com C#. | [FluxoCaixa.sln](../FluxoCaixa.sln), projetos em `src/` |
| **Testes (TDD/BDD). Código testável e testes cobrindo lógica de negócio** | Testes unitários com xUnit, Moq e FluentAssertions para entidades, value objects e use cases. | [tests/FluxoCaixa.Tests.Unit/](../tests/FluxoCaixa.Tests.Unit/) (Domain Lancamentos/Consolidado, Application) |
| **Boas práticas (SOLID, Clean Code, Design Patterns, DDD)** | DDD (entidades, value objects, domain events), Result Pattern, Repository, Unit of Work, Vertical Slices, Outbox Pattern. | Domínios em `src/FluxoCaixa.Domain.*`, Application em `src/FluxoCaixa.Application/`, [README.md](../README.md) (Decisões Técnicas) |
| **README com instruções claras de como a aplicação funciona e como rodar localmente** | README com visão geral, arquitetura, tecnologias, pré-requisitos, execução via Docker Compose e local, endpoints e testes. | [README.md](../README.md) |
| **Códigos que não funcionarem localmente seguindo as instruções podem ser descartados** | Docker Compose orquestra PostgreSQL, RabbitMQ, migration, API e Worker. Instruções para execução local sem Docker. | [docker-compose.yml](../docker-compose.yml), [README.md](../README.md) (Como Executar) |
| **Hospedar em repositório público (GitHub)** | A cargo do candidato. | - |
| **Todas as documentações de projeto devem estar no repositório** | README, docs/flows (fluxos por endpoint), docs/SUMMARY.md (este arquivo). | [README.md](../README.md), [docs/](.) |

---

## Requisitos Não-Funcionais

| Requisito | Implementação | Referência |
|-----------|---------------|------------|
| **O serviço de controle de lançamento não deve ficar indisponível se o sistema de consolidado diário cair** | API de lançamentos e Worker/consolidado desacoplados. Lançamento persiste na base + outbox na mesma transação; publicação para RabbitMQ e processamento do consolidado são assíncronos. Falha do Worker não impacta o registro de lançamentos. | [RegistrarLancamentoUseCase](../src/FluxoCaixa.Application/UseCases/Lancamentos/RegistrarLancamento/RegistrarLancamentoUseCase.cs), [OutboxPublisherService](../src/FluxoCaixa.Infrastructure/BackgroundServices/OutboxPublisherService.cs), [IntegrationWorkerService](../src/FluxoCaixa.Worker/Services/IntegrationWorkerService.cs) |
| **Em dias de picos, serviço de consolidado recebe 50 req/s com no máximo 5% de perda** | ResponseCache no endpoint de consolidado (5 min). API e Worker stateless, permitindo dimensionamento horizontal. | [ConsolidadoController](../src/FluxoCaixa.API/Controllers/ConsolidadoController.cs) (ResponseCache), [README.md](../README.md) |

---

## Documentação de Fluxos por Endpoint

| Endpoint | Documento |
|----------|-----------|
| POST /api/auth/login | [POST_auth_login.md](flows/POST_auth_login.md) |
| POST /api/lancamentos | [POST_lancamentos.md](flows/POST_lancamentos.md) |
| GET /api/lancamentos | [GET_lancamentos.md](flows/GET_lancamentos.md) |
| GET /api/lancamentos/{id} | [GET_lancamentos_id.md](flows/GET_lancamentos_id.md) |
| GET /api/consolidado/{data} | [GET_consolidado.md](flows/GET_consolidado.md) |

---

## Índice de Arquivos Relevantes

- **Requisitos do desafio:** [desafio-desenvolvedor-backend-nov25.pdf](../desafio-desenvolvedor-backend-nov25.pdf) (raiz do repositório)
- **Visão geral e execução:** [README.md](../README.md)
- **Resumo requisitos vs implementação:** [docs/SUMMARY.md](SUMMARY.md) (este arquivo)
