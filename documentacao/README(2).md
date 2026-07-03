# ProjetoWeb: API de Pedidos
Projeto desenvolvido para a disciplina de Projetos Web da Uniftec. Período letivo 2/2026

## Objetivo
Desenvolver uma API para a consulta de pedidos, implementando as definições REST, permitindo ser integrada com outras aplicações

## Desenvolvimento
API construída em C# com o framework .NET Core

### Entidades
- Pedido
  
| Cmpo  | Tipo | Descrição |
| ------------- | ------------- | ------------- |
| Id | Guid | Id do pedido |
| UsuarioId  | Guid  | Id do usuário que fez o pedido  |
| ProdutosModel  | Lista de produtos  | Lista de produtos contidos dentro do pedido. Apenas uma abstração dos produtos, com informações essênciais  |
| DataPedido  | DateTime  | Data em que o pedido foi atualizado pela última vez  |
| StatusPedido  | int  | Indica o status do pedido: Pendente pagamento (0), Concluído (1) e Cancelado (-1) |
| ValorTotal | decimal  | Calcula o valor total do pedido com seus itens |
| CEPEnderecoEntrega | string  | CEP do endereço de entrega do pedido |
| NumeroEnderecoEntrega | string  | Número do endereço de entrega do pedido |


- Produto (Abstração)
  
| Cmpo  | Tipo | Descrição |
| ------------- | ------------- | ------------- |
| Id  | Guid  | Id do produto  |
| PedidoId | Guid | Id do pedido em que ele pertence |
| ProdutoId | Guid | Id do produto na tabela de produtos (consulta microsserviço) |
| Quantidade | int | Quantidade escolhida para o produto |
| Preco | decimal | Valor do produto unitário (consulta em microsserviço) |
| Disponivel | bool | Indica se o produto está disponível para compra ou não |
| Excluido | bool | Indica se o produto está excluido no banco de dados ou não |

- AtualizacaoPedido

| Cmpo  | Tipo | Descrição |
| ------------- | ------------- | ------------- |
| PedidoId | Guid | Id do pedido que será atualizado |
| StatusPedido  | int  | Novo status do pedido |


- AtualizarEnderecoPedido

| Cmpo  | Tipo | Descrição |
| ------------- | ------------- | ------------- |
| PedidoId | Guid | Id do pedido que será atualizado o endereço |
| CEPEnderecoEntrega | string | Novo CEP do endereço de entrega do pedido |
| NumeroEnderecoEntrega | string | Novo número do endereço de entrega do pedido |

### Endpoints
- GET - api/Pedido - Lista todos os pedidos salvos no banco de dados (OK)

Resposta:
```
[
  {
    "id": "68353014-34b2-4859-8150-b7b3e0fd3d7c",
    "usuarioId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "produtosModel": [
      {
        "id": "3dd836fb-b036-42b4-bbd6-715d2e9d7a42",
        "pedidoId": "68353014-34b2-4859-8150-b7b3e0fd3d7c",
        "produtoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "quantidade": 1,
        "preco": 10,
        "disponivel": false,
		"excluido": false
      }
    ],
    "dataPedido": "2026-05-13T00:00:00",
    "statusPedido": 0,
    "valorTotal": 10,
    "cepEnderecoEntrega": "00000000",
    "numeroEnderecoEntrega": "0000"
  }
]
```

- GET - api/Pedido/{id} - Lista o pedido do id especificado (OK)

Resposta:
```
{
  "id": "4d9bc71c-1220-408a-88a4-5cf494fd310c",
  "usuarioId": "3fa85f64-5717-4562-b3fc-2c963f66af76",
  "produtosModel": [
    {
      "id": "39d03767-87e0-4b92-8bbe-d354bc91b9cd",
      "pedidoId": "4d9bc71c-1220-408a-88a4-5cf494fd310c",
      "produtoId": "3fa85f64-5717-4562-b3fc-2c963f66a776",
      "quantidade": 2,
      "preco": 10,
      "disponivel": false,
      "excluido": false
    }
  ],
  "dataPedido": "2026-05-13T00:00:00",
  "statusPedido": 0,
  "valorTotal": 20,
  "cepEnderecoEntrega": "88888888",
  "numeroEnderecoEntrega": "8888"
}
```

- GET - api/Pedido/GetPedidosUsuario/{usuarioId} - Lista todos os pedidos do usuário especificado (OK)

Resposta:
```
[
  {
    "id": "4d9bc71c-1220-408a-88a4-5cf494fd310c",
    "usuarioId": "3fa85f64-5717-4562-b3fc-2c963f66af76",
    "produtosModel": [
      {
        "id": "39d03767-87e0-4b92-8bbe-d354bc91b9cd",
        "pedidoId": "4d9bc71c-1220-408a-88a4-5cf494fd310c",
        "produtoId": "3fa85f64-5717-4562-b3fc-2c963f66a776",
        "quantidade": 2,
        "preco": 10,
        "disponivel": false,
        "excluido": false
      }
    ],
    "dataPedido": "2026-05-13T00:00:00",
    "statusPedido": 0,
    "valorTotal": 20,
    "cepEnderecoEntrega": "88888888",
    "numeroEnderecoEntrega": "8888"
  },
  {
    "id": "d835374e-1644-48e1-b934-d7a2da33cbaa",
    "usuarioId": "3fa85f64-5717-4562-b3fc-2c963f66af76",
    "produtosModel": [
      {
        "id": "28025c07-149c-43d9-ad58-4374c1d85f79",
        "pedidoId": "d835374e-1644-48e1-b934-d7a2da33cbaa",
        "produtoId": "3fa85f64-5717-4562-b3fc-2c963f66a886",
        "quantidade": 3,
        "preco": 10,
        "disponivel": false,
        "excluido": false
      }
    ],
    "dataPedido": "2026-05-13T00:00:00",
    "statusPedido": 0,
    "valorTotal": 30,
    "cepEnderecoEntrega": "88888888",
    "numeroEnderecoEntrega": "8888"
  }
]
```

- POST - api/Pedido - Insere um pedido no banco de dados (OK)

Envio:
```
{
  "usuarioId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "produtosModel": [
    {
      "produtoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "quantidade": 1
    }
  ],
  "cepEnderecoEntrega": "00000000",
  "numeroEnderecoEntrega": "0000"
}
```

Resposta:
```
Inserção Pedido - Pedido inserido com sucesso
```

- PUT - api/Pedido - Atualiza um pedido no banco de dados (OK)

Envio:
Obs.: O campo "id" é o id do pedido no banco de dados. Nessa atualização, podemos colocar os novos pedidos atualizados, substituindo os antigos, ou não
```
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "statusPedido": 1,
  "produtosModel": [
    {
      "produtoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "quantidade": 2
    }
  ]
}

Ou atualizando somente o status do pedido

{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "statusPedido": 1
}
```

Resposta:
```
Alteração Pedido - Pedido atualizado com sucesso
```

- PUT - api/Pedido/AtualizarStatusPedido - Atualiza o status de um pedido no banco de dados (OK)

Envio:
```
{
  "pedidoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "statusPedido": 0
}
```

Resposta:
```
Atualizar Status Pedido - Pedido atualizado com sucesso
```

- PUT - api/Pedido/AtualizarEnderecoEntregaPedido - Atualiza o endereço de entrega de um pedido no banco de dados (OK)

Envio:
```
{
  "pedidoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "cepEnderecoEntrega": "88888888",
  "numeroEnderecoEntrega": "8888"
}
```

Resposta:
```
Atualizar Endereço Pedido - Endereço do pedido atualizado com sucesso
```

- DELETE - api/Pedido/{id} - Deleta um pedido especificado no banco de dados (OK)

Resposta:
```
Deletar Pedido Específico - Pedido excluido com sucesso
```

- DELETE - api/Pedido/DeletePedidos/{usuarioid} - Deleta todos os pedidos de um usuário especificado (OK)

Resposta:
```
Deletar Pedidos Usuário - Pedidos excluidos com sucesso
```

- DELETE - api/Pedido/DeleteProdutoPedido/{pedidoId}/{produtoId} - Deleta um produto específico de um pedido específico (OK)

Resposta:
```
Deletar Produto Pedido - Produto do pedido excluido com sucesso
```

### Banco de dados
- PostgreSQL
- Scripts de criação de tabelas necessárias:
```
CREATE TABLE public.pedido (
	id varchar NOT NULL,
	usuarioid varchar NULL,
	datapedido date NULL,
	statuspedido int4 NULL,
	cependerecoentrega varchar NULL,
	numeroenderecoentrega varchar NULL
);

CREATE TABLE public.produto_pedido (
	pedidoid varchar NOT NULL,
	id varchar NULL,
	produtoid varchar NULL,
	quantidade varchar NULL
);
```
