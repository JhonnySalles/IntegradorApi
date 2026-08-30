# Autenticação e Cliente REST API - Auth_and_ApiClient

## 🎯 Objetivo / Contexto

O projeto `IntegradorApi.Api` é a biblioteca responsável por gerenciar toda a comunicação HTTP entre o integrador desktop e as APIs REST remotas. Ele provê um cliente HTTP genérico reutilizável (`ApiClientService`), gerenciamento transparente de tokens de autenticação JWT (`ApiAuthManager`) e serviços especializados para cada módulo de integração.

## 🧩 Arquivos e Componentes

- **`IntegradorApi.Api/Core/ApiAuthManager.cs`**: Gerenciador estático do estado de autenticação em memória.
- **`IntegradorApi.Api/Services/ApiClientService.cs`**: Cliente HTTP genérico que encapsula `HttpClient`, injeção de headers Bearer e rotinas GET, PATCH e DELETE.
- **`IntegradorApi.Api/Models/SignInRequest.cs`**: DTO de credenciais para autenticação (`Username`, `Password`).
- **`IntegradorApi.Api/Models/AuthResponse.cs`**: DTO de resposta do servidor REST contendo `AccessToken`, `RefreshToken`, `Authenticated`, `Expiration` e `Username`.
- **`IntegradorApi.Api/Models/PagedResponse.cs`**: Estrutura genérica de resposta paginada (`PagedApiResponse<T>`, `PageableDto`).

## 🔐 Fluxo de Autenticação JWT (`ApiAuthManager` & `ApiClientService`)

1. **Checagem de Estado**: Antes de cada chamada REST (GET, PATCH ou DELETE), o `ApiClientService` invoca `SignInAsync()`.
2. **Verificação de Autenticação**: Se `ApiAuthManager.IsAuthenticated` for falso ou o token estiver expirado, ele limpa as credenciais existentes e realiza uma requisição `POST /auth/signin`.
3. **Payload de Login**:
   ```json
   {
     "username": "usuario_configurado",
     "password": "senha_configurada"
   }
   ```
4. **Armazenamento do Token**: Ao receber a resposta `AuthResponse` com `authenticated = true`, armazena os valores no `ApiAuthManager`:
   - `AccessToken`: Token JWT usado nas requisições subsequentes.
   - `RefreshToken`: Token de renovação.
   - `Expiration`: Data/hora de expiração do token.
5. **Injeção de Cabeçalho**: Adiciona o cabeçalho `Authorization: Bearer <AccessToken>` em todas as requisições enviadas ao servidor REST.
6. **Tratamento de 401 Unauthorized**: Se um endpoint responder com `HTTP 401`, o `ApiAuthManager.ClearAuthentication()` é executado para forçar um novo login na próxima tentativa.

## ⚙️ Métodos Genéricos do `ApiClientService`

### 1. `GetAsync<T>(string requestUri)`
- **Objetivo**: Executa uma requisição `HTTP GET` para buscar dados ou atualizações paginadas.
- **Parâmetros**: `requestUri` (URI relativa do endpoint, ex.: `/api/manga-extractor/tabela/volume/atualizacao/2026-08-30T00:00:00?page=0&size=20&direction=asc`).
- **Retorno**: Objeto deserializado do tipo `T` (ex.: `PagedApiResponse<MangaVolumeDto>`) ou `null` em caso de erro.

### 2. `PatchAsync(string requestUri, object data)`
- **Objetivo**: Executa uma requisição `HTTP PATCH` enviando uma lista de objetos serializada em JSON para inserção ou atualização no servidor remoto.
- **Parâmetros**: `requestUri` (ex.: `/api/manga-extractor/tabela/volume/lista`), `data` (lista de DTOs).
- **Retorno**: `bool` indicando sucesso da operação (`StatusCode 2xx`).

### 3. `DeleteAsync(string requestUri, object data)`
- **Objetivo**: Executa uma requisição `HTTP DELETE` enviando uma lista de objetos no corpo JSON para remoção no servidor remoto.
- **Parâmetros**: `requestUri` (ex.: `/api/manga-extractor/tabela/volume/lista`), `data` (lista de DTOs).
- **Retorno**: `bool` indicando sucesso da operação (`StatusCode 2xx`).

## 🩺 Endpoint de Teste de Saúde (`Health Check`)

- **URL**: `GET /health`
- **Utilização**: Invocado pelo método `TestConnection` no desktop client quando a conexão é do tipo `APIREST`.
- **Formato Esperado**: Retorno HTTP `200 OK` com `Content-Type: text/plain`.
