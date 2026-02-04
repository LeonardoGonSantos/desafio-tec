CREATE TABLE IF NOT EXISTS consolidado_diario (
    id UUID PRIMARY KEY,
    data DATE NOT NULL UNIQUE,
    saldo NUMERIC(15, 2) NOT NULL,
    total_creditos NUMERIC(15, 2) NOT NULL DEFAULT 0,
    total_debitos NUMERIC(15, 2) NOT NULL DEFAULT 0,
    quantidade_lancamentos INTEGER NOT NULL DEFAULT 0,
    updated_at TIMESTAMP NOT NULL DEFAULT NOW(),
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE consolidado_diario IS 'Consolidado diário do saldo';
