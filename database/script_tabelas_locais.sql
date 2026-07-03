CREATE TABLE IF NOT EXISTS public.painel_diario (
    id SERIAL PRIMARY KEY,
    indicador VARCHAR(100),
    valor TEXT,
    detalhe TEXT,
    data_referencia DATE,
    data_atualizacao TIMESTAMP DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS public.media_avaliacao_produto (
    produtoid UUID,
    nomeproduto VARCHAR(255),
    mediaavaliacao DECIMAL(10,2),
    totalavaliacoes INT,
    data_atualizacao TIMESTAMP DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS public.media_venda_produto (
    produtoid UUID,
    nomeproduto VARCHAR(255),
    quantidadevendida INT,
    valortotal DECIMAL(10,2),
    mediavenda DECIMAL(10,2),
    data_atualizacao TIMESTAMP DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS public.media_vendas_cliente (
    clienteid UUID,
    nomecliente VARCHAR(255),
    quantidadepedidos INT,
    totalvendas DECIMAL(10,2),
    mediavendas DECIMAL(10,2),
    data_atualizacao TIMESTAMP DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS public.total_vendas (
    totalpedidos INT,
    totalvendas DECIMAL(10,2),
    data_atualizacao TIMESTAMP DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS public.avaliacoes_diarias (
    nomeproduto VARCHAR(255),
    data_avaliacao DATE,
    quantidade INT,
    soma INT,
    media DECIMAL(10,2),
    data_atualizacao TIMESTAMP DEFAULT NOW()
);
