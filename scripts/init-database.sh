#!/bin/bash
set -e

echo "=========================================="
echo "Inicializando banco de dados FluxoCaixa"
echo "=========================================="

DB_HOST="${DB_HOST:-localhost}"
DB_PORT="${DB_PORT:-5432}"
POSTGRES_USER="${POSTGRES_USER:-postgres}"
POSTGRES_PASSWORD="${POSTGRES_PASSWORD:-postgres}"
DB_NAME="${DB_NAME:-fluxocaixa}"

echo "Aguardando PostgreSQL em $DB_HOST:$DB_PORT..."
until PGPASSWORD=$POSTGRES_PASSWORD psql -h "$DB_HOST" -p "$DB_PORT" -U "$POSTGRES_USER" -c '\q' 2>/dev/null; do
  echo "Aguardando PostgreSQL..."
  sleep 2
done

echo "✓ PostgreSQL está pronto!"

echo "Criando banco de dados '$DB_NAME' se não existir..."
PGPASSWORD=$POSTGRES_PASSWORD psql -h "$DB_HOST" -p "$DB_PORT" -U "$POSTGRES_USER" -tc "SELECT 1 FROM pg_database WHERE datname = '$DB_NAME'" | grep -q 1 || \
  PGPASSWORD=$POSTGRES_PASSWORD psql -h "$DB_HOST" -p "$DB_PORT" -U "$POSTGRES_USER" -c "CREATE DATABASE $DB_NAME"

echo "✓ Banco verificado"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
echo ""
echo "Executando migrations..."
for f in "$SCRIPT_DIR"/migrations/*.sql; do
  if [ -f "$f" ]; then
    echo "  $(basename "$f")"
    PGPASSWORD=$POSTGRES_PASSWORD psql -h "$DB_HOST" -p "$DB_PORT" -U "$POSTGRES_USER" -d "$DB_NAME" -f "$f"
  fi
done

echo ""
echo "=========================================="
echo "✓ Banco inicializado com sucesso!"
echo "=========================================="
