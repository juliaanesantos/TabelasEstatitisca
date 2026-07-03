# Marketplace.Auth.API

API RESTful de autenticação e gerenciamento de usuários para o ecossistema Marketplace.

---

## Sumário

- [Visão Geral](#visão-geral)
- [Configuração](#configuração)
- [Ciclo de Vida dos Tokens](#ciclo-de-vida-dos-tokens)
- [Endpoints](#endpoints)
  - [Autenticação](#autenticação)
  - [Usuários](#usuários)
- [Guia de Integração para Outros Microsserviços](#guia-de-integração-para-outros-microsserviços)
- [Fluxo Completo de Autenticação](#fluxo-completo-de-autenticação)
- [Respostas de Erro](#respostas-de-erro)

---

## Visão Geral

Esta API é responsável exclusivamente por:

1. Registrar e gerenciar usuários.
2. Autenticar usuários e emitir tokens JWT (access token) e refresh tokens.
3. Renovar access tokens usando um refresh token válido.
4. Encerrar sessões específicas (logout).
5. Redefinir senhas via e-mail.

Todas as outras APIs do Marketplace **validam o JWT localmente** — sem precisar chamar esta API a cada requisição.

---

## Configuração

```json
{
  "Jwt": {
    "Chave": "<chave-secreta-forte>",
    "Emissor": "Marketplace.Auth.API",
    "Audiencia": "Marketplace.Auth.Clientes",
    "ExpiracaoMinutos": "60",
    "RefreshTokenExpiracaoDias": "7"
  }
}
```

| Parâmetro                   | Descrição                                          | Padrão |
|-----------------------------|----------------------------------------------------|--------|
| `Chave`                     | Chave HMAC-SHA256 para assinar o JWT               | —      |
| `Emissor`                   | `iss` claim do token                               | —      |
| `Audiencia`                 | `aud` claim do token                               | —      |
| `ExpiracaoMinutos`          | Validade do access token em minutos                | `60`   |
| `RefreshTokenExpiracaoDias` | Validade do refresh token em dias                  | `7`    |

> **Outras APIs:** configure `AddJwtBearer` com a mesma `Chave`, `Emissor` e `Audiencia` — veja o exemplo em [Guia de Integração](#guia-de-integração-para-outros-microsserviços).

---

## Ciclo de Vida dos Tokens

```
Login
  >> Novo Access Token  (JWT, válido por ExpiracaoMinutos — padrão 60 min)
  >> Novo Refresh Token (opaco, válido por RefreshTokenExpiracaoDias — padrão 7 dias)
  >> Sessões anteriores NÃO são invalidadas (multi-sessão ativo)

Enquanto o Refresh Token for válido:
  POST /api/autenticacao/refresh-token
    >> Novo Access Token  [gerado]
    >> Refresh Token      [o mesmo — não é rotacionado]

POST /api/autenticacao/logout
  >> Refresh Token informado é revogado imediatamente
  >> Outras sessões do mesmo usuário NÃO são afetadas

Refresh Token expirado (após 7 dias):
  >> Usuário deve fazer login novamente
```

**Regras importantes:**

- **Multi-sessão:** cada login gera um refresh token independente. Celular, notebook e outros clientes podem estar logados simultaneamente.
- O refresh token **não é rotacionado** a cada renovação. O mesmo token continua válido até expirar ou ser revogado via logout.
- Logout **apenas revoga o token informado** — não afeta outras sessões.
- Refresh token expirado ou revogado → a API retorna `400` → o cliente deve redirecionar para login.

---

## Endpoints

### Autenticação

#### `POST /api/autenticacao/login`

Autentica o usuário e retorna os tokens. Não requer autenticação.

**Body:**
```json
{
  "email": "usuario@exemplo.com",
  "senha": "Senha@123"
}
```

**Resposta `200 OK`:**
```json
{
  "accessToken": "eyJhbGci...",
  "accessTokenExpiresIn": "2026-01-15T11:00:00Z",
  "refreshToken": "GC5KxMwi...",
  "refreshTokenExpiresIn": "2026-01-22T10:00:00Z",
  "usuarioId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nome": "Joao Silva",
  "email": "usuario@exemplo.com"
}
```

| Campo                    | Descrição                                     |
|--------------------------|-----------------------------------------------|
| `accessToken`            | JWT para usar no header `Authorization`       |
| `accessTokenExpiresIn`   | Quando o access token expira (UTC)            |
| `refreshToken`           | Token opaco para renovar o access token       |
| `refreshTokenExpiresIn`  | Quando o refresh token expira (UTC)           |
| `usuarioId`              | ID do usuário autenticado                     |
| `nome`                   | Nome do usuário                               |
| `email`                  | E-mail do usuário                             |

> O `refreshToken` é gerado em Base64 e pode conter `+`, `/` e `=`.
> Sempre envie dentro de um body JSON — nunca via query string ou URL.

---

#### `POST /api/autenticacao/refresh-token`

Renova o access token usando um refresh token válido. **O refresh token não muda.**
Não requer autenticação.

**Body:**
```json
{
  "token": "<refresh-token>"
}
```

**Resposta `200 OK`:**
```json
{
  "accessToken": "eyJhbGci...",
  "accessTokenExpiresIn": "2026-01-15T12:00:00Z"
}
```

| Campo                   | Descrição                               |
|-------------------------|-----------------------------------------|
| `accessToken`           | Novo JWT                                |
| `accessTokenExpiresIn`  | Quando o novo access token expira (UTC) |

**Erros:**

| Status | Situação                                   |
|--------|--------------------------------------------|
| `400`  | Refresh token inválido ou expirado         |
| `404`  | Usuário associado ao token não encontrado  |

---

#### `POST /api/autenticacao/logout`

Invalida o refresh token da sessão atual. Não afeta outras sessões do mesmo usuário.
Não requer autenticação — funciona mesmo com access token expirado.

**Body:**
```json
{
  "token": "<refresh-token>"
}
```

**Resposta:** `204 No Content`

> Retorna `204` independentemente de o token existir ou já estar revogado — comportamento intencional para evitar enumeração de tokens.

---

#### `POST /api/autenticacao/esqueci-senha`

Envia um e-mail com token para redefinição de senha (válido por 2 horas).
Não requer autenticação.

**Body:**
```json
{
  "email": "usuario@exemplo.com"
}
```

**Resposta:** `204 No Content`

---

#### `POST /api/autenticacao/resetar-senha`

Redefine a senha usando o token recebido por e-mail.
Não requer autenticação.

**Body:**
```json
{
  "email": "usuario@exemplo.com",
  "token": "449733fe7bd24393ac72ed975993b1fe",
  "novaSenha": "NovaSenha@456"
}
```

**Resposta:** `204 No Content`

---

### Usuários

#### `POST /api/usuario`

Cria um novo usuário. Não requer autenticação.

**Body:**
```json
{
  "nome": "Joao Silva",
  "email": "joao@exemplo.com",
  "senha": "Senha@123",
  "documento": "01234567890",
  "tipoPessoa": 0,
  "funcao": 0,
  "nomeFantasia": null,
  "dataNascimento": "1990-05-20",
  "telefone": "54991234567"
}
```

| Campo           | Tipo       | Obrigatório | Descrição                                               |
|-----------------|------------|-------------|---------------------------------------------------------|
| `nome`          | string     | Sim         | Nome completo                                           |
| `email`         | string     | Sim         | E-mail único                                            |
| `senha`         | string     | Sim         | Mínimo 8 caracteres, letras maiúsculas, números e símbolos |
| `documento`     | string     | Sim         | CPF (PF) ou CNPJ (PJ), somente dígitos                 |
| `tipoPessoa`    | int        | Sim         | `0` = PessoaFisica, `1` = PessoaJuridica               |
| `funcao`        | int        | Não         | `0` = Comprador (padrão), `1` = Vendedor, `2` = Administrador |
| `nomeFantasia`  | string     | Não         | Apenas para PessoaJuridica                              |
| `dataNascimento`| DateOnly   | Não         | Formato `YYYY-MM-DD`                                    |
| `telefone`      | string     | Não         | Somente dígitos                                         |

**Resposta `201 Created`:**
```json
{
  "id": "a7ebcf6b-dc0c-4d43-ae77-eff70a77fc64",
  "nome": "Joao Silva",
  "nomeFantasia": null,
  "email": "joao@exemplo.com",
  "documento": "01234567890",
  "dataNascimento": "1990-05-20",
  "telefone": "54991234567",
  "tipoPessoa": 0,
  "funcao": 0,
  "status": 0,
  "criadoEm": "2026-04-23T14:54:38Z"
}
```

---

#### `GET /api/usuario/{id}`

Retorna os dados de um usuário pelo ID.
Requer autenticação.

**Resposta `200 OK`:** `UsuarioDto` (mesmo schema do `POST /api/usuario`)

---

#### `PUT /api/usuario/{id}`

Atualiza os dados de um usuário.
Requer autenticação.

**Body:**
```json
{
  "nome": "Joao Atualizado",
  "email": "novo@exemplo.com",
  "nomeFantasia": null,
  "dataNascimento": "1990-05-20",
  "telefone": "54999998888"
}
```

> `nomeFantasia`, `dataNascimento` e `telefone` são opcionais.
> `documento` e `tipoPessoa` não podem ser alterados após o cadastro.

**Resposta `200 OK`:** `UsuarioDto` atualizado

---

#### `DELETE /api/usuario/{id}`

Remove um usuário.
Requer autenticação com role `Administrador`.

**Resposta:** `204 No Content`

---

## Guia de Integração para Outros Microsserviços

### 1. Validar o JWT localmente

Configure `AddJwtBearer` em cada serviço com os mesmos parâmetros desta API:

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Chave"]!)),
            ValidateIssuer = true,
            ValidIssuer = "Marketplace.Auth.API",
            ValidateAudience = true,
            ValidAudience = "Marketplace.Auth.Clientes",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
```

> Com `ClockSkew = TimeSpan.Zero`, o token expira exatamente em `accessTokenExpiresIn`, sem margem extra.

### 2. Claims disponíveis no token

| Claim   | Conteúdo             |
|---------|----------------------|
| `sub`   | `usuarioId` (Guid)   |
| `email` | E-mail do usuário    |
| `name`  | Nome do usuário      |
| `role`  | Função do usuário    |
| `jti`   | ID único do token    |

### 3. Tratamento de 401 nos clientes

```
1. Chamada para qualquer API  →  401 Unauthorized
2.   POST /api/autenticacao/refresh-token
        200 OK  →  salvar novo accessToken e repetir a chamada original
        400     →  refresh token expirado/revogado → redirecionar para login
```

Nunca tente renovar o token de forma recursiva — se o refresh também falhar, force o logout.

---

## Fluxo Completo de Autenticação

```
Cliente (dispositivo A)          Auth API                Outra API
  |                                  |                       |
  |-- POST /login ------------------->|                       |
  |<-- { accessToken,                 |                       |
  |      accessTokenExpiresIn,        |                       |
  |      refreshToken,                |                       |
  |      refreshTokenExpiresIn }      |                       |
  |                                   |                       |
  |  (dispositivo B também pode       |                       |
  |   fazer login independentemente)  |                       |
  |                                   |                       |
  |-- GET /recurso (Bearer) -------------------------------->  |
  |<-- 200 OK ---------------------------------------------- |
  |                                   |                       |
  |   (60 min depois)                 |                       |
  |-- GET /recurso (Bearer) -------------------------------->  |
  |<-- 401 Unauthorized ------------------------------------- |
  |                                   |                       |
  |-- POST /refresh-token ----------->|                       |
  |<-- { accessToken,                 |                       |
  |      accessTokenExpiresIn }       |                       |
  |                                   |                       |
  |-- GET /recurso (Bearer) -------------------------------->  |
  |<-- 200 OK ---------------------------------------------- |
  |                                   |                       |
  |   (usuário clica em "sair")       |                       |
  |-- POST /logout (refreshToken) --->|                       |
  |<-- 204 No Content ----------------|                       |
  |                                   |                       |
  |   (7 dias depois — RT expirado,   |                       |
  |    ou usuário fez logout)         |                       |
  |-- POST /refresh-token ----------->|                       |
  |<-- 400 Bad Request ---------------|                       |
  |                                   |                       |
  |-- [redirecionar para login]       |                       |
```

---

## Respostas de Erro

| Status | Situação                                         |
|--------|--------------------------------------------------|
| `400`  | Dados inválidos ou regra de negócio violada      |
| `401`  | Credenciais inválidas ou token ausente/expirado  |
| `404`  | Usuário não encontrado                           |
| `500`  | Erro interno do servidor                         |

**Exemplo `400` (validação):**
```json
{
  "erros": [
    { "campo": "Email", "mensagem": "E-mail inválido." }
  ]
}
```

**Exemplo `401`:**
```json
{
  "erro": "E-mail ou senha inválidos."
}
```
