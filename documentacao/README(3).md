# ProjetoWeb: API de Carrinho
Projeto desenvolvido para a disciplina de Projetos Web da Uniftec. Período letivo 2/2026

## Objetivo
Desenvolver uma API para a exibição de um carrinho, contendo os pedidos dos usuários, implementando as definições REST, permitindo ser integrada com outras aplicações

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
| TextoStatusPedido  | string  | Descreve o status do pedido do campo "StatusPedido" |
| ValorTotal | decimal  | Calcula o valor total do pedido com seus itens |
| CEPEnderecoEntrega | string  | Descreve o CEP em que o pedido será entregue |
| NumeroEnderecoEntrega | string  | Descreve o número do endereço em que o pedido será entregue |

- Produto (Abstração)
  
| Cmpo  | Tipo | Descrição |
| ------------- | ------------- | ------------- |
| Id  | Guid  | Id do produto  |
| PedidoId | Guid | Id do pedido em que ele pertence |
| ProdutoId | Guid | Id do produto na tabela de produtos (consulta microsserviço) |
| Quantidade | int | Quantidade escolhida para o produto |
| Preco | decimal | Valor do produto unitário (consulta em microsserviço) |
| Disponivel | bool | Indica se o produto está disponível para compra ou não |

- Carrinho

| Cmpo  | Tipo | Descrição |
| ------------- | ------------- | ------------- |
| UsuarioId  | Guid  | Id do usuário atrelado aos pedidos  |
| PedidosModel  | Lista pedidos  | Lista contendo todos os pedidos do usuários  |
| ValorTotalCarrinho  | decimal  | Valor total do carrinho, somando todos os pedidos do usuário  |

- AtualizacaoPedido (entidade para atualizarmos algumas informações de um pedido através do carrinho)

| Cmpo  | Tipo | Descrição |
| ------------- | ------------- | ------------- |
| PedidoId  | Guid  | Id do pedido que será atualizado  |
| StatusPedido  | int  | Novo status do pedido que será atualizado: Pendente pagamento (0), Concluído (1) e Cancelado (-1)  |

### Endpoints
- GET - api/Carrinho/{usuarioId} - Lista todos os pedidos salvos no banco de dados para o usuário definido (OK)

Resposta:
```
{
  "usuarioId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "pedidosModel": [
    {
      "id": "68353014-34b2-4859-8150-b7b3e0fd3d7c",
      "usuarioId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "produtosModel": [
        {
          "id": "21226bb1-e99b-4009-93ff-5e41f94cc7d3",
          "pedidoId": "68353014-34b2-4859-8150-b7b3e0fd3d7c",
          "produtoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
          "quantidade": 21,
          "preco": 10,
          "disponivel": false
        },
        {
          "id": "d682b84a-1348-4458-9af3-67aaa369bf88",
          "pedidoId": "68353014-34b2-4859-8150-b7b3e0fd3d7c",
          "produtoId": "3fa85f64-5717-4562-b3fc-2c963f66af99",
          "quantidade": 20,
          "preco": 10,
          "disponivel": false
        }
      ],
      "dataPedido": "2026-05-13T00:00:00",
      "statusPedido": -1,
      "textoStatusPedido": "Cancelado",
      "valorTotal": 410,
      "cepEnderecoEntrega": "",
      "numeroEnderecoEntrega": ""
    }
  ],
  "valorTotalCarrinho": 410
}
```

- POST - api/Carrinho/AtualizarStatusPedido - Atualizamos o status do pedido especificado (Pendente pagamento (0), Concluído (1) e Cancelado (-1)) (OK)

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

- DELETE - api/Carrinho/LimparCarrinho/{usuarioId} - Limpa o carrinho de um usuário específico (OK)

Resposta:
```
Limpar Carrinho Usuário - Carrinho limpo com sucesso
```

- DELETE - api/Carrinho/DeletePedido/{pedidoId} - Remove um pedido específico (OK)

Resposta:
```
Deletar Pedido - Pedido removido com sucesso
```

### Banco de dados
- PostgreSQL
- Scripts de criação de tabelas necessárias:
```
CREATE TABLE public.carrinho (
	id varchar NULL,
	usuarioid varchar NULL,
	valor_total numeric NULL
);
```
