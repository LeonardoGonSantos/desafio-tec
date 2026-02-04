# POST /api/auth/login

Autentica o usuário e retorna um token JWT.

## Descrição

O endpoint recebe credenciais (usuário e senha). Em ambiente de demonstração, aceita credenciais fixas e emite um JWT válido por 60 minutos. Em produção, deve-se integrar com Identity/banco de usuários e hash de senhas.

## Diagrama de Sequência

```mermaid
sequenceDiagram
    participant Client
    participant API as AuthController
    participant Config as IConfiguration

    Client->>API: POST /api/auth/login (body)
    API->>API: Valida username e password
    alt Credenciais inválidas
        API-->>Client: 401 Unauthorized
    else Credenciais válidas
        API->>Config: GetSection Jwt
        API->>API: GenerateJwtToken(username)
        API-->>Client: 200 OK + token e expiresAt
    end
```

## Request

**Headers**

- `Content-Type: application/json`

**Body**

```json
{
  "username": "admin",
  "password": "admin123"
}
```

| Campo    | Tipo   | Obrigatório | Descrição   |
|----------|--------|-------------|-------------|
| username | string | Sim         | Nome do usuário |
| password | string | Sim         | Senha       |

**Credenciais de demonstração:** `admin` / `admin123`

## Response

**200 OK**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-02-03T13:00:00Z"
}
```

**401 Unauthorized**

```json
{
  "message": "Credenciais inválidas"
}
```

## Códigos de Status

| Código | Descrição           |
|--------|---------------------|
| 200    | Token gerado        |
| 401    | Credenciais inválidas |
