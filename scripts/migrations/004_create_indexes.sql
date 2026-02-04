CREATE INDEX IF NOT EXISTS idx_lancamentos_data_lancamento ON lancamentos(data_lancamento);
CREATE INDEX IF NOT EXISTS idx_lancamentos_created_at ON lancamentos(created_at DESC);
CREATE INDEX IF NOT EXISTS idx_lancamentos_data_created ON lancamentos(data_lancamento, created_at DESC);

CREATE INDEX IF NOT EXISTS idx_outbox_published_at ON outbox(published_at) WHERE published_at IS NULL;
CREATE INDEX IF NOT EXISTS idx_outbox_created_at ON outbox(created_at);

CREATE UNIQUE INDEX IF NOT EXISTS idx_consolidado_data ON consolidado_diario(data);
CREATE INDEX IF NOT EXISTS idx_consolidado_updated_at ON consolidado_diario(updated_at DESC);
