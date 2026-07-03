# API de PAGAMENTOS E FRETE


Esta API foi desenvolvida para integrar os principais módulos de um sistema de e-commerce, permitindo a comunicação entre pedidos, cálculo de frete, processamento de pagamentos e gestão de transportadoras.
Ela fornece endpoints REST que permitem:
- Consultar pedidos externos e seus endereços de entrega.
- Calcular e acompanhar o status de fretes.
- Registrar e processar pagamentos, incluindo transações via gateway.
- Gerenciar transportadoras, incluindo cadastro, ativação e desativação.
O objetivo é oferecer uma interface clara e padronizada para que sistemas internos e externos possam interagir de forma segura e eficiente.

📦 Endpoints da API


🔍 Consultas de Pedido
- GET /api/pedido-externo/{pedidoId} → Retorna o valor total de um pedido externo.
{
  "pedidoId": "123e4567-e89b-12d3-a456-426614174000",
  "valorTotal": 250.00
}


📍 Endereço de Entrega
- GET /api/pedido-externo/{pedidoId}/endereco → Retorna o endereço de entrega de um pedido externo.
{
  "cepDestino": "95000-000",
  "numero": "100"
}



🚚 Frete
Base URL: http://pagamento.neurosky.com.br/api/frete

🔍 Consultas
- GET /api/frete → Lista todos os fretes cadastrados.
- GET /api/frete/{id} → Retorna os detalhes de um frete específico.
- GET /api/frete/pedido/{pedidoId} → Retorna o frete vinculado a um pedido.
{
  "idFrete": "222e3333-e44b-55d6-a777-888999000111",
  "pedidoId": "123e4567-e89b-12d3-a456-426614174000",
  "valorFrete": 50.00,
  "statusEntrega": "Pendente",
  "cepDestino": "95000-000"
}


📦 Cálculo de Frete
- POST /api/frete/calcular → Calcula o frete com base nos dados fornecidos.
{
  "pedidoId": "123e4567-e89b-12d3-a456-426614174000",
  "cepOrigem": "90000-000",
  "cepDestino": "95000-000"
}


📌 Fluxo de Status do Frete
O frete segue o fluxo: Pendente → Preparando → Enviado → EmTransito → Entregue
- POST /api/frete/{id}/confirmar → Confirma o frete.
- POST /api/frete/{id}/enviar → Envia o frete.
- POST /api/frete/{id}/em-transito → Marca como em trânsito.
- POST /api/frete/{id}/entregar → Marca como entregue.
{
  "codigoRastreio": "BR123456789"
}


❌ Cancelamento
- DELETE /api/frete/{id} → Cancela o frete.

💳 Pagamento
Base URL: http://pagamento.neurosky.com.br/api/pagamento

🔍 Consultas
- GET /api/pagamento → Lista todos os pagamentos.
- GET /api/pagamento/{id} → Retorna os detalhes de um pagamento.
{
  "pagamentoId": "987e6543-e21b-12d3-a456-426614174999",
  "valorTotal": 250.00,
  "statusPagamento": "Pendente"
}


➕ Criação de Pagamento
- POST /api/pagamento → Registra um novo pagamento.
{
  "pedidoId": "123e4567-e89b-12d3-a456-426614174000",
  "cpfCliente": "12345678900",
  "valorTotal": 250.00,
  "metodoPagamento": "CartaoCredito"
}


🔄 Processamento de Transação
- POST /api/pagamento/transacao → Processa uma transação via gateway.
{
  "pagamentoId": "987e6543-e21b-12d3-a456-426614174999",
  "valor": 250.00,
  "retornoGateway": "Aprovado",
  "statusTransacao": true
}


❌ Cancelamento
- DELETE /api/pagamento/{id} → Cancela um pagamento.

🚛 Transportadora
Base URL: http://pagamento.neurosky.com.br/api/transportadora

🔍 Consultas
- GET /api/transportadora → Lista todas as transportadoras.
- GET /api/transportadora/{id} → Retorna os detalhes de uma transportadora.
{
  "transportadoraId": "111e2222-e33b-44d5-a666-777888999000",
  "nome": "Transportadora Rápida",
  "codigoServico": "EXPRESSO",
  "valorBase": 30.00,
  "prazoMinDias": 2,
  "prazoMaxDias": 5,
  "ativo": true
}


➕ Criação de Transportadora
- POST /api/transportadora → Cadastra uma nova transportadora.
{
  "nome": "Transportadora Econômica",
  "codigoServico": "PADRAO",
  "valorBase": 20.00,
  "prazoMinDias": 5,
  "prazoMaxDias": 10,
  "ativo": true
}


⚙️ Ativação e Desativação
- POST /api/transportadora/{id}/ativar → Ativa uma transportadora.
- POST /api/transportadora/{id}/desativar → Desativa uma transportadora.
{
  "mensagem": "Transportadora ativada com sucesso."
}


{
  "mensagem": "Transportadora desativada com sucesso."
}





