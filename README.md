# Microserviço de Estatísticas - Marketplace

## Integrantes
Juliane, Gustavo e Jenifer

## Descrição
Microserviço responsável por fornecer dados estatísticos da plataforma de marketplace, incluindo médias de avaliação de produtos, vendas por produto, vendas por cliente e total de vendas.

## Tecnologias
- C# / .NET 8
- ASP.NET Web API
- PostgreSQL
- Swagger

## Estrutura do Projeto

```
src/
├── Ftec.ProjetosWeb.Estatistica.Api/          # API REST (Controllers)
├── Ftec.ProjetosWeb.Estatistica.Aplicacao/     # Camada de aplicação (DTOs, Adapters, Services)
├── Ftec.ProjetosWeb.Estatistica.Dominio/       # Camada de domínio (Entidades, Interfaces)
└── Ftec.ProjetosWeb.Estatistica.Persistencia/  # Camada de persistência (Repositórios)
database/
    └── script.sql                              # Script de criação das views
docs/
README.md
```

## Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/Estatistica/painel-hoje` | Painel do dia (novos clientes, faturamento, top produto) |
| GET | `/api/Estatistica/media-avaliacao-produto` | Média de avaliação por produto |
| GET | `/api/Estatistica/media-venda-produto` | Média de venda por produto |
| GET | `/api/Estatistica/media-vendas-cliente` | Média de vendas por cliente |
| GET | `/api/Estatistica/total-vendas` | Total de vendas geral |

## Como executar

1. Configure a connection string do PostgreSQL em `appsettings.json`
2. Execute o script `database/script.sql` no banco de dados
3. Compile e execute:

```bash
dotnet run --project src/Ftec.ProjetosWeb.Estatistica.Api
```

4. Acesse o Swagger em: `http://localhost:5168/swagger`
