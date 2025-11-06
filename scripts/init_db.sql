\c lastlink_products;

-- Tabela principal de produtos
CREATE TABLE IF NOT EXISTS "Products" (
    "Id" UUID PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Category" TEXT NOT NULL,
    "UnitCost" NUMERIC(18,2) NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabela de eventos de produto (para o consumidor RabbitMQ registrar)
CREATE TABLE IF NOT EXISTS "ProductEvents" (
    "Id" UUID PRIMARY KEY,
    "EventType" TEXT NOT NULL,
    "Payload" TEXT NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Índices auxiliares para performance e auditoria
CREATE INDEX IF NOT EXISTS idx_products_createdat ON "Products" ("CreatedAt");
CREATE INDEX IF NOT EXISTS idx_productevents_createdat ON "ProductEvents" ("CreatedAt");