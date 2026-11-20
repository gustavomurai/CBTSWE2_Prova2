# Prova2 — Sistema de Gerenciamento de Usuários e Produtos

**Disciplina:** CBTSWE2  
**Instituição:** Instituto Federal de São Paulo (IFSP) — Campus Cubatão

## Integrantes

- **Gustavo Cerqueira Murai** — CB3033261
- **Igor Cerqueira Murai** — CB3033295

---

## Sobre o projeto

O Prova2 é um sistema de gerenciamento desenvolvido em C# e .NET, composto por três aplicações integradas: uma API, uma aplicação Web e uma aplicação Desktop.

O sistema permite realizar operações de gerenciamento de usuários e produtos, utilizando autenticação e comunicação entre aplicações.

## Estrutura do projeto

```text
CBTSWE2_Prova2/
│
├── Prova2.Api/
│   ├── Controllers/
│   ├── Data/
│   ├── DTOs/
│   ├── Migrations/
│   ├── Models/
│   └── Program.cs
│
├── Prova2.Desktop/
│   ├── Form1.cs
│   ├── Form1.Designer.cs
│   └── Program.cs
│
├── Prova2.Web/
│   ├── Controllers/
│   ├── Models/
│   ├── Views/
│   └── Program.cs
│
└── CBTSWE2_Prova2.sln
```

## Tecnologias utilizadas

- C#
- .NET
- ASP.NET Core Web API
- ASP.NET Core MVC
- Windows Forms
- Entity Framework Core
- SQLite
- JWT (JSON Web Token)
- HTML e CSS

## Funcionalidades implementadas

### 1. API

- Cadastro de usuários.
- Autenticação de usuários.
- Geração e utilização de tokens JWT.
- Consulta de usuários.
- Consulta de produtos.
- Operações de gerenciamento por meio de endpoints REST.
- Persistência de dados.

### 2. Aplicação Web

- Tela de autenticação.
- Listagem de produtos.
- Cadastro de produtos.
- Edição de produtos.
- Exclusão de produtos.
- Controle de status dos produtos.
- Identificação dos usuários responsáveis pelo cadastro e atualização.

### 3. Aplicação Desktop

- Tela de login.
- Cadastro de usuários.
- Consulta de usuários.
- Busca por ID.
- Atualização de usuários.
- Exclusão de usuários.
- Ativação e desativação de usuários.
- Integração com a API.

## Como executar o projeto

### Pré-requisitos

- Visual Studio com suporte ao desenvolvimento .NET.
- SDK do .NET compatível com os arquivos `.csproj`.
- Entity Framework Core, conforme as dependências do projeto.

### Passo 1 — Abrir a solução

Abra o arquivo:

`CBTSWE2_Prova2.sln`

### Passo 2 — Restaurar dependências

No Visual Studio, execute a restauração dos pacotes NuGet.

### Passo 3 — Preparar o banco de dados

Configure a conexão SQLite e as chaves necessárias nos arquivos de configuração locais.

Caso seja necessário criar o banco utilizando as migrations, execute:

```bash
dotnet ef database update --project Prova2.Api --startup-project Prova2.Api
```

### Passo 4 — Executar a API

Inicie primeiro o projeto `Prova2.Api`.

### Passo 5 — Executar as aplicações

Com a API em execução, inicie:

- `Prova2.Web` para acessar o sistema pelo navegador.
- `Prova2.Desktop` para acessar o gerenciamento de usuários pelo Windows Forms.

Verifique os endereços configurados nos projetos antes da execução.

## Autenticação e segurança

A API utiliza autenticação baseada em JWT para controlar o acesso aos recursos protegidos.

As configurações sensíveis devem ser fornecidas localmente e não devem ser publicadas no repositório.

## Objetivo acadêmico

Este projeto foi desenvolvido como atividade acadêmica da disciplina CBTSWE2, com o objetivo de aplicar conhecimentos de desenvolvimento em C#, integração entre aplicações, APIs REST, autenticação, persistência de dados e interfaces gráficas.

---

## Autores

**Gustavo Cerqueira Murai** — CB3033261  
**Igor Cerqueira Murai** — CB3033295

IFSP — Campus Cubatão