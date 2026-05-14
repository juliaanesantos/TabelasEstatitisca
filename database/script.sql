-- ============================================================
-- Microserviço de Estatísticas - Script de Criação
-- Plataforma Marketplace
-- ============================================================
-- As views abaixo consultam dados das tabelas de outros
-- microserviços (produto, pedido, cliente, avaliacao, etc.)
-- Execute este script APÓS a criação das tabelas dos demais
-- microserviços.
-- ============================================================

-- View: Média de avaliação por produto
CREATE OR REPLACE VIEW public.v_media_avaliacao_produto AS
SELECT
    p.id AS produtoid,
    p.descricao AS nomeproduto,
    COALESCE(AVG(a.nota), 0) AS mediaavaliacao,
    COUNT(a.id) AS totalavaliacoes
FROM public.produto p
LEFT JOIN public.avaliacao a ON a.produtoid = p.id
GROUP BY p.id, p.descricao
ORDER BY mediaavaliacao DESC;

-- View: Média de venda por produto
CREATE OR REPLACE VIEW public.v_media_venda_produto AS
SELECT
    p.id AS produtoid,
    p.descricao AS nomeproduto,
    COALESCE(SUM(pc.quantidade), 0) AS quantidadevendida,
    COALESCE(SUM(pc.total), 0) AS valortotal,
    CASE
        WHEN COUNT(pc.id) > 0 THEN COALESCE(SUM(pc.total) / COUNT(pc.id), 0)
        ELSE 0
    END AS mediavenda
FROM public.produto p
LEFT JOIN public.pedidocliente pc ON pc.idproduto = p.id
GROUP BY p.id, p.descricao
ORDER BY quantidadevendida DESC;

-- View: Média de vendas por cliente
CREATE OR REPLACE VIEW public.v_media_vendas_cliente AS
SELECT
    c.id AS clienteid,
    c.nome AS nomecliente,
    COUNT(p.id) AS quantidadepedidos,
    COALESCE(SUM(p.valor_total), 0) AS totalvendas,
    CASE
        WHEN COUNT(p.id) > 0 THEN COALESCE(SUM(p.valor_total) / COUNT(p.id), 0)
        ELSE 0
    END AS mediavendas
FROM public.cliente c
LEFT JOIN public.pedido p ON p.idcliente = c.id
GROUP BY c.id, c.nome
ORDER BY totalvendas DESC;

-- View: Total de vendas geral
CREATE OR REPLACE VIEW public.v_total_vendas AS
SELECT
    COUNT(id) AS totalpedidos,
    COALESCE(SUM(valor_total), 0) AS totalvendas
FROM public.pedido;

-- View: Painel de estatísticas do dia
CREATE OR REPLACE VIEW public.v_painel_estatisticas AS
SELECT
    'Novos Clientes' AS indicador,
    COUNT(*)::text AS valor,
    'Contas criadas hoje' AS detalhe
FROM public.cliente
WHERE data_criacao::date = CURRENT_DATE
UNION ALL
SELECT
    'Faturamento' AS indicador,
    COALESCE(SUM(valor_total), 0)::text AS valor,
    'Total de vendas em R$' AS detalhe
FROM public.pedido
WHERE data_pedido::date = CURRENT_DATE
UNION ALL
SELECT
    'Total Pedidos' AS indicador,
    COUNT(*)::text AS valor,
    'Pedidos realizados hoje' AS detalhe
FROM public.pedido
WHERE data_pedido::date = CURRENT_DATE;
