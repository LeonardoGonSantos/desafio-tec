CREATE TABLE IF NOT EXISTS outbox (
    id UUID PRIMARY KEY,
    aggregate_id UUID NOT NULL,
    event_type VARCHAR(100) NOT NULL,
    payload TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    published_at TIMESTAMP NULL,
    integration_status VARCHAR(20) DEFAULT 'pending',
    integration_at TIMESTAMP NULL,
    integration_error TEXT NULL,
    retry_count INTEGER DEFAULT 0
);

COMMENT ON TABLE outbox IS 'Outbox Pattern - eventos para publicação no RabbitMQ';
COMMENT ON COLUMN outbox.integration_status IS 'pending, success, failed';
