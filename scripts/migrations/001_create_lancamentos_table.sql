CREATE TABLE IF NOT EXISTS lancamentos (
    id UUID PRIMARY KEY,
    tipo INTEGER NOT NULL CHECK (tipo IN (1, 2)),
    valor NUMERIC(15, 2) NOT NULL CHECK (valor > 0),
    descricao VARCHAR(200) NOT NULL,
    data_lancamento DATE NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE lancamentos IS 'Lançamentos financeiros (débitos e créditos)';
COMMENT ON COLUMN lancamentos.tipo IS '1=Débito, 2=Crédito';
