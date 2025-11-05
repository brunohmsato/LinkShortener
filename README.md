# 🔗 Link Shortener – Encurtador de Links com Analytics

Um projeto desenvolvido em **.NET 10 (C#)**, seguindo os princípios da **Clean Architecture**, com foco em **aprendizado, boas práticas e escalabilidade**.  
O objetivo foi criar um **SaaS completo** para encurtar URLs, rastrear acessos e fornecer métricas detalhadas.

---

## Tecnologias Utilizadas

| Camada | Tecnologias |
|---------|--------------|
| **Backend (API)** | ASP.NET Core Web API |
| **Arquitetura** | Clean Architecture (Domain → Application → Infrastructure → Presentation) |
| **Banco de Dados** | SQLite + Entity Framework Core |
| **Autenticação** | JWT (JSON Web Token) |
| **Controle de Acesso** | `[Authorize]`, `[AllowAnonymous]`, `[EnableRateLimiting]` |
| **Segurança** | Rate Limiter + HTTPS Redirection |
| **Documentação** | Swagger / OpenAPI |
| **Logging & Exception** | Middleware global de exceções |
| **Hospedagem** | Compatível com Render / Azure App Service |

---

## Funcionalidades

✅ **Criação de links curtos**  
- Endpoint: `POST /api/links`  
- Recebe uma URL e retorna o link encurtado.  
- Exemplo de resposta:
  ```json
  {
    "id": "b9e9e45d-0d43-4d6f-8c15-4c6ce09f1cc2",
    "code": "AbC123",
    "shortUrl": "https://localhost:7133/AbC123",
    "targetUrl": "https://www.microsoft.com",
    "createdAt": "2025-11-05T16:20:00Z"
  }
  ```

✅ **Redirecionamento público**  
- Endpoint: `GET /{code}`  
- Redireciona o usuário para o destino original.  
- Registra automaticamente os dados de acesso (referrer, IP, user-agent, UTM).

✅ **Analytics e estatísticas**  
- Endpoint: `GET /api/links/{id}/stats`  
- Retorna número total de cliques, origens, campanhas e agrupamento por dia.

✅ **Autenticação JWT**  
- Login: `POST /api/auth/login`  
- Consulta de perfil: `GET /api/auth/me`  
- Tokens com expiração e validação de emissor/audience.

✅ **Proteção contra abuso (Rate Limiter)**  
- Permite até 10 criações de link a cada 60s por IP.  
- Retorna HTTP 429 se excedido.

---

## Exemplo de Login

**POST** `/api/auth/login`

```json
{
  "email": "teste@email.com",
  "password": "1234"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs..."
}
```

Use esse token no Swagger (botão **Authorize**) para acessar endpoints protegidos.

---

## Estrutura do Projeto

```
src/
 ├── LinkShortener.Domain/
 │    ├── Abstractions/
 │    ├── Entities/
 │    └── Interfaces/
 ├── LinkShortener.Application/
 │    ├── Contracts/
 │    ├── Interfaces/
 │    ├── Services/
 │    └── ConfigureServices.cs
 ├── LinkShortener.Infrastructure/
 │    ├── Migrations/
 │    ├── Persistence/
 │    ├── Repositories/
 │    └── ConfigureServices.cs
 └── LinkShortener.Presentation/
      ├── Controllers/
      ├── Handlers/
      ├── Services/
      ├── Program.cs
      └── appsettings.json
```

---

## Execução Local

```bash
# Restaurar pacotes
dotnet restore

# Executar migrações (opcional)
dotnet ef database update --project LinkShortener.Infrastructure

# Rodar a API
dotnet run --project LinkShortener.Presentation
```

Abra o Swagger em:
```
https://localhost:7133/swagger
```

---

## Exemplos de Rotas

| Tipo | Endpoint | Autenticação | Descrição |
|------|-----------|---------------|------------|
| `POST` | `/api/auth/login` | ❌ | Gera token JWT |
| `GET` | `/api/auth/me` | ✅ | Retorna usuário autenticado |
| `POST` | `/api/links` | ✅ | Cria novo link curto |
| `GET` | `/api/links/{id}/stats` | ✅ | Retorna estatísticas |
| `GET` | `/{code}` | ❌ | Redireciona publicamente |

---

## Próximos Passos

- [ ] Criar front-end em React para dashboard de estatísticas  
- [ ] Gerar QR Codes para links curtos  
- [ ] Implementar refresh tokens  
- [ ] Adicionar suporte multi-tenant  
- [ ] Deploy automatizado via GitHub Actions  

---

## Autor

**Bruno Sato**  
Desenvolvedor .NET • Entusiasta de arquitetura limpa e SaaS  
[LinkedIn](https://www.linkedin.com/in/brunohmsato) • [GitHub](https://github.com/brunohmsato)

---

> *Este projeto foi criado como base de estudo e demonstração prática de Clean Architecture e autenticação JWT no .NET*
